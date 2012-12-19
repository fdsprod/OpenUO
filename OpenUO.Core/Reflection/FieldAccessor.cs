#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   FieldAccessor.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System.Reflection;

#endregion

namespace OpenUO.Core.Reflection
{
    public class FieldAccessor<TargetType, FieldType>
    {
        protected FieldFastGetInvokeHandler<TargetType, FieldType> getMethodHandler;
        protected FieldFastSetInvokeHandler<TargetType, FieldType> setMethodHandler;

        public FieldAccessor(string Field)
        {
            getMethodHandler = BaseFieldAccessor.GetFieldInvoker<TargetType, FieldType>(typeof (TargetType).GetField(Field));
            setMethodHandler = BaseFieldAccessor.SetFieldInvoker<TargetType, FieldType>(typeof (TargetType).GetField(Field));
        }

        public FieldAccessor(FieldInfo Field)
        {
            getMethodHandler = BaseFieldAccessor.GetFieldInvoker<TargetType, FieldType>(Field);
            setMethodHandler = BaseFieldAccessor.SetFieldInvoker<TargetType, FieldType>(Field);
        }

        public FieldType Get(TargetType TargetObject)
        {
            return getMethodHandler(TargetObject);
        }

        public void Set(TargetType TargetObject, FieldType Value)
        {
            setMethodHandler(TargetObject, Value);
        }

        public override string ToString()
        {
            return "Property Invoker : " + getMethodHandler.Method.Name.Substring(4);
        }
    }
}