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
using Client.Graphics;
using Client.Graphics.Elements;
using Client.Input;
using Client.Patterns.Chaining;
using OpenUO.Core.Patterns;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Testing
{
    public class TestRendering
    {
        private Camera2D camera;
        private TexturedElement[] _elements;
        private Vertices<VertexPositionTexture> _vertices;
        private Texture2D _texture;

        public TestRendering(IoCContainer container)
        {
            camera = new Camera2D();

            RenderDelegateChainStep renderStep = new RenderDelegateChainStep("TestRendering", Render);

            container.Resolve<IUIRenderChain>().RegisterStep(renderStep);

            IInput input = container.Resolve<IInput>();

            input.AddBinding(string.Empty, false, false, false, System.Windows.Forms.Keys.Up, null, (s, e) => { camera.Position += Vector3.UnitY; });
            input.AddBinding(string.Empty, false, false, false, System.Windows.Forms.Keys.Down, null, (s, e) => { camera.Position -= Vector3.UnitY; });            
            input.AddBinding(string.Empty, false, false, false, System.Windows.Forms.Keys.Left, null, (s, e) => { camera.Position += Vector3.UnitX; });
            input.AddBinding(string.Empty, false, false, false, System.Windows.Forms.Keys.Right, null, (s, e) => { camera.Position -= Vector3.UnitX; });

            IDeviceContextService deviceContextService = container.Resolve<IDeviceContextService>();
            DeviceContext context = deviceContextService.Context;

            deviceContextService.Form.ResizeEnd += new EventHandler(Form_ResizeEnd);

            _texture = Texture2D.FromFile(context, "Resources\\Helix.jpg", Usage.None, Pool.Managed);
            _elements = new TexturedElement[250];

            for (int i = 0; i < _elements.Length; i++)
            {
                _elements[i] = new TexturedElement(new SharpDX.Vector2(50, 50));
                _elements[i].Position = new Vector2(i * 20, i * 20);
            }

            _vertices = new Vertices<VertexPositionTexture>(context,
                new VertexPositionTexture() { Position = new Vector3(-0.5f, -0.5f, 0), TextureCoordinate = new Vector2(0, 0) },
                new VertexPositionTexture() { Position = new Vector3(-0.5f,  0.5f, 0), TextureCoordinate = new Vector2(1, 0) },
                new VertexPositionTexture() { Position = new Vector3( 0.5f, -0.5f, 0), TextureCoordinate = new Vector2(0, 1) },
                new VertexPositionTexture() { Position = new Vector3( 0.5f,  0.5f, 0), TextureCoordinate = new Vector2(1, 1) });
        }

        void Form_ResizeEnd(object sender, EventArgs e)
        {

        }

        private void Render(DrawState state)
        {
            SimpleTextureEffect shader = state.GetShader<SimpleTextureEffect>();
            
            state.PushCamera(camera);

            shader.Texture = _texture;
            shader.Bind(state);

            _vertices.Draw(state, null, PrimitiveType.TriangleStrip);

            state.PopCamera();

        }
    }
}
