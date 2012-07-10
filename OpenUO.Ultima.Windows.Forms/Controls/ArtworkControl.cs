#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace OpenUO.Ultima.Windows.Forms.Controls
{
    public enum ArtworkControlType
    {
        Land,
        Statics
    }

    public sealed class ArtworkControl : Control
    {
        private ArtworkFactory _artworkFactory;
        private ArtworkControlType _artworkControlType;

        public ArtworkControlType ArtworkControlType
        {
            get { return _artworkControlType; }
            set 
            {
                if (_artworkControlType != value)
                {
                    _artworkControlType = value;
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
                    Invalidate();
                }
            }
        }

        public ArtworkControl()
        {
            DoubleBuffered = true;            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //if (!DesignModeUtility.IsDesignMode)
            //{
                const int MAX_WIDTH = 48;
                const int MAX_HEIGHT = 48;
                
                Rectangle itemBounds = new Rectangle(
                    0,
                    0,
                    MAX_WIDTH,
                    MAX_HEIGHT);
                
                itemBounds.Inflate(new Size(-2, -2));

                using (Brush backBrush = new LinearGradientBrush(itemBounds, Color.Gainsboro, Color.White, LinearGradientMode.ForwardDiagonal))
                using (Brush borderBrush = new SolidBrush(Color.LightSteelBlue))//(itemBounds, Color.Gainsboro, Color.White, LinearGradientMode.BackwardDiagonal))
                using (Pen borderPen = new Pen(borderBrush))
                {
                    int columns = Math.Max(0, ((Width - 1) / MAX_WIDTH));
                    int rows = Math.Max(0, ((Height - 1) / MAX_HEIGHT));

                    int startingIndex = 0;

                    for (int y = 0; y < rows; y++)
                    {
                        e.Graphics.TranslateTransform(0, y * MAX_HEIGHT);

                        for (int x = 0; x < columns; x++)
                        {
                            int index = startingIndex + ((y * columns) + x);

                            e.Graphics.FillRectangle(backBrush, itemBounds);
                            e.Graphics.DrawRectangle(borderPen, itemBounds);

                            if (_artworkFactory != null)
                            {
                                Bitmap bmp = null;
                                
                                if(_artworkControlType == Forms.Controls.ArtworkControlType.Land)
                                    bmp = _artworkFactory.GetLand<Bitmap>(index);
                                else
                                    bmp = _artworkFactory.GetStatic<Bitmap>(index);

                                if (bmp != null)
                                {
                                    e.Graphics.DrawImageUnscaledAndClipped(bmp, itemBounds);
                                }
                            }

                            e.Graphics.TranslateTransform(MAX_WIDTH, 0);
                        }

                        e.Graphics.ResetTransform();
                    }

                    e.Graphics.ResetTransform();
                }
            //}

            if (_artworkFactory == null)
            {
                var textRenderingHint = e.Graphics.TextRenderingHint;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

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
        }
    }
}
