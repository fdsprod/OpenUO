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
using Client.Configuration;
using Client.Graphics;
using OpenUO.Core.Patterns;
using OpenUO.Ultima;
using SharpDX;

namespace Client.Ultima
{
    public class World : IDraw, IUpdate
    {
        private Camera2D _camera;
        private InstallLocation _install;
        private List<Facet> _facets;
        private IoCContainer _container;
        private Facet _currentFacet;

        public Mobile Player
        {
            get;
            set;
        }

        public Facet CurrentFacet
        {
            get { return _currentFacet; }
            set { _currentFacet = value; }
        }

        public World(IoCContainer container)
        {
            IConfiguration config = container.Resolve<IConfiguration>();

            _container = container;
            _install = config.GetValue<InstallLocation>(ConfigSections.Client, ConfigKeys.InstallLocation);

            _camera = new Camera2D();
			_facets = new List<Facet>();
        }

        public void RegisterFacet(int fileIndex, int mapID, int width, int height)
        {
            _facets.Add(new Facet(_install, this, fileIndex, mapID, width, height));
        }

        public void Update(UpdateState state)
        {

        }

        public unsafe void Draw(DrawState state)
        {
            DeviceContext context = state.Context;

            context.PerformanceMonitor.StartTimer("World.Draw");

            state.PushCamera(_camera);

            float length = new Vector2(context.Viewport.Width, context.Viewport.Height).Length();

            Vector2 position;

            position.X = Player.Position.X;
            position.Y = Player.Position.Y;

            Vector2 offset = new Vector2(position.X % 8, position.Y % 8);

            int overCellX = (int)Math.Round(position.X, 0, MidpointRounding.AwayFromZero);
            int overCellY = (int)Math.Round(position.Y, 0, MidpointRounding.AwayFromZero);

            int cellNodeX = (int)overCellX / 8;
            int cellNodeY = (int)overCellY / 8;

            int nodesToDraw = (int)Math.Round(length / 44f / 8f, 0, MidpointRounding.AwayFromZero);

            if (nodesToDraw % 2 == 0)
                nodesToDraw++;

            int nodesOverTwo = (int)Math.Round(nodesToDraw / 2f, 0, MidpointRounding.AwayFromZero);

            //int startNodeX = cellNodeX - nodesOverTwo;
            //int startNodeY = cellNodeY - nodesOverTwo;

            //int endNodeX = cellNodeX + nodesOverTwo;
            //int endNodeY = cellNodeY + nodesOverTwo;

            //state.BeginEffect("ColorEffect");

            //if (_stream == null)
            //    _stream = new VertexStream<VertexPositionColor, short>(state.Context, streamSize, (streamSize / 4) * 6);

            //_stream.Begin(PrimitiveType.TriangleList);

            //for (int y = startNodeY; y < endNodeY; y++)
            //{
            //    for (int x = startNodeX; x < endNodeX; x++)
            //    {
            //        for (int ty = 0; ty < 8; ty++)
            //        {
            //            for (int tx = 0; tx < 8; tx++)
            //            {
            //                Vector2 p = new Vector2(x * 8 + tx, y * 8 + ty);
            //                Color4 color = new Color4(0,0,0,1);

            //                if (p.X == overCellX && p.Y == overCellY)
            //                    color.Red = 1;

            //                FragmentLocation location;
            //                _stream.Allocate(VertexFragment.Quad, out location);

            //                VertexPositionColor* vPtr = (VertexPositionColor*)location.Vertices;
            //                ushort* iPtr = (ushort*)location.Indices;

            //                *iPtr++ = (ushort)(location.BaseIndex + 0);
            //                *iPtr++ = (ushort)(location.BaseIndex + 1);
            //                *iPtr++ = (ushort)(location.BaseIndex + 2);
            //                *iPtr++ = (ushort)(location.BaseIndex + 2);
            //                *iPtr++ = (ushort)(location.BaseIndex + 1);
            //                *iPtr = (ushort)(location.BaseIndex + 3);

            //                vPtr->Position.X = p.X;
            //                vPtr->Position.Y = p.Y;
            //                vPtr->Position.Z = 0;
            //                vPtr->Color = color;
            //                vPtr++;
            //                vPtr->Position.X = p.X + 1;
            //                vPtr->Position.Y = p.Y;
            //                vPtr->Position.Z = 0;
            //                vPtr->Color = color;
            //                vPtr++;
            //                vPtr->Position.X = p.X;
            //                vPtr->Position.Y = p.Y + 1;
            //                vPtr->Position.Z = 0;
            //                vPtr->Color = color;
            //                vPtr++;
            //                vPtr->Position.X = p.X + 1;
            //                vPtr->Position.Y = p.Y + 1;
            //                vPtr->Position.Z = 0;
            //                vPtr->Color = color;
            //            }
            //        }
            //    }
            //}

            //state.SetStream(_stream);

            //int primitiveCount;

            //context.CalculatePrimitiveCount(_stream.PrimitiveType, _stream.IndexCount, out primitiveCount);
            //context.DrawIndexedPrimitive(_stream.PrimitiveType, 0, 0, _stream.VertexCount, 0, primitiveCount);

            //state.ReleaseStream();
            //_stream.End();

            //state.EndEffect();
            //state.PopProjectionMatrix();
            //state.PopViewMatrix();

            context.PerformanceMonitor.StopTimer("World.Draw");
        }
    }
}
