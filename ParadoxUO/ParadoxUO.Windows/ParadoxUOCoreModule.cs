using OpenUO.Core.Net;
using OpenUO.Core.Patterns;
using OpenUO.Ultima.Paradox3d;

using ParadoxUO.Net;

namespace ParadoxUO
{
    internal sealed class ParadoxUOCoreModule : IModule
    {
        public string Name
        {
            get { return "ParadoxUO - Core Module"; }
        }

        public void OnLoad(IContainer container)
        {
            container.RegisterModule<OpenUOParadox3dModule>();

            container.Register<INetworkClient, NetworkClient>().AsSingleton();
        }

        public void OnUnload(IContainer container)
        {
            
        }
    }
}