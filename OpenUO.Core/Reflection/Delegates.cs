#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   Delegates.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

namespace OpenUO.Core.Reflection
{
    public delegate object FastInvokeHandler(object target, params object[] paramters);

    public delegate ReturnType FastInvokeHandler<ReturnType>(object target, params object[] paramters);

    public delegate FieldType FieldFastGetInvokeHandler<TargetType, FieldType>(TargetType obj);

    public delegate void FieldFastSetInvokeHandler<TargetType, FieldType>(TargetType obj, FieldType value);
}