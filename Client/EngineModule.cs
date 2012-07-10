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

using System.Linq;
using Client.Components;
using Client.Configuration;
using Client.Diagnostics.Tracer;
using Client.Graphics;
using Client.Input;
using Client.Network;
using Client.Patterns.Chaining;
using Client.Testing;
using OpenUO.Core.Diagnostics;
using OpenUO.Core.Patterns;
using OpenUO.Ultima;

namespace Client
{
    public class EngineModule : ModuleBase
    {
        protected override void Initialize()
        {
#if DEBUG
            new DebugTraceListener { TraceLevel = TraceLevels.Verbose };
#endif

            //Register engine managers
            Container.Register<IConfiguration, ConfigurationManager>().AsSingleton();
            Container.Register<IDeviceContextService, DeviceContextManager>().AsSingleton();
            Container.Register<IInput, InputManager>().AsSingleton();
            Container.Register<INetwork, NetworkManager>().AsSingleton();
            Container.Register<IShaderManager, ShaderManager>().AsSingleton();

            //Register Render Chains
            Container.Register<IWorldRenderChain, WorldRenderChain>().AsSingleton();
            Container.Register<IUIRenderChain, UIRenderChain>().AsSingleton();

            //Register Update Chain
            Container.Register<IChain<UpdateState>, UpdateChain>().AsSingleton();

            //Kernel.Bind<IStorageAdapterParameterBuilder>().To<DirectX9UnicodeFontAdapterParameterBuilder>()
            //    .When(request => 
            //        request.ParentRequest.Service == typeof(IUnicodeFontFactory<Texture2D>));

            //Register UO file factories
            //Kernel.Bind<IAnimationDataFactory<AnimationData>>().To<AnimationDataFactory<AnimationData>>().InSingletonScope();
            //Kernel.Bind<IAnimationFactory<Texture2D>>().To<AnimationFactory<Texture2D>>().InSingletonScope();
            //Kernel.Bind<IArtworkFactory<Texture2D>>().To<ArtworkFactory<Texture2D>>().InSingletonScope();
            //Kernel.Bind<IASCIIFontFactory<Texture2D>>().To<ASCIIFontFactory<Texture2D>>().InSingletonScope();
            //Kernel.Bind<IGumpFactory<Texture2D>>().To<GumpFactory<Texture2D>>().InSingletonScope();
            //Kernel.Bind<ISkillsFactory<Skill>>().To<SkillsFactory<Skill>>().InSingletonScope();
            //Kernel.Bind<ISoundFactory<Sound>>().To<SoundFactory<Sound>>().InSingletonScope();
            //Kernel.Bind<ITexmapFactory<Texture2D>>().To<TexmapFactory<Texture2D>>().InSingletonScope();
            //Kernel.Bind<IUnicodeFontFactory<Texture2D>>().To<UnicodeFontFactory<Texture2D>>().InSingletonScope();

            //Register client components
            Container.Register<IConsole>().AsSingleton();

            IConfiguration config = Container.Resolve<IConfiguration>();
            config.RestoreDefaultsInvoked += OnConfigRestoreDefaultsInvoked;

            if (config.GetValue<bool>(ConfigSections.Diagnostics, ConfigKeys.ShowConsole))
            {
                NativeMethods.AllocConsole();

                if (config.GetValue<bool>(ConfigSections.Diagnostics, ConfigKeys.ConsoleTraceListener, false))
                    new ConsoleTraceListener();
            }

            if (config.GetValue<bool>(ConfigSections.Diagnostics, ConfigKeys.FileTraceListener, false))
                new DebugLogTraceListener("debug.log");

            IDeviceContextService deviceContextService = Container.Resolve<IDeviceContextService>();

            IShaderManager shaderManager = Container.Resolve<IShaderManager>();
            shaderManager.Register<SimpleTextureEffect>(new SimpleTextureEffect(deviceContextService.Context));
            shaderManager.Register<CombineShader>(new CombineShader(deviceContextService.Context));

            new GameConsoleTracer(Container);

            new TestRendering(Container);
        }

        private void OnConfigRestoreDefaultsInvoked(object sender, System.EventArgs e)
        {
            IConfiguration config = Container.Resolve<IConfiguration>();

            config.SetValue(ConfigSections.Client, ConfigKeys.GameWidth, 1024);
            config.SetValue(ConfigSections.Client, ConfigKeys.GameHeight, 768);

#if DEBUG
            config.SetValue(ConfigSections.Diagnostics, ConfigKeys.ShowConsole, true);
#else
            config.SetValue(ConfigSections.Diagnostics, ConfigKeys.ShowConsole, false);
#endif
            config.SetValue(ConfigSections.Diagnostics, ConfigKeys.ConsoleTraceListener, false);
            config.SetValue(ConfigSections.Diagnostics, ConfigKeys.FileTraceListener, true);

            InstallLocation install = InstallationLocator.Locate().FirstOrDefault();

            if (install != null)
                config.SetValue(ConfigSections.Client, ConfigKeys.InstallLocation, install);
        }

        public override string Name
        {
            get { return "EngineModule"; }
        }
    }
}
