#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   PropertyAccesor.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;
using System.Reflection;

#endregion

namespace OpenUO.Core.Reflection
{
    public class PropertyAccessor<TargetType, PropertyType>
    {
        protected FastInvokeHandler getMethodHandler;
        protected FastInvokeHandler setMethodHandler;

        public PropertyAccessor(string Property)
        {
            getMethodHandler = BasePropertyAccessor.GetPropertyInvoker(typeof (TargetType), Property);
            setMethodHandler = BasePropertyAccessor.SetPropertyInvoker(typeof (TargetType), typeof (TargetType).GetProperty(Property));
        }

        public PropertyAccessor(PropertyInfo Property)
        {
            getMethodHandler = BasePropertyAccessor.GetPropertyInvoker(typeof (TargetType), Property.Name);
            setMethodHandler = BasePropertyAccessor.SetPropertyInvoker(typeof (TargetType), Property);
        }

        public PropertyType Get(TargetType TargetObject, params object[] Paramters)
        {
            return (PropertyType)getMethodHandler(TargetObject, Paramters);
        }

        public void Set(TargetType TargetObject, params object[] Paramters)
        {
            setMethodHandler(TargetObject, Paramters);
        }

        public override string ToString()
        {
            return "Property Invoker : " + getMethodHandler.Method.Name.Substring(4);
        }
    }

    public class PropertyAccessor
    {
        protected FastInvokeHandler getMethodHandler;
        protected FastInvokeHandler setMethodHandler;

        public PropertyAccessor(Type targetType, string Property)
        {
            getMethodHandler = BasePropertyAccessor.GetPropertyInvoker(targetType, Property);
            setMethodHandler = BasePropertyAccessor.SetPropertyInvoker(targetType, targetType.GetProperty(Property));
        }

        public PropertyAccessor(Type targetType, PropertyInfo Property)
        {
            getMethodHandler = BasePropertyAccessor.GetPropertyInvoker(targetType, Property.Name);
            setMethodHandler = BasePropertyAccessor.SetPropertyInvoker(targetType, Property);
        }

        public object Get(object target, params object[] Paramters)
        {
            return getMethodHandler(target, Paramters);
        }

        public void Set(object target, params object[] Paramters)
        {
            setMethodHandler(target, Paramters);
        }

        public override string ToString()
        {
            return "Property Invoker : " + getMethodHandler.Method.Name.Substring(4);
        }
    }
}