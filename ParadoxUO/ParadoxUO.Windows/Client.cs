#region License Header

// Copyright (c) 2015 OpenUO Software Team.
// All Right Reserved.
// 
// Client.cs
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.

#endregion

#region Usings

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using OpenUO.Core.Net;
using OpenUO.Core.Patterns;
using OpenUO.Ultima;

using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Paradox.Engine;
using SiliconStudio.Paradox.Games;
using SiliconStudio.Paradox.Graphics;
using SiliconStudio.Paradox.Rendering;
using SiliconStudio.Paradox.Rendering.Composers;

#endregion

namespace ParadoxUO
{
    public interface IClient
    {
        IContainer Container
        {
            get;
        }
    }
    
    internal class Client : Game
    {
        private INetworkClient _network;

        public Client()
        {
            Container = new Container();

            Container.Register<IGame>(this);
            Container.Register(Services.GetServiceAs<IGamePlatform>());
            Container.Register<IGraphicsDeviceService>(GraphicsDeviceManager);
            Container.Register<IServiceRegistry>(Services);

            IsFixedTimeStep = false;
        }

        public IContainer Container
        {
            get;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _network = Container.Resolve<INetworkClient>();
        }

        protected override Task LoadContent()
        {
            Script.Add(new RenderScript(Container));

            return base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _network.Slice();
        }
    }

    public class RenderScript : AsyncScript
    {
        private SpriteBatch _spriteBatch;
        private TexmapFactory _texmapFactory;
        private ArtworkFactory _artworkFactory;
        private Texture _tile;
        private IContainer _container;

        public RenderScript(IContainer container)
        {
            _container = container;
        }

        private readonly Vector2 _virtualResolution = new Vector2(1280, 720);

        public override void Start()
        {
            base.Start();

            var installs = InstallationLocator.Locate();

            _spriteBatch = new SpriteBatch(GraphicsDevice) { VirtualResolution = new Vector3(_virtualResolution, 1) };

            _artworkFactory = new ArtworkFactory(installs.First(), _container);
            _texmapFactory = new TexmapFactory(installs.First(), _container);
            
            // register the renderer in the pipeline
            var scene = SceneSystem.SceneInstance.Scene;
            var compositor = ((SceneGraphicsCompositorLayers)scene.Settings.GraphicsCompositor);

            compositor.Master.Renderers.Add(new ClearRenderFrameRenderer());
            compositor.Master.Renderers.Add(new SceneDelegateRenderer(RenderQuad));

        }
        
        private void RenderQuad(RenderContext renderContext, RenderFrame frame)
        {
            GraphicsDevice.Clear(frame.DepthStencil, DepthStencilClearOptions.DepthBuffer);

            if (_tile != null)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(_tile, Vector2.Zero);
                _spriteBatch.End();
            }
        }
    }
}