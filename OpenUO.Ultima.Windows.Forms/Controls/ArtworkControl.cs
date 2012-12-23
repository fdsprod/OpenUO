#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   ArtworkControl.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

#endregion

namespace OpenUO.Ultima.Windows.Forms.Controls
{
    public enum ArtworkControlType
    {
        Land,
        Statics
    }

    public sealed class ArtworkControl : Control
    {
        private const int MAX_WIDTH = 48;
        private const int MAX_HEIGHT = 48;
        private readonly Dictionary<int, Bitmap> _cache;
        private readonly Rectangle _cellBounds;

        private readonly VScrollBar _scrollBar;
        private ArtworkControlType _artworkControlType;
        private ArtworkFactory _artworkFactory;
        private int _visibleColumnsCount;
        private int _visibleRowsCount;

        public ArtworkControl()
        {
            DoubleBuffered = true;

            _cache = new Dictionary<int, Bitmap>();

            _cellBounds = new Rectangle(
                0,
                0,
                MAX_WIDTH,
                MAX_HEIGHT);

            _cellBounds.Inflate(new Size(-2, -2));

            _scrollBar = new VScrollBar();
            _scrollBar.ValueChanged += OnScrollbarValueChanged;

            Recalculate(true);
            Controls.Add(_scrollBar);
        }

        public ArtworkControlType ArtworkControlType
        {
            get { return _artworkControlType; }
            set
            {
                if (_artworkControlType != value)
                {
                    _artworkControlType = value;

                    Recalculate(true);
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ArtworkFactory Factory
        {
            get { return _artworkFactory; }
            set
            {
                if (_artworkFactory != value)
                {
                    _artworkFactory = value;

                    Recalculate(true);
                    Invalidate();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_cache != null)
            {
                foreach (var value in _cache.Values)
                {
                    value.Dispose();
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            Recalculate(false);

            base.OnResize(e);
        }

        private void Recalculate(bool resetScrollbar)
        {
            _scrollBar.Location = new Point(Width - _scrollBar.Width - 2, 1);
            _scrollBar.Size = new Size(_scrollBar.Width, Height - 2);
            _visibleColumnsCount = Math.Max(0, (((Width - _scrollBar.Width) - 1) / MAX_WIDTH));
            _visibleRowsCount = Math.Max(0, ((Height - 1) / MAX_HEIGHT));

            if (_visibleColumnsCount == 0)
            {
                _scrollBar.Maximum = 0;
                return;
            }

            if (_artworkFactory == null)
            {
                return;
            }

            if (_artworkControlType == ArtworkControlType.Land)
            {
                int count = _artworkFactory.GetLandTileCount<Bitmap>() / _visibleColumnsCount;

                if (_artworkFactory.GetStaticTileCount<Bitmap>() % _visibleColumnsCount > 0)
                {
                    count++;
                }

                _scrollBar.Maximum = count;
            }
            else
            {
                int count = _artworkFactory.GetStaticTileCount<Bitmap>() / _visibleColumnsCount;

                if (_artworkFactory.GetStaticTileCount<Bitmap>() % _visibleColumnsCount > 0)
                {
                    count++;
                }

                _scrollBar.Maximum = count;
            }

            if (resetScrollbar)
            {
                _scrollBar.Value = 0;
            }
        }

        private void OnScrollbarValueChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Brush backBrush = new LinearGradientBrush(_cellBounds, Color.Gainsboro, Color.White, LinearGradientMode.ForwardDiagonal))
            using (Brush borderBrush = new SolidBrush(Color.LightSteelBlue))
            using (Pen borderPen = new Pen(borderBrush))
            {
                int startingIndex = _scrollBar.Value * _visibleColumnsCount;

                for (int y = 0; y < _visibleRowsCount; y++)
                {
                    e.Graphics.TranslateTransform(0, y * MAX_HEIGHT);

                    for (int x = 0; x < _visibleColumnsCount; x++)
                    {
                        int index = startingIndex + ((y * _visibleColumnsCount) + x);

                        e.Graphics.FillRectangle(backBrush, _cellBounds);

                        if (_artworkFactory != null)
                        {
                            Bitmap bmp = null;

                            if (_artworkControlType == ArtworkControlType.Land)
                            {
                                if (!_cache.TryGetValue(index, out bmp))
                                {
                                    bmp = _artworkFactory.GetLand<Bitmap>(index);
                                    _cache.Add(index, bmp);
                                }
                            }
                            else
                            {
                                if (!_cache.TryGetValue(index, out bmp))
                                {
                                    bmp = _artworkFactory.GetStatic<Bitmap>(index);
                                    _cache.Add(index, bmp);
                                }
                            }

                            if (bmp != null)
                            {
                                e.Graphics.DrawImageUnscaledAndClipped(bmp, _cellBounds);
                            }
                        }

                        e.Graphics.DrawRectangle(borderPen, _cellBounds);
                        e.Graphics.TranslateTransform(MAX_WIDTH, 0);
                    }

                    e.Graphics.ResetTransform();
                }

                e.Graphics.ResetTransform();
            }

            if (_artworkFactory == null)
            {
                TextRenderingHint textRenderingHint = e.Graphics.TextRenderingHint;
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                using (Brush backBrush = new SolidBrush(Color.Red))
                using (Brush foreBrush = new SolidBrush(Color.Maroon))
                using (StringFormat format = new StringFormat())
                {
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Center;

                    e.Graphics.DrawString("ArtworkControl.Factory is not set.", Font, backBrush, new RectangleF(0, 0, Width, Height), format);
                    e.Graphics.DrawString("ArtworkControl.Factory is not set.", Font, foreBrush, new RectangleF(1, 1, Width, Height), format);
                }

                e.Graphics.TextRenderingHint = textRenderingHint;
            }

            using (Brush borderBrush = new SolidBrush(Color.LightSteelBlue))
            using (Pen borderPen = new Pen(borderBrush))
            {
                e.Graphics.DrawRectangle(borderPen, new Rectangle(0, 0, Width - 1, Height - 1));
            }
        }
    }
}