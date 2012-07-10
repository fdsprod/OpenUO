using System;
using Microsoft.Practices.Unity;

namespace OpenUO.Core.PresentationFramework.ComponentModel
{
    public sealed class SingletonFactory : InstanceFactory
    {
        public SingletonFactory(IUnityContainer container)
            : base(container)
        {

        }

        protected override void EnsureRegistered(Type type)
        {
            if (!Container.IsRegistered(type))
                Container.RegisterType(type, new ContainerControlledLifetimeManager());
        }
    }
}
