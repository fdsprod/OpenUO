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
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics.Elements
{
    public abstract class ElementRect : Element
    {
        private VertexPositionColorTexture[] _vertexData = new VertexPositionColorTexture[4];
        private Vertices<VertexPositionColorTexture> _vertices;
        private bool _clipChildren = false; 
        private bool _dirty = true; 
        private bool _normalised;
        private Vector2 _size;
        private ElementScaling _hSize;
        private ElementScaling _vSize;
        private List<Element> _children;
        private bool _enableDepthTest;

        public ElementScaling VerticalScaling
        {
            get { return _vSize; }
            set { _vSize = value; }
        }

        public ElementScaling HorizontalScaling
        {
            get { return _hSize; }
            set { _hSize = value; }
        }

        public bool DrawAtMaxZDepth
        {
            get { return _enableDepthTest; }
            set
            {
                if (_enableDepthTest != value)
                {
                    _enableDepthTest = value;
                    _dirty = true;
                }
            }
        }

        public bool NormalisedCoordinates
        {
            get { return _normalised; }
            set { _normalised = value; }
        }

        public bool ClipChildren
        {
            get { return _clipChildren; }
            set { _clipChildren = value; }
        }

        public Vector2 Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    SizeChanged();
                }
            }
        }

        public sealed override List<Element> Children
        {
            get { return _children; }
        }

        protected override sealed Vector2 ElementSize
        {
            get { return _size; }
        }

        protected override sealed bool ClipsChildren
        {
            get { return _clipChildren && _children != null; }
        }

        protected override sealed bool IsNormalised
        {
            get { return _normalised; }
        }

        protected ElementRect(Vector2 sizeInPixels)
            : this(sizeInPixels, false) { }

        protected ElementRect(Vector2 size, bool normalised)
        {
            _normalised = normalised;
            Size = size;
        }

        public void Add(Element element)
        {
            if (element == null)
                throw new ArgumentNullException();
            if (element.Parent != null)
                throw new InvalidOperationException("element.parent");

            SetParent(element);

            if (_children == null)
                _children = new List<Element>();
            _children.Add(element);
        }

        public void Remove(Element element)
        {
            if (_children != null && element != null && element.Parent == this)
            {
                _children.Remove(element);
                ResetParent(element);
            }
        }

        protected override void GetDisplayMatrix(out Matrix matrix, Vector2 scale, ref Vector2 elementSize)
        {
            if (_hSize == ElementScaling.FillToParentPlusSize)
                elementSize.X += _normalised ? (1 - Position.X) : (scale.X - Position.X);

            if (_vSize == ElementScaling.FillToParentPlusSize)
                elementSize.Y += _normalised ? (1 - Position.Y) : (scale.Y - Position.Y);

            base.GetDisplayMatrix(out matrix, scale, ref elementSize);
        }

        protected override void DrawElement(DrawState state)
        {
            bool depthState = false;

            if (_enableDepthTest)
            {
                depthState = state.RenderState.DepthColourCull.DepthTestEnabled;
                state.RenderState.DepthColourCull.DepthTestEnabled = true;
            }
            
            _vertices.Draw(state, null, PrimitiveType.TriangleStrip);

            if (_enableDepthTest)
                state.RenderState.DepthColourCull.DepthTestEnabled = depthState;
        }

        protected override void PreDraw(DeviceContext context, Vector2 size)
        {
            if (_dirty)
            {
                _vertexData[0].Position = new Vector3(0, 0, 1);
                _vertexData[1].Position = new Vector3(1, 0, 1);
                _vertexData[2].Position = new Vector3(0, 1, 1);
                _vertexData[3].Position = new Vector3(1, 1, 1);

                WriteTextureCoords(
                    ref _vertexData[0].TextureCoordinate,
                    ref _vertexData[1].TextureCoordinate, 
                    ref _vertexData[2].TextureCoordinate, 
                    ref _vertexData[3].TextureCoordinate);

                WriteColours(
                    ref _vertexData[0].Color, 
                    ref _vertexData[1].Color,
                    ref _vertexData[2].Color, 
                    ref _vertexData[3].Color);

                if (_vertices == null)
                    _vertices = new Vertices<VertexPositionColorTexture>(context, _vertexData);
                else
                    _vertices.SetDirty();
                
                _dirty = false;
            }
        }

        protected void SetDirty()
        {
            _dirty = true;
        }

        protected virtual void SizeChanged()
        {

        }
    }
}
