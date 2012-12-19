#region License Header

// /***************************************************************************
//  *   Copyright (c) 2011 OpenUO Software Team.
//  *   All Right Reserved.
//  *
//  *   ClilocData.cs
//  *
//  *   This program is free software; you can redistribute it and/or modify
//  *   it under the terms of the GNU General Public License as published by
//  *   the Free Software Foundation; either version 3 of the License, or
//  *   (at your option) any later version.
//  ***************************************************************************/

#endregion

#region Usings

using System;
using System.IO;
using System.Text;
using OpenUO.Core.Diagnostics;

#endregion

namespace OpenUO.Ultima
{
    public class LocalizationEntry
    {
        public LocalizationEntry(ClientLocalizationLanguage lng, int index, long offset, long length)
        {
            Language = lng;
            Index = index;
            Offset = offset;
            Length = length;
        }

        public ClientLocalizationLanguage Language
        {
            get;
            protected set;
        }

        public int Index
        {
            get;
            protected set;
        }

        public long Offset
        {
            get;
            protected set;
        }

        public long Length
        {
            get;
            protected set;
        }

        protected ClilocInfo Info
        {
            get;
            set;
        }

        public virtual void Clear()
        {
            Info = null;
        }

        public ClilocInfo Lookup(BinaryReader bin)
        {
            bin.BaseStream.Seek(Offset, SeekOrigin.Begin);

            byte[] data = new byte[Length];

            for (long i = 0; i < data.Length; i++)
            {
                data[i] = bin.ReadByte();
            }

            Info = new ClilocInfo(Language, Index, Encoding.UTF8.GetString(data));
            data = null;

            return Info;
        }

        public ClilocInfo Lookup(FileInfo file, bool forceUpdate = false)
        {
            try
            {
                if (Info != null)
                {
                    if (!forceUpdate)
                    {
                        return Info;
                    }
                }

                using (BinaryReader reader = new BinaryReader(file.OpenRead(), Encoding.UTF8))
                {
                    Lookup(reader);
                }
            }
            catch (Exception e)
            {
                Tracer.Error(e);
            }

            return Info;
        }
    }
}