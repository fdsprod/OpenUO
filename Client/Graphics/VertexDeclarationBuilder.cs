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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
	internal class VertexDeclarationBuilder
	{
		internal static VertexDeclarationBuilder Instance;

		static VertexDeclarationBuilder()
		{
			_formatMapping = new Dictionary<Type, DeclarationType>();
			_formatMappingSize = new Dictionary<DeclarationType, int>();
			_usageMapping = new Dictionary<string, DeclarationUsage>();

			_formatMapping.Add(typeof(Color), DeclarationType.Color);
			_formatMapping.Add(typeof(Half2), DeclarationType.HalfTwo);
			_formatMapping.Add(typeof(Half4), DeclarationType.HalfFour);
			_formatMapping.Add(typeof(float), DeclarationType.Float1);
			_formatMapping.Add(typeof(Vector2), DeclarationType.Float2);
			_formatMapping.Add(typeof(Vector3), DeclarationType.Float3);
			_formatMapping.Add(typeof(Vector4), DeclarationType.Float4);

			foreach (KeyValuePair<Type, DeclarationType> kvp in _formatMapping)
			{
				_formatMappingSize.Add(kvp.Value, Marshal.SizeOf(kvp.Key));
			}

			FieldInfo[] enums = typeof(DeclarationUsage).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

			foreach (FieldInfo field in enums)
			{
				DeclarationUsage usage = (DeclarationUsage)field.GetValue(null);
				_usageMapping.Add(usage.ToString().ToLower(), usage);
			}

			_usageMapping.Add("norm", DeclarationUsage.Normal);
			_usageMapping.Add("pos", DeclarationUsage.Position);
			_usageMapping.Add("binorm", DeclarationUsage.Binormal);
			_usageMapping.Add("colour", DeclarationUsage.Color);
			_usageMapping.Add("diffuse", DeclarationUsage.Color);
			_usageMapping.Add("col", DeclarationUsage.Color);
			_usageMapping.Add("size", DeclarationUsage.PointSize);
			_usageMapping.Add("psize", DeclarationUsage.PointSize);
			_usageMapping.Add("tex", DeclarationUsage.TextureCoordinate);
			_usageMapping.Add("texture", DeclarationUsage.TextureCoordinate);
			_usageMapping.Add("texcoord", DeclarationUsage.TextureCoordinate);
			_usageMapping.Add("texcoordinate", DeclarationUsage.TextureCoordinate);
		}

		private static DeclarationHash _hashingDecl;
		private static Dictionary<Type, DeclarationType> _formatMapping;
		private static Dictionary<DeclarationType, int> _formatMappingSize;
		private static Dictionary<string, DeclarationUsage> _usageMapping;

		private short _typeIndex;
		private DeviceContext _context;
		private Dictionary<Type, short> _typeHash;
		private Dictionary<Type, VertexElement[]> _declarationMapping;
		private Dictionary<DeclarationType, bool> _vertexFormatSupported;
		private VertexDeclaration[] _declarations = new VertexDeclaration[128];
		private Dictionary<DeclarationHash, VertexDeclaration> _declarationHash = new Dictionary<DeclarationHash, VertexDeclaration>();
		private Dictionary<ElementHash, VertexDeclaration> _elementHash = new Dictionary<ElementHash, VertexDeclaration>();

		public VertexDeclarationBuilder(DeviceContext context)
		{
			_context = context;
			_declarationMapping = new Dictionary<Type, VertexElement[]>();
			_typeHash = new Dictionary<Type, short>();
			_hashingDecl = new DeclarationHash(typeof(Vector3), _typeHash, ref _typeIndex);//static to keep from GC messing
			Instance = this;

			BuildFormatList();
		}

		public VertexDeclaration GetDeclaration<T>()
		{
			int index = Ident.TypeIndex<T>();

			while (index > _declarations.Length)
				Array.Resize(ref _declarations, _declarations.Length * 2);

			if (_declarations[index] != null)
				return _declarations[index];

			VertexElement[] elements = GetDeclaration(typeof(T));

			if (_context == null)
				return null;

			VertexDeclaration declaration;

			lock (_hashingDecl)
			{
				_hashingDecl.SetFrom(typeof(T), _typeHash, ref _typeIndex);
				if (_declarationHash.TryGetValue(_hashingDecl, out declaration))
				{
					_declarations[index] = declaration;
					return declaration;
				}
			}

			for (int i = 0; i < elements.Length; i++)
				ValidateFormat(typeof(T), elements[i].Type);

			ElementHash ehash = new ElementHash(elements);
			_elementHash.TryGetValue(ehash, out declaration);

			if (declaration == null)
				declaration = new VertexDeclaration(_context, elements);

			_declarations[index] = declaration;
			_declarationHash.Add(new DeclarationHash(typeof(T), _typeHash, ref _typeIndex), declaration);

			if (_elementHash.ContainsKey(ehash) == false)
				_elementHash.Add(ehash, declaration);

			return declaration;
		}

		public VertexDeclaration GetDeclaration(Type[] streamTypes, IVertices[] buffers)
		{
			VertexDeclaration declaration;

			lock (_hashingDecl)
			{
				_hashingDecl.SetFrom(streamTypes, _typeHash, ref _typeIndex);
				if (_declarationHash.TryGetValue(_hashingDecl, out declaration))
					return declaration;
			}

			VertexElement[][] mappings = new VertexElement[streamTypes.Length][];

			int i = 0;
			for (i = 0; i < streamTypes.Length; i++)
			{
				//buffer provides the vertex elements itself
				if (buffers[i] is IDeviceVertexBuffer &&
					(buffers[i] as IDeviceVertexBuffer).IsImplementationUserSpecifiedVertexElements(out mappings[i]))
					continue;

				mappings[i] = GetDeclaration(streamTypes[i]);
			}

			List<VertexElement> mapping = new List<VertexElement>();

			short stream = 0;
			foreach (VertexElement[] map in mappings)
			{
				foreach (VertexElement el in map)
				{
					bool skip = false;

					foreach (VertexElement check in mapping)
					{
						if (el.UsageIndex == check.UsageIndex &&
							el.Usage == check.Usage)
						{
							skip = true;
							break;
						}
					}

					if (skip)
						continue;

					VertexElement v = el;
					v.Stream = stream;
					mapping.Add(v);
				}
				stream++;
			}

			VertexElement[] elements = mapping.ToArray();

			ElementHash ehash = new ElementHash(elements);
			_elementHash.TryGetValue(ehash, out declaration);

			if (declaration == null)
				declaration = new VertexDeclaration(_context, elements);
			_declarationHash.Add(new DeclarationHash(streamTypes, _typeHash, ref _typeIndex), declaration);

			if (_elementHash.ContainsKey(ehash) == false)
				_elementHash.Add(ehash, declaration);

			return declaration;
		}

		public VertexDeclaration GetDeclaration(VertexElement[] elements)
		{
			VertexDeclaration declaration;
			ElementHash ehash = new ElementHash(elements);

			if (_elementHash.TryGetValue(ehash, out declaration))
				return declaration;

			declaration = new VertexDeclaration(_context, elements);
			_elementHash.Add(ehash, declaration);

			return declaration;
		}

		public VertexElement[] GetDeclaration(Type type)
		{
			lock (_declarationMapping)
			{
				VertexElement[] mapping;
				if (_declarationMapping.TryGetValue(type, out mapping))
					return mapping;

				if (type == typeof(Vector3))//special case
					mapping = new VertexElement[] { new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0) };
				if (type == typeof(Vector4))
					mapping = new VertexElement[] { new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0) };

				if (mapping == null)
				{
					List<VertexElement> elements = new List<VertexElement>();
					int offset = 0;

					if (type.IsValueType == false)
						throw new ArgumentException("Type " + type.Name + " is a not a ValueType (struct)");

					foreach (FieldInfo f in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
					{
						if (!f.ReflectedType.IsValueType)
							throw new ArgumentException("Field " + type.Name + "." + f.Name + " is a not a ValueType (struct)");

						int size = Marshal.SizeOf(f.FieldType);

						bool attribSet = false;

						foreach (object o in f.GetCustomAttributes(true))
						{
							if (o is VertexElementAttribute)
							{
								VertexElementAttribute att = (VertexElementAttribute)o;
								DeclarationType format = att.Type;

								if (format == DeclarationType.Unused)
									format = DetermineFormat(f);
								else
								{
									int formatSize;

									if (!_formatMappingSize.TryGetValue(format, out formatSize))
										throw new ArgumentException(string.Format("Invlaid DeclarationType ({0}) specified in VertexElementAttribute for {1}.{2}", format, type.FullName, f.Name));

									if (formatSize != Marshal.SizeOf(f.FieldType))
										throw new ArgumentException(string.Format("DeclarationType size mismatch in {4}.{5}, {0} requires a size of {1}, specified type {2} has size {3}", format, formatSize, f.FieldType.FullName, Marshal.SizeOf(f.FieldType), type.FullName, f.Name));
								}

								elements.Add(new VertexElement(0, (short)offset, format, DeclarationMethod.Default, att.Usage, (byte)att.Index));
								attribSet = true;
								break;
							}
						}

						if (!attribSet)
						{
							DeclarationType format = DetermineFormat(f);
							int index;
							DeclarationUsage usage = DetermineUsage(elements, f, out index);

							elements.Add(new VertexElement(0, (short)offset, format, DeclarationMethod.Default, usage, (byte)index));
						}

						offset += size;
					}

					elements.Add(VertexElement.VertexDeclarationEnd);

					mapping = elements.ToArray();
				}

				_declarationMapping.Add(type, mapping);

				return mapping;
			}
		}

		public static DeclarationType DetermineFormat(Type type)
		{
			return _formatMapping[type];
		}

		internal static int SizeOfFormatType(DeclarationType element)
		{
			return _formatMappingSize[element];
		}

		private void BuildFormatList()
		{
			_vertexFormatSupported = new Dictionary<DeclarationType, bool>();

			Capabilities caps = _context.Capabilities;

			System.Reflection.FieldInfo[] enums = typeof(DeclarationType).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

			for (int i = 0; i < enums.Length; i++)
			{
				DeclarationType format = (DeclarationType)enums[i].GetValue(null);
				_vertexFormatSupported.Add(format, true);
			}

			_vertexFormatSupported[DeclarationType.Ubyte4] = caps.DeclarationTypes.HasFlag(DeclarationTypeCaps.UByte4);
			_vertexFormatSupported[DeclarationType.HalfTwo] = caps.DeclarationTypes.HasFlag(DeclarationTypeCaps.HalfTwo);
			_vertexFormatSupported[DeclarationType.HalfFour] = caps.DeclarationTypes.HasFlag(DeclarationTypeCaps.HalfFour);
			_vertexFormatSupported[DeclarationType.Dec3N] = caps.DeclarationTypes.HasFlag(DeclarationTypeCaps.Dec3N);
			_vertexFormatSupported[DeclarationType.Short2N] = caps.DeclarationTypes.HasFlag(DeclarationTypeCaps.Short2N);
			_vertexFormatSupported[DeclarationType.Short4N] = caps.DeclarationTypes.HasFlag(DeclarationTypeCaps.Short4N);
			_vertexFormatSupported[DeclarationType.UShort2N] = caps.DeclarationTypes.HasFlag(DeclarationTypeCaps.UShort2N);
			_vertexFormatSupported[DeclarationType.UByte4N] = caps.DeclarationTypes.HasFlag(DeclarationTypeCaps.UByte4N);
			_vertexFormatSupported[DeclarationType.UShort4N] = caps.DeclarationTypes.HasFlag(DeclarationTypeCaps.UShort4N);
			_vertexFormatSupported[DeclarationType.UDec3] = caps.DeclarationTypes.HasFlag(DeclarationTypeCaps.UDec3);
		}

		private void ValidateFormat(Type type, DeclarationType format)
		{
			bool supported = true;

			if (_vertexFormatSupported != null &&
				_vertexFormatSupported.TryGetValue(format, out supported) && !supported)
				throw new InvalidOperationException(string.Format("Graphics device does not support vertex element format \'{0}\', as used in vertex structure \'{1}\'", format, type.FullName));
		}

		private static DeclarationType DetermineFormat(FieldInfo field)
		{
			DeclarationType format;

			if (_formatMapping.TryGetValue(field.FieldType, out format))
				return format;

			throw new ArgumentException("Field (" + field.FieldType.Name + ") " + field.DeclaringType.Name + "." + field.Name + " value mapping cannot be determined. Either set the DeclarationType with a [VertexElement()] attribute, or change the declaration to a supported type.");
		}

		private static DeclarationUsage DetermineUsage(List<VertexElement> elements, FieldInfo field, out int index)
		{
			string name = field.Name.ToLower().Replace("_", "");
			string number = "";

			for (int i = name.Length - 1; i >= 0; i--)
			{
				if (char.IsDigit(name[i]))
					number = name[i] + number;
				else
					break;
			}

			name = name.Substring(0, name.Length - number.Length);

			index = 0;

			if (number.Length > 0)
				index = int.Parse(number, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

			DeclarationUsage usage;

			if (_usageMapping.TryGetValue(name, out usage))
			{
				while (true)
				{
					bool valid = true;

					foreach (VertexElement el in elements)
					{
						if (el.Usage == usage && el.UsageIndex == index)
						{
							index++;
							valid = false;
						}
					}

					if (valid)
						break;
				}

				return usage;
			}
			throw new ArgumentException("Field (" + field.FieldType.Name + ") " + field.DeclaringType.Name + "." + field.Name + " usage mapping cannot be determined. Either set the DeclarationUsage with a [VertexElement()] attribute, or change the field name to a known usage type.");
		}

		private static class Ident
		{
			static object _sync = new object();
			static volatile int _index = 0;

			static int Index
			{
				get
				{
					lock (_sync)
						return _index++;
				}
			}

			public static int TypeIndex<T>()
			{
				return Type<T>.id;
			}

			private class Type<T>
			{
				public static int id = Index;
			}
		}
#if DEBUG
		private struct VertexUsage
		{
			public DeclarationUsage Usage;
			public int Index;

			public VertexUsage(DeclarationUsage usage = DeclarationUsage.Position, int index = 0)
			{
				Usage = usage;
				Index = index;
			}
		}
#endif
		private class DeclarationHash : IComparable<DeclarationHash>
		{
			private List<short> _hash = new List<short>();
			private short _hashCode;

			public DeclarationHash(Type type, Dictionary<Type, short> typeHash, ref short typeIndex)
			{
				SetFrom(type, typeHash, ref typeIndex);
			}

			public void SetFrom(Type type, Dictionary<Type, short> typeHash, ref short typeIndex)
			{
				_hash.Clear();
				short hash;
				_hashCode = 0;

				lock (typeHash)
				{
					if (!typeHash.TryGetValue(type, out hash))
					{
						hash = typeIndex++;
						typeHash.Add(type, hash);
						_hashCode ^= hash;
					}
				}
				_hash.Add(hash);
			}

			public DeclarationHash(Type[] types, Dictionary<Type, short> typeHash, ref short typeIndex)
			{
				SetFrom(types, typeHash, ref typeIndex);
			}

			public void SetFrom(Type[] types, Dictionary<Type, short> typeHash, ref short typeIndex)
			{
				_hash.Clear();
				_hashCode = 0;

				lock (typeHash)
				{
					for (int i = 0; i < types.Length; i++)
					{
						short hash;
						if (!typeHash.TryGetValue(types[i], out hash))
						{
							hash = typeIndex++;
							typeHash.Add(types[i], hash);
						}
						_hash.Add(hash);
						_hashCode ^= hash;
					}
				}
			}

			public override int GetHashCode()
			{
				return _hashCode;
			}

			public int CompareTo(DeclarationHash other)
			{
				int cmp = other._hash.Count.CompareTo(_hash.Count);

				if (cmp != 0)
					return cmp;

				for (int i = 0; i < _hash.Count; i++)
				{
					cmp = _hash[i].CompareTo(other._hash[i]);
					if (cmp != 0)
						return cmp;
				}

				return 0;
			}

			public override bool Equals(object obj)
			{
				if (obj is IComparable<DeclarationHash>)
					return ((IComparable<DeclarationHash>)obj).CompareTo(this) == 0;

				return base.Equals(obj);
			}
		}

		private class ElementHash : IComparable<ElementHash>
		{
			private int _hash;
			private VertexElement[] _elements;

			public ElementHash(VertexElement[] elements)
			{
				_elements = elements;

				for (int i = 0; i < elements.Length; i++)
				{
					_hash ^= ((int)elements[i].Usage);
					_hash ^= ((int)elements[i].Method) << 3;
					_hash ^= ((int)elements[i].Type) << 6;
					_hash ^= ((int)elements[i].UsageIndex) << 8;
					_hash ^= ((int)elements[i].Stream) << 16;
					_hash ^= (int)elements[i].Offset;
					_hash ^= i;
				}
			}

			public override bool Equals(object obj)
			{
				if (obj is ElementHash)
					return CompareTo(obj as ElementHash) == 0;

				return base.Equals(obj);
			}
			public override int GetHashCode()
			{
				return _hash;
			}

			public int CompareTo(ElementHash othervh)
			{
				VertexElement[] other = othervh._elements;

				if (_elements.Length != other.Length)
					return _elements.Length - other.Length;

				for (int i = 0; i < _elements.Length; i++)
				{
					if (_elements[i].Offset != other[i].Offset)
						return (int)_elements[i].Offset - (int)other[i].Offset;

					if (_elements[i].Stream != other[i].Stream)
						return (int)_elements[i].Stream - (int)other[i].Stream;

					if (_elements[i].UsageIndex != other[i].UsageIndex)
						return (int)_elements[i].UsageIndex - (int)other[i].UsageIndex;

					if (_elements[i].Type != other[i].Type)
						return (int)_elements[i].Type - (int)other[i].Type;

					if (_elements[i].Method != other[i].Method)
						return (int)_elements[i].Method - (int)other[i].Method;

					if (_elements[i].Usage != other[i].Usage)
						return (int)_elements[i].Usage - (int)other[i].Usage;
				}

				return 0;
			}
		}
	}
}
