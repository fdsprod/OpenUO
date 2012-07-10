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
using SharpDX.Direct3D9;

namespace Client.Graphics
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class VertexElementAttribute : Attribute
	{
		private DeclarationUsage _usage;
		private DeclarationType _format;
		private byte _index;

		public static int SizeOfFormatType(DeclarationType type)
		{
			return VertexDeclarationBuilder.SizeOfFormatType(type);
		}

		public static int CalculateVertexStride(VertexElement[] elements)
		{

			int stride = 0;
			for (int i = 0; i < elements.Length; i++)
				stride = Math.Max(stride, elements[i].Offset + VertexElementAttribute.SizeOfFormatType(elements[i].Type));
			return stride;
		}

		public static bool ExtractUsage(VertexElement[] elements, DeclarationUsage usage, int index, out DeclarationType format, out int offset)
		{
			format = (DeclarationType)0;
			offset = 0;

			for (int i = 0; i < elements.Length; i++)
			{
				if (elements[i].Usage == usage &&
					elements[i].UsageIndex == index)
				{
					format = elements[i].Type;
					offset = elements[i].Offset;
					return true;
				}
			}
			return false;
		}

		public byte Index
		{
			get { return _index; }
		}

		public DeclarationType Type
		{
			get { return _format; }
		}

		public DeclarationUsage Usage
		{
			get { return _usage; }
		}

		public VertexElementAttribute(DeclarationUsage usage, DeclarationType format)
		{
			_usage = usage;
			_format = format;
		}

        public VertexElementAttribute(DeclarationUsage usage)
        {
            _usage = usage;
            _format = DeclarationType.Unused;
        }

		public VertexElementAttribute(DeclarationUsage usage, byte usageIndex)
		{
			_usage = usage;
			_format = DeclarationType.Unused;
			_index = usageIndex;
		}

		public VertexElementAttribute(DeclarationUsage usage, DeclarationType format, byte usageIndex)
		{
			_usage = usage;
			_format = format;
			_index = usageIndex;
		}
	}
}
