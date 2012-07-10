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
using System.Text;
using Client.ChainSteps;
using Client.Graphics;
using Client.Input;
using Client.Patterns.Chaining;
using OpenUO.Core.Patterns;
using OpenUO.Ultima;

namespace Client.Components
{
    internal class Console : IConsole
    {
        public const int FONT_INDEX = 0;

        private bool _isOpen;
        private Shader _shader;
        private List<ConsoleLine> _lines;
        private Stack<Color> _colorStack;
        private StringBuilder _cursorText;
        //private IUnicodeFontFactory<Texture2D> _fontFactory;
        //private VertexStream<VertexPositionTexture, short> _vertexStream;

        public bool IsOpen
        {
            get { return _isOpen; }
        }

        public Console(IoCContainer container)
        {
            IInput input = container.Resolve<IInput>();

            input.KeyDown += OnInputKeyDown;

            _cursorText = new StringBuilder();
            _lines = new List<ConsoleLine>();
            _colorStack = new Stack<Color>();
            _colorStack.Push(System.Drawing.Color.White);
            //_shader = container.Resolve<IShaderManager>().GetShader("DiffuseEffect");
            //_fontFactory = container.Resolve<IUnicodeFontFactory<Texture2D>>();
            //_vertexStream = new VertexStream<VertexPositionTexture, short>(container.Resolve<IDeviceContextService>().Context, 4 * 20 + 12, 6 * 20 + 18);

            container.Resolve<IUIRenderChain>().RegisterStep(new DelegateChainStepBase<DrawState>("Console", Render));
        }

        public void PushColor(Color color)
        {
            _colorStack.Push(color);
        }

        public void PopColor()
        {
            if (_colorStack.Count > 1)
                _colorStack.Pop();
        }

        public ConsoleLine[] GetHistory(int count)
        {
            int index = 0;

            if (_lines.Count > 20)
                index = _lines.Count - 20;

            if (_lines.Count < 20)
                count = _lines.Count;

            return _lines.GetRange(index, count).ToArray();
        }

        public void WriteLine(string format, params object[] args)
        {
            _lines.Add(new ConsoleLine(string.Format(format, args), _colorStack.Peek()));
        }

        private void Render(DrawState state)
        {
            if (!IsOpen)
                return;

            //DeviceContext context = state.Context;
            //Viewport viewport = context.Viewport;

            //_vertexStream.Begin(PrimitiveType.TriangleList);

            //float height = _fontFactory.GetFontHeight(FONT_INDEX);
            //float consoleHeight = height * 21;

            //AllocateQuad(0, 0, viewport.Width, consoleHeight);

            //ConsoleLine[] lines = GetHistory(20);
            //Texture2D[] textures = new Texture2D[lines.Length + 2];

            //for (int i = lines.Length - 1; i >= 0; i--)
            //{
            //    Texture2D texture = _fontFactory.GetText(FONT_INDEX, lines[0].Text, 0);

            //    const float x = 5;
            //    float y = consoleHeight - 5 - ((i + 2) * height);

            //    AllocateQuad(x, y, texture.Size.X, texture.Size.Y);

            //    textures[i + 1] = texture;
            //}

            //_vertexStream.Bind();

            //Matrix projection;
            //Matrix.OrthoOffCenterRH(0, viewport.Width, viewport.Height, 0, 0, 1, out projection);
            
            //state.Context.SetRenderState(RenderState.CullMode, Cull.Clockwise);

            //context.SetStreamSource(0, _vertexStream.VertexBuffer, 0, _vertexStream.VertexStride);
            //context.VertexDeclaration = _vertexStream.VertexDeclaration;
            //context.Indices = _vertexStream.IndexBuffer;

            //_shader.Bind(state);

            //for (int i = 0; i < textures.Length; i++)
            //{
            //    context.SetTexture(0, textures[i]);
            //    _shader.CommitChanges();
            //    context.DrawIndexedPrimitive(PrimitiveType.TriangleList, i * 4, 0, 4, 0, 2);
            //}

            //_vertexStream.End();

            //foreach (Texture texture in textures)
            //    if (texture != null)
            //        texture.Dispose();
        }

        private unsafe void AllocateQuad(float x, float y, float width, float height)
        {
            //FragmentLocation location;
            //_vertexStream.Allocate(VertexFragment.Quad, out location);

            //VertexPositionTexture* vPtr = (VertexPositionTexture*)location.Vertices;
            //ushort* iPtr = (ushort*)location.Indices;

            //*iPtr++ = (ushort)(location.BaseIndex + 0);
            //*iPtr++ = (ushort)(location.BaseIndex + 1);
            //*iPtr++ = (ushort)(location.BaseIndex + 2);
            //*iPtr++ = (ushort)(location.BaseIndex + 2);
            //*iPtr++ = (ushort)(location.BaseIndex + 1);
            //*iPtr = (ushort)(location.BaseIndex + 3);

            //vPtr->Position.X = x;
            //vPtr->Position.Y = y;
            //vPtr->Position.Z = 0;
            //vPtr->TextureCoordinate.X = 0;
            //vPtr->TextureCoordinate.Y = 0;
            //vPtr++;
            //vPtr->Position.X = x;
            //vPtr->Position.Y = y + height;
            //vPtr->Position.Z = 0;
            //vPtr->TextureCoordinate.X = 1;
            //vPtr->TextureCoordinate.Y = 0;
            //vPtr++;
            //vPtr->Position.X = x + width;
            //vPtr->Position.Y = y;
            //vPtr->Position.Z = 0;
            //vPtr->TextureCoordinate.X = 0;
            //vPtr->TextureCoordinate.Y = 1;
            //vPtr++;
            //vPtr->Position.X = x + width;
            //vPtr->Position.Y = y + height;
            //vPtr->Position.Z = 0;
            //vPtr->TextureCoordinate.X = 1;
            //vPtr->TextureCoordinate.Y = 1;
        }

        private void OnInputKeyDown(object sender, HandledKeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Oemtilde)
            {
                _isOpen = !_isOpen;
                e.Handled = true;
                return;
            }

            if (!_isOpen)
                return;

            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                ExecuteCommand();
                return;
            }

            if (e.KeyCode == System.Windows.Forms.Keys.Back)
            {
                _cursorText.Remove(_cursorText.Length - 1, 1);
                return;
            }

            _cursorText.Append(e.KeyValue);
        }

        private void ExecuteCommand()
        {
            WriteLine("Command: {0}", _cursorText);
            _cursorText.Clear();
        }
    }
}
