#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   GumpControl.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

#endregion

namespace OpenUO.Ultima.Windows.Forms.Controls
{
    public sealed class GumpControl : Control
    {
        private readonly VScrollBar _scrollBar;
        private GumpFactory _gumpFactory;

        public GumpControl()
        {
            DoubleBuffered = true;

            _scrollBar = new VScrollBar();
            _scrollBar.ValueChanged += OnScrollbarValueChanged;
            _scrollBar.Dock = DockStyle.Right;

            Controls.Add(_scrollBar);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GumpFactory Factory
        {
            get { return _gumpFactory; }
            set
            {
                if (_gumpFactory != value)
                {
                    _gumpFactory = value;
                    _scrollBar.Maximum = _gumpFactory.GetLength<Bitmap>();
                    Invalidate();
                }
            }
        }

        private void OnScrollbarValueChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        protected override void OnResize(EventArgs e)
        {
            _scrollBar.Location = new Point(Width - _scrollBar.Width - 2, 1);
            _scrollBar.Size = new Size(_scrollBar.Width, Height - 2);

            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int centerX = (Width - 1) / 2;
            int centerY = (Height - 1) / 2;

            if (_gumpFactory != null)
            {
                Bitmap image = _gumpFactory.GetGump<Bitmap>(_scrollBar.Value);

                if (image != null)
                {
                    int widthOver2 = image.Width / 2;
                    int heightOver2 = image.Height / 2;

                    e.Graphics.DrawImage(image, new Rectangle(centerX - widthOver2, centerY - heightOver2, image.Width, image.Height));
                }
            }

            if (_gumpFactory == null)
            {
                TextRenderingHint textRenderingHint = e.Graphics.TextRenderingHint;
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                using (Brush backBrush = new SolidBrush(Color.Red))
                using (Brush foreBrush = new SolidBrush(Color.Maroon))
                using (StringFormat format = new StringFormat())
                {
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Center;

                    e.Graphics.DrawString("GumpControl.Factory is not set.", Font, backBrush, new RectangleF(0, 0, Width, Height), format);
                    e.Graphics.DrawString("GumpControl.Factory is not set.", Font, foreBrush, new RectangleF(1, 1, Width, Height), format);
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