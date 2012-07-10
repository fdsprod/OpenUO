using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using OpenUO.Core.PresentationFramework.ComponentModel.Design;

namespace OpenUO.Core.PresentationFramework.ComponentModel
{
    public sealed class ViewModelLocator
    {
        private IUnityContainer _container;
        private Dictionary<Type, IFactory> _typeFactories;

        public ViewModelLocator(IUnityContainer container)
        {
            if (!container.IsRegistered<SingletonFactory>())
                container.RegisterType<SingletonFactory>();

            if (!container.IsRegistered<InstanceFactory>())
                container.RegisterType<InstanceFactory>();

            _typeFactories = new Dictionary<Type, IFactory>();
            _container = container;
        }

        public void RegisterViewModel<T>(bool singleton)
        {
            _typeFactories.Add(typeof(T), singleton ? _container.Resolve<SingletonFactory>() : _container.Resolve<InstanceFactory>());
        }

        public ViewModelBase Get(Type viewModel)
        {
            IFactory factory;

            if (!_typeFactories.TryGetValue(viewModel, out factory))
            {
                factory = _container.Resolve<InstanceFactory>();
            }

            return (ViewModelBase)factory.Create(viewModel);
        }
    }
}
