using System;
namespace OpenUO.Core.Patterns
{
    public interface IContainer
    {
        void AutoRegister();
        void AutoRegister(bool ignoreDuplicateImplementations);
        void AutoRegister(bool ignoreDuplicateImplementations, Func<Type, bool> registrationPredicate);
        void AutoRegister(global::System.Collections.Generic.IEnumerable<global::System.Reflection.Assembly> assemblies);
        void AutoRegister(global::System.Collections.Generic.IEnumerable<global::System.Reflection.Assembly> assemblies, bool ignoreDuplicateImplementations);
        void AutoRegister(global::System.Collections.Generic.IEnumerable<global::System.Reflection.Assembly> assemblies, bool ignoreDuplicateImplementations, Func<Type, bool> registrationPredicate);
        void AutoRegister(global::System.Collections.Generic.IEnumerable<global::System.Reflection.Assembly> assemblies, Func<Type, bool> registrationPredicate);
        void AutoRegister(Func<Type, bool> registrationPredicate);
        void BuildUp(object input);
        void BuildUp(object input, global::OpenUO.Core.Patterns.ResolveOptions resolveOptions);
        bool CanResolve(Type resolveType);
        bool CanResolve(Type resolveType, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters);
        bool CanResolve(Type resolveType, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, global::OpenUO.Core.Patterns.ResolveOptions options);
        bool CanResolve(Type resolveType, global::OpenUO.Core.Patterns.ResolveOptions options);
        bool CanResolve(Type resolveType, string name, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters);
        bool CanResolve(Type resolveType, string name, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, global::OpenUO.Core.Patterns.ResolveOptions options);
        bool CanResolve(Type resolveType, string name, global::OpenUO.Core.Patterns.ResolveOptions options);
        bool CanResolve<ResolveType>() where ResolveType : class;
        bool CanResolve<ResolveType>(global::OpenUO.Core.Patterns.NamedParameterOverloads parameters) where ResolveType : class;
        bool CanResolve<ResolveType>(global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, global::OpenUO.Core.Patterns.ResolveOptions options) where ResolveType : class;
        bool CanResolve<ResolveType>(global::OpenUO.Core.Patterns.ResolveOptions options) where ResolveType : class;
        bool CanResolve<ResolveType>(string name) where ResolveType : class;
        bool CanResolve<ResolveType>(string name, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters) where ResolveType : class;
        bool CanResolve<ResolveType>(string name, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, global::OpenUO.Core.Patterns.ResolveOptions options) where ResolveType : class;
        bool CanResolve<ResolveType>(string name, global::OpenUO.Core.Patterns.ResolveOptions options) where ResolveType : class;
        void Dispose();
        global::OpenUO.Core.Patterns.Container GetChildContainer();
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register(Type registerType);
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register(Type registerType, Func<global::OpenUO.Core.Patterns.Container, global::OpenUO.Core.Patterns.NamedParameterOverloads, object> factory);
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register(Type registerType, Func<global::OpenUO.Core.Patterns.Container, global::OpenUO.Core.Patterns.NamedParameterOverloads, object> factory, string name);
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register(Type registerType, object instance);
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register(Type registerType, object instance, string name);
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register(Type registerType, string name);
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register(Type registerType, Type registerImplementation);
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register(Type registerType, Type registerImplementation, object instance);
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name);
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register(Type registerType, Type registerImplementation, string name);
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register<RegisterType, RegisterImplementation>()
            where RegisterType : class
            where RegisterImplementation : class, RegisterType;
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register<RegisterType, RegisterImplementation>(RegisterImplementation instance)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType;
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register<RegisterType, RegisterImplementation>(RegisterImplementation instance, string name)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType;
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register<RegisterType, RegisterImplementation>(string name)
            where RegisterType : class
            where RegisterImplementation : class, RegisterType;
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register<RegisterType>() where RegisterType : class;
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register<RegisterType>(RegisterType instance) where RegisterType : class;
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register<RegisterType>(RegisterType instance, string name) where RegisterType : class;
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register<RegisterType>(Func<global::OpenUO.Core.Patterns.Container, global::OpenUO.Core.Patterns.NamedParameterOverloads, RegisterType> factory) where RegisterType : class;
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register<RegisterType>(Func<global::OpenUO.Core.Patterns.Container, global::OpenUO.Core.Patterns.NamedParameterOverloads, RegisterType> factory, string name) where RegisterType : class;
        global::OpenUO.Core.Patterns.Container.RegisterOptions Register<RegisterType>(string name) where RegisterType : class;
        void RegisterModule<TModule>() where TModule : class, global::OpenUO.Core.Patterns.IModule;
        global::OpenUO.Core.Patterns.Container.MultiRegisterOptions RegisterMultiple(Type registrationType, global::System.Collections.Generic.IEnumerable<Type> implementationTypes);
        global::OpenUO.Core.Patterns.Container.MultiRegisterOptions RegisterMultiple<RegisterType>(global::System.Collections.Generic.IEnumerable<Type> implementationTypes);
        object Resolve(Type resolveType);
        object Resolve(Type resolveType, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters);
        object Resolve(Type resolveType, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, global::OpenUO.Core.Patterns.ResolveOptions options);
        object Resolve(Type resolveType, global::OpenUO.Core.Patterns.ResolveOptions options);
        object Resolve(Type resolveType, string name);
        object Resolve(Type resolveType, string name, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters);
        object Resolve(Type resolveType, string name, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, global::OpenUO.Core.Patterns.ResolveOptions options);
        object Resolve(Type resolveType, string name, global::OpenUO.Core.Patterns.ResolveOptions options);
        ResolveType Resolve<ResolveType>() where ResolveType : class;
        ResolveType Resolve<ResolveType>(global::OpenUO.Core.Patterns.NamedParameterOverloads parameters) where ResolveType : class;
        ResolveType Resolve<ResolveType>(global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, global::OpenUO.Core.Patterns.ResolveOptions options) where ResolveType : class;
        ResolveType Resolve<ResolveType>(global::OpenUO.Core.Patterns.ResolveOptions options) where ResolveType : class;
        ResolveType Resolve<ResolveType>(string name) where ResolveType : class;
        ResolveType Resolve<ResolveType>(string name, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters) where ResolveType : class;
        ResolveType Resolve<ResolveType>(string name, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, global::OpenUO.Core.Patterns.ResolveOptions options) where ResolveType : class;
        ResolveType Resolve<ResolveType>(string name, global::OpenUO.Core.Patterns.ResolveOptions options) where ResolveType : class;
        global::System.Collections.Generic.IEnumerable<object> ResolveAll(Type resolveType);
        global::System.Collections.Generic.IEnumerable<object> ResolveAll(Type resolveType, bool includeUnnamed);
        global::System.Collections.Generic.IEnumerable<ResolveType> ResolveAll<ResolveType>() where ResolveType : class;
        global::System.Collections.Generic.IEnumerable<ResolveType> ResolveAll<ResolveType>(bool includeUnnamed) where ResolveType : class;
        bool TryResolve(Type resolveType, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, global::OpenUO.Core.Patterns.ResolveOptions options, out object resolvedType);
        bool TryResolve(Type resolveType, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, out object resolvedType);
        bool TryResolve(Type resolveType, global::OpenUO.Core.Patterns.ResolveOptions options, out object resolvedType);
        bool TryResolve(Type resolveType, out object resolvedType);
        bool TryResolve(Type resolveType, string name, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, global::OpenUO.Core.Patterns.ResolveOptions options, out object resolvedType);
        bool TryResolve(Type resolveType, string name, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, out object resolvedType);
        bool TryResolve(Type resolveType, string name, global::OpenUO.Core.Patterns.ResolveOptions options, out object resolvedType);
        bool TryResolve(Type resolveType, string name, out object resolvedType);
        bool TryResolve<ResolveType>(global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, global::OpenUO.Core.Patterns.ResolveOptions options, out ResolveType resolvedType) where ResolveType : class;
        bool TryResolve<ResolveType>(global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, out ResolveType resolvedType) where ResolveType : class;
        bool TryResolve<ResolveType>(global::OpenUO.Core.Patterns.ResolveOptions options, out ResolveType resolvedType) where ResolveType : class;
        bool TryResolve<ResolveType>(out ResolveType resolvedType) where ResolveType : class;
        bool TryResolve<ResolveType>(string name, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, global::OpenUO.Core.Patterns.ResolveOptions options, out ResolveType resolvedType) where ResolveType : class;
        bool TryResolve<ResolveType>(string name, global::OpenUO.Core.Patterns.NamedParameterOverloads parameters, out ResolveType resolvedType) where ResolveType : class;
        bool TryResolve<ResolveType>(string name, global::OpenUO.Core.Patterns.ResolveOptions options, out ResolveType resolvedType) where ResolveType : class;
        bool TryResolve<ResolveType>(string name, out ResolveType resolvedType) where ResolveType : class;
    }
}
