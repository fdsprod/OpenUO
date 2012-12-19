#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   IContainer.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace OpenUO.Core.Patterns
{
    public interface IContainer : IDisposable
    {
        void AutoRegister();
        void AutoRegister(bool ignoreDuplicateImplementations);
        void AutoRegister(bool ignoreDuplicateImplementations, Func<Type, bool> registrationPredicate);
        void AutoRegister(IEnumerable<Assembly> assemblies);
        void AutoRegister(IEnumerable<Assembly> assemblies, bool ignoreDuplicateImplementations);
        void AutoRegister(IEnumerable<Assembly> assemblies, bool ignoreDuplicateImplementations, Func<Type, bool> registrationPredicate);
        void AutoRegister(IEnumerable<Assembly> assemblies, Func<Type, bool> registrationPredicate);
        void AutoRegister(Func<Type, bool> registrationPredicate);
        void BuildUp(object input);
        void BuildUp(object input, ResolveOptions resolveOptions);
        bool CanResolve(Type resolveType);
        bool CanResolve(Type resolveType, NamedParameterOverloads parameters);
        bool CanResolve(Type resolveType, NamedParameterOverloads parameters, ResolveOptions options);
        bool CanResolve(Type resolveType, ResolveOptions options);
        bool CanResolve(Type resolveType, string name, NamedParameterOverloads parameters);
        bool CanResolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options);
        bool CanResolve(Type resolveType, string name, ResolveOptions options);
        bool CanResolve<TResolveType>() where TResolveType : class;
        bool CanResolve<TResolveType>(NamedParameterOverloads parameters) where TResolveType : class;
        bool CanResolve<TResolveType>(NamedParameterOverloads parameters, ResolveOptions options) where TResolveType : class;
        bool CanResolve<TResolveType>(ResolveOptions options) where TResolveType : class;
        bool CanResolve<TResolveType>(string name) where TResolveType : class;
        bool CanResolve<TResolveType>(string name, NamedParameterOverloads parameters) where TResolveType : class;
        bool CanResolve<TResolveType>(string name, NamedParameterOverloads parameters, ResolveOptions options) where TResolveType : class;
        bool CanResolve<TResolveType>(string name, ResolveOptions options) where TResolveType : class;

        Container GetChildContainer();
        Container.RegisterOptions Register(Type registerType);
        Container.RegisterOptions Register(Type registerType, Func<Container, NamedParameterOverloads, object> factory);
        Container.RegisterOptions Register(Type registerType, Func<Container, NamedParameterOverloads, object> factory, string name);
        Container.RegisterOptions Register(Type registerType, object instance);
        Container.RegisterOptions Register(Type registerType, object instance, string name);
        Container.RegisterOptions Register(Type registerType, string name);
        Container.RegisterOptions Register(Type registerType, Type registerImplementation);
        Container.RegisterOptions Register(Type registerType, Type registerImplementation, object instance);
        Container.RegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name);
        Container.RegisterOptions Register(Type registerType, Type registerImplementation, string name);

        Container.RegisterOptions Register<TRegisterType, TRegisterImplementation>()
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType;

        Container.RegisterOptions Register<TRegisterType, TRegisterImplementation>(TRegisterImplementation instance)
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType;

        Container.RegisterOptions Register<TRegisterType, TRegisterImplementation>(TRegisterImplementation instance, string name)
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType;

        Container.RegisterOptions Register<TRegisterType, TRegisterImplementation>(string name)
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType;

        Container.RegisterOptions Register<TRegisterType>() where TRegisterType : class;
        Container.RegisterOptions Register<TRegisterType>(TRegisterType instance) where TRegisterType : class;
        Container.RegisterOptions Register<TRegisterType>(TRegisterType instance, string name) where TRegisterType : class;
        Container.RegisterOptions Register<TRegisterType>(Func<Container, NamedParameterOverloads, TRegisterType> factory) where TRegisterType : class;

        Container.RegisterOptions Register<TRegisterType>(Func<Container, NamedParameterOverloads, TRegisterType> factory, string name)
            where TRegisterType : class;

        Container.RegisterOptions Register<TRegisterType>(string name) where TRegisterType : class;
        void RegisterModule<TModule>() where TModule : class, IModule;
        Container.MultiRegisterOptions RegisterMultiple(Type registrationType, IEnumerable<Type> implementationTypes);
        Container.MultiRegisterOptions RegisterMultiple<TRegisterType>(IEnumerable<Type> implementationTypes);
        object Resolve(Type resolveType);
        object Resolve(Type resolveType, NamedParameterOverloads parameters);
        object Resolve(Type resolveType, NamedParameterOverloads parameters, ResolveOptions options);
        object Resolve(Type resolveType, ResolveOptions options);
        object Resolve(Type resolveType, string name);
        object Resolve(Type resolveType, string name, NamedParameterOverloads parameters);
        object Resolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options);
        object Resolve(Type resolveType, string name, ResolveOptions options);
        TResolveType Resolve<TResolveType>() where TResolveType : class;
        TResolveType Resolve<TResolveType>(NamedParameterOverloads parameters) where TResolveType : class;
        TResolveType Resolve<TResolveType>(NamedParameterOverloads parameters, ResolveOptions options) where TResolveType : class;
        TResolveType Resolve<TResolveType>(ResolveOptions options) where TResolveType : class;
        TResolveType Resolve<TResolveType>(string name) where TResolveType : class;
        TResolveType Resolve<TResolveType>(string name, NamedParameterOverloads parameters) where TResolveType : class;
        TResolveType Resolve<TResolveType>(string name, NamedParameterOverloads parameters, ResolveOptions options) where TResolveType : class;
        TResolveType Resolve<TResolveType>(string name, ResolveOptions options) where TResolveType : class;
        IEnumerable<object> ResolveAll(Type resolveType);
        IEnumerable<object> ResolveAll(Type resolveType, bool includeUnnamed);
        IEnumerable<TResolveType> ResolveAll<TResolveType>() where TResolveType : class;
        IEnumerable<TResolveType> ResolveAll<TResolveType>(bool includeUnnamed) where TResolveType : class;
        bool TryResolve(Type resolveType, NamedParameterOverloads parameters, ResolveOptions options, out object resolvedType);
        bool TryResolve(Type resolveType, NamedParameterOverloads parameters, out object resolvedType);
        bool TryResolve(Type resolveType, ResolveOptions options, out object resolvedType);
        bool TryResolve(Type resolveType, out object resolvedType);
        bool TryResolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options, out object resolvedType);
        bool TryResolve(Type resolveType, string name, NamedParameterOverloads parameters, out object resolvedType);
        bool TryResolve(Type resolveType, string name, ResolveOptions options, out object resolvedType);
        bool TryResolve(Type resolveType, string name, out object resolvedType);

        bool TryResolve<TResolveType>(NamedParameterOverloads parameters, ResolveOptions options, out TResolveType resolvedType)
            where TResolveType : class;

        bool TryResolve<TResolveType>(NamedParameterOverloads parameters, out TResolveType resolvedType) where TResolveType : class;
        bool TryResolve<TResolveType>(ResolveOptions options, out TResolveType resolvedType) where TResolveType : class;
        bool TryResolve<TResolveType>(out TResolveType resolvedType) where TResolveType : class;

        bool TryResolve<TResolveType>(string name, NamedParameterOverloads parameters, ResolveOptions options, out TResolveType resolvedType)
            where TResolveType : class;

        bool TryResolve<TResolveType>(string name, NamedParameterOverloads parameters, out TResolveType resolvedType) where TResolveType : class;
        bool TryResolve<TResolveType>(string name, ResolveOptions options, out TResolveType resolvedType) where TResolveType : class;
        bool TryResolve<TResolveType>(string name, out TResolveType resolvedType) where TResolveType : class;
    }
}