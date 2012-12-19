#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   MethodInvoker.cs
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
    public class MethodInvoker<TargetObjectType>
    {
        protected FastInvokeHandler methodHandler;

        public MethodInvoker(string Method)
        {
            methodHandler = BaseMethodInvoker.GetMethodInvoker(typeof (TargetObjectType).GetMethod(Method));
        }

        public MethodInvoker(MethodInfo Method)
        {
            methodHandler = BaseMethodInvoker.GetMethodInvoker(Method);
        }

        public object Invoke(TargetObjectType TargetObject, params object[] Paramters)
        {
            return methodHandler.Invoke(TargetObject, Paramters);
        }

        public IAsyncResult BeginInvoke(TargetObjectType TargetObject, object[] Paramters, AsyncCallback Callback, object @Object)
        {
            return methodHandler.BeginInvoke(TargetObject, Paramters, Callback, @Object);
        }

        public override string ToString()
        {
            return "Method Invoker : " + methodHandler.Method.Name;
        }
    }
}