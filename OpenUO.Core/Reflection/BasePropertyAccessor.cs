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

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace OpenUO.Core.Reflection
{
    public static class BasePropertyAccessor
    {
        public static FastInvokeHandler GetPropertyInvoker(Type TargetType, string PropertyName)
        {
            return BaseMethodInvoker.GetMethodInvoker(TargetType.GetMethod("get_" + PropertyName));
        }

        public static FastInvokeHandler SetPropertyInvoker(Type TargetType, string PropertyName)
        {
            return GetMethodInvoker4Set(TargetType.GetMethod("set_" + TargetType.GetProperty(PropertyName).Name));
        }

        public static FastInvokeHandler GetPropertyInvoker(Type TargetType, PropertyInfo Property)
        {
            return BaseMethodInvoker.GetMethodInvoker(TargetType.GetMethod("get_" + Property.Name));
        }

        public static FastInvokeHandler SetPropertyInvoker(Type TargetType, PropertyInfo Property)
        {
            return GetMethodInvoker4Set(TargetType.GetMethod("set_" + Property.Name));
        }

        private static FastInvokeHandler GetMethodInvoker4Set(MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty,
                typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module);

            ILGenerator il = dynamicMethod.GetILGenerator();
            ParameterInfo[] ps = methodInfo.GetParameters();
            Type[] paramTypes = new Type[ps.Length];

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    paramTypes[i] = ps[i].ParameterType.GetElementType();
                else
                    paramTypes[i] = ps[i].ParameterType;
            }

            LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

            for (int i = 0; i < paramTypes.Length; i++)
                locals[i] = il.DeclareLocal(paramTypes[i], true);

            for (int i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitFastInt(il, i);
                il.Emit(OpCodes.Ldelem_Ref);
                EmitCastToReference(il, paramTypes[i]);
                il.Emit(OpCodes.Stloc, locals[i]);
            }

            if (!methodInfo.IsStatic)
                il.Emit(OpCodes.Ldarg_0);

            for (int i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(ps[i].ParameterType.IsByRef ? OpCodes.Ldloca_S : OpCodes.Ldloc, locals[i]);
            }

            il.EmitCall(!methodInfo.IsStatic ? OpCodes.Callvirt : OpCodes.Call, methodInfo, null);

            if (methodInfo.ReturnType == typeof(void))
                il.Emit(OpCodes.Ldnull);
            else
                EmitBoxIfNeeded(il, methodInfo.ReturnType);

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(il, i);
                    il.Emit(OpCodes.Ldloc, locals[i]);
                    if (locals[i].LocalType.IsValueType)
                        il.Emit(OpCodes.Box, locals[i].LocalType);
                    il.Emit(OpCodes.Stelem_Ref);
                }
            }

            il.Emit(OpCodes.Ret);
            FastInvokeHandler invoder = (FastInvokeHandler)dynamicMethod.CreateDelegate(typeof(FastInvokeHandler));
            return invoder;
        }


        private static void EmitCastToReference(ILGenerator il, Type type)
        {
            il.Emit(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);
        }

        private static void EmitBoxIfNeeded(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
        }

        private static void EmitFastInt(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
            }

            if (value > -129 && value < 128)
            {
                il.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4, value);
            }
        }
    }
}
