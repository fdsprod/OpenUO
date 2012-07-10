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

using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public abstract class Element : ICullable
    {
        private static ElementCamera _camera;

        private bool _enabled;
        private bool _clipTest;
        private HorizontalAlignment _horizontalAlignment;
        private VerticalAlignment _verticalAlignment;
        private AlphaBlendState _blendState;
        private Vector2 _position;

        internal Element _parent;

        public virtual List<Element> Children
        {
            get { return null; }
        }

        public AlphaBlendState BlendState
        {
            get { return _blendState; }
            set { _blendState = value; }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return _horizontalAlignment; }
            set { _horizontalAlignment = value; }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return _verticalAlignment; }
            set { _verticalAlignment = value; }
        }

        public bool Visible
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public Element Parent
        {
            get { return _parent; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        protected virtual bool ClipsChildren
        {
            get { return false; }
        }

        protected virtual bool IsNormalised
        {
            get { return false; }
        }

        protected virtual bool UseSize
        {
            get { return true; }
        }

        protected abstract Vector2 ElementSize
        {
            get;
        }

        protected Element()
        {
            _enabled = true;
        }

        protected void SetParent(Element element)
        {
            element._parent = this;
        }

        protected void ResetParent(Element element)
        {
            element._parent = null;
        }

        protected virtual void GetDisplayMatrix(out Matrix matrix, Vector2 scale, ref Vector2 elementSize)
        {
            if (IsNormalised)
                scale = new Vector2(1, 1);
            bool useSize = UseSize;

            if (useSize)
                matrix.M11 = elementSize.X / scale.X;
            else
                matrix.M11 = 1.0f / scale.X;
            matrix.M12 = 0;
            matrix.M13 = 0;
            matrix.M14 = 0;

            matrix.M21 = 0;
            if (useSize)
                matrix.M22 = elementSize.Y / scale.Y;
            else
                matrix.M22 = 1.0f / scale.Y;
            matrix.M23 = 0;
            matrix.M24 = 0;

            matrix.M31 = 0;
            matrix.M32 = 0;
            matrix.M33 = 1;
            matrix.M34 = 0;

            matrix.M41 = _position.X;
            matrix.M42 = _position.Y;
            matrix.M43 = 0;
            matrix.M44 = 1;

            switch (_verticalAlignment)
            {
                case VerticalAlignment.Centre:
                    matrix.M42 += (scale.Y - elementSize.Y) * 0.5f;
                    break;
                case VerticalAlignment.Top:
                    matrix.M42 += scale.Y - elementSize.Y;
                    break;
            }
            switch (_horizontalAlignment)
            {
                case HorizontalAlignment.Centre:
                    matrix.M41 += (scale.X - elementSize.X) * 0.5f;
                    break;
                case HorizontalAlignment.Right:
                    matrix.M41 += scale.X - elementSize.X;
                    break;
            }

            matrix.M41 /= scale.X;
            matrix.M42 /= scale.Y;
        }

        protected virtual void WriteTextureCoords(ref Vector2 topLeft, ref Vector2 topRight, ref Vector2 bottomLeft, ref Vector2 bottomRight)
        {
            topLeft = new Vector2(0, 0);
            topRight = new Vector2(1, 0);
            bottomLeft = new Vector2(0, 1);
            bottomRight = new Vector2(1, 1);
        }

        protected virtual void WriteColours(ref Color topLeft, ref Color topRight, ref Color bottomLeft, ref Color bottomRight)
        {
            topLeft = System.Drawing.Color.White;
            topRight = System.Drawing.Color.White;
            bottomLeft = System.Drawing.Color.White;
            bottomRight = System.Drawing.Color.White;
        }

        public void Render(DrawState state)
        {
            if (_enabled)
                Render(state, 255);
        }

        public void Render(DrawState state, byte clipDepth)
        {
            Element parent = _parent;
            Matrix matrix;
            DeviceContext context = state.Context;
            
            if (parent == null)
            {
                _clipTest = false;      

                DeviceRenderState rstate = new DeviceRenderState();

                rstate.DepthColourCull.DepthWriteEnabled = false;
                rstate.DepthColourCull.DepthTestEnabled = false;

                state.PushRenderState(ref rstate);

                if (_camera == null)
                    _camera = new ElementCamera(true);

                state.PushCamera(_camera);
            }
            else
                _clipTest = parent._clipTest | parent.ClipsChildren;

            StencilTestState stencilState = new StencilTestState();

            if (_clipTest)
            {
                stencilState.Enabled = true;
                stencilState.ReferenceValue = clipDepth;
                stencilState.StencilFunction = Compare.Equal;
                stencilState.StencilPassOperation = StencilOperation.Keep;
            }

            bool clearStencil = false;

            if (ClipsChildren)
            {
                clearStencil = clipDepth == 255;
                clipDepth--;

                if (!_clipTest)
                {
                    stencilState.Enabled = true;
                    stencilState.ReferenceValue = clipDepth;
                    stencilState.StencilPassOperation = StencilOperation.Replace;
                }
                else
                    stencilState.StencilPassOperation = StencilOperation.Decrement;
            }

            Viewport viewport = context.Viewport;
            Vector2 scale = new Vector2(viewport.Width, viewport.Height);

            if ((scale.X != 0 && scale.Y != 0))
            {
                Vector2 size = ElementSize;
                GetDisplayMatrix(out matrix, scale, ref size);

                state.PushWorldMatrixMultiply(ref matrix);

                BindShader(state, false);

                state.RenderState.AlphaBlend = _blendState;
                state.RenderState.StencilTest = stencilState;

                if (!UseSize)
                    size = new Vector2(1, 1);
                else if (IsNormalised)
                {
                    size.X *= scale.X;
                    size.Y *= scale.Y;
                }

                PreDraw(state.Context, size);

                DrawElement(state);

                List<Element> children = Children;
                if (children != null)
                    foreach (Element child in children)
                        if (child.CullTest(state))
                            child.Render(state, clipDepth);

                if (clearStencil)
                {
                    BindShader(state, true);

                    stencilState = new StencilTestState();
                    stencilState.Enabled = true;
                    stencilState.StencilFunction = Compare.Never;
                    stencilState.StencilFailOperation = StencilOperation.Zero;
                    state.RenderState.StencilTest = stencilState;

                    DrawElement(state);
                }

                state.PopWorldMatrix();
            }

            if (parent == null)
            {
                state.PopRenderState();
                state.PopCamera();
            }
        }

        public bool TryGetLayout(out Vector2 position, out Vector2 size, Vector2 targetSize)
        {
            position = new Vector2();
            size = new Vector2(1, 1);
            Matrix tmp = Matrix.Identity;
            Vector2 originalSize = targetSize;

            if (TryApplyLayout(ref position, ref size, ref targetSize, ref tmp))
            {
                position.X *= originalSize.X;
                position.Y *= originalSize.Y;
                size.X *= originalSize.X;
                size.Y *= originalSize.Y;
                return true;
            }
            else
            {
                position = new Vector2();
                size = new Vector2();
                return false;
            }
        }

        private bool TryApplyLayout(ref Vector2 position, ref Vector2 localScale, ref Vector2 scale, ref Matrix matrix)
        {
            if (_parent != null)
            {
                if (!_parent.TryApplyLayout(ref position, ref localScale, ref scale, ref matrix))
                    return false;
            }

            if ((scale.X != 0 && scale.Y != 0))
            {
                Vector2 size = ElementSize;
                GetDisplayMatrix(out matrix, scale, ref size);

                if (!UseSize)
                    size = new Vector2(1, 1);
                else if (IsNormalised)
                {
                    size.X *= scale.X;
                    size.Y *= scale.Y;
                }

                scale = size;

                Transform(ref position, ref matrix, out position);
                Vector2.TransformNormal(ref localScale, ref matrix, out localScale);

                return true;
            }

            return false;
        }

        private static void Transform(ref Vector2 source, ref Matrix transform, out Vector2 destination)
        {
            Vector4 output;
            Vector2.Transform(ref source, ref transform, out output);

            destination.X = output.X;
            destination.Y = output.Y;
        }

        protected abstract void DrawElement(DrawState state);

        protected virtual void PreDraw(DeviceContext context, Vector2 size) { }

        protected abstract void BindShader(DrawState state, bool maskOnly);

        public bool CullTest(ICuller culler)
        {
            return _enabled;
        }
    }
}
