using System;
using Microsoft.Practices.Unity;

namespace OpenUO.Core.PresentationFramework.ComponentModel
{
    public interface IFactory
    {
        object Create(Type type);
    }

    public class InstanceFactory : IFactory
    {
        protected IUnityContainer Container;

        public InstanceFactory(IUnityContainer container)
        {
            Container = container;
        }

        public virtual object Create(Type type)
        {
            EnsureRegistered(type);
            return Container.Resolve(type);            
        }

        protected virtual void EnsureRegistered(Type type)
        {
            if (!Container.IsRegistered(type))
                Container.RegisterType(type);
        }
    }
}
