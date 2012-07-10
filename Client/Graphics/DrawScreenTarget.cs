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

using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public sealed class DrawScreenTarget : DrawTarget
    {
        private RenderTarget2D _worldRenderTarget;
        private RenderTarget2D _uiRenderTarget;
        private Vertices<VertexPositionTexture> _vertices;

        public DrawScreenTarget(DeviceContext context)
            : base(context)
        {
            CreateRenderTarget();
        }

        private void CreateRenderTarget()
        {
            Viewport viewport = Context.Viewport;

            _worldRenderTarget = new RenderTarget2D(Context, viewport.Width, viewport.Height, Format.A8R8G8B8);
            _uiRenderTarget = new RenderTarget2D(Context, viewport.Width, viewport.Height, Format.A8R8G8B8);

            _vertices = new Vertices<VertexPositionTexture>(Context,
                new VertexPositionTexture() { Position = new Vector3(-1f, -1f, 0), TextureCoordinate = new Vector2(0, 0) },
                new VertexPositionTexture() { Position = new Vector3(-1f, 1f, 0), TextureCoordinate = new Vector2(0, 1) },
                new VertexPositionTexture() { Position = new Vector3(1f, -1f, 0), TextureCoordinate = new Vector2(1, 0) },
                new VertexPositionTexture() { Position = new Vector3(1f, 1f, 0), TextureCoordinate = new Vector2(1, 1) });
        }

        public void BeginWorld(DrawState state)
        {
            state.Context.SetRenderTarget(_worldRenderTarget);
            state.Context.Clear(ClearFlags.Target | ClearFlags.ZBuffer, System.Drawing.Color.CornflowerBlue, 1.0f, 0);
        }

        public void EndWorld(DrawState state)
        {
            state.Context.SetRenderTarget(null);
        }

        public void BeginUI(DrawState state)
        {
            state.Context.SetRenderTarget(_uiRenderTarget);
            state.Context.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Color4(0, 0, 0, 0), 1.0f, 0);
        }

        public void EndUI(DrawState state)
        {
            state.Context.SetRenderTarget(null);
        }

        protected override void OnContextReset(DeviceContext context)
        {
            base.OnContextReset(context);

            CreateRenderTarget();
        }

        public void Combine(DrawState state)
        {
            CombineShader shader = state.GetShader<CombineShader>();

            shader.WorldTexture = _worldRenderTarget;
            shader.UITexture = _uiRenderTarget;
            shader.Bind(state);

            _vertices.Draw(state, null, PrimitiveType.TriangleStrip);
        }
    }
}
