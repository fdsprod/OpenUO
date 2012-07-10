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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using OpenUO.Core.Diagnostics;

namespace OpenUO.Ultima
{
    [Serializable]
    [XmlRoot("install")]
    [TypeConverter(typeof(InstallLocationTypeConverter))]
    public class InstallLocation
    {
        private static readonly Version _convertedToUOPVersion = new Version("7.0.24.0");

        private Version _version;
         
        [XmlIgnore]
        public Version Version
        {
            get
            {
                if (_version == null)
                {
                    string clientExe = GetPath("client.exe");

                    if (File.Exists(clientExe))
                    {
                        var fileVersionInfo = GetClientVersion();
                        _version = new Version(fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart, fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
                    }
                    else
                    {
                        _version = new Version("0.0.0.0");
                    }
                }

                return _version;
            }
        }

        [XmlAttribute("directory")]
        public string Directory 
        { 
            get;
            private set;
        }

        public bool IsUOPFormat
        {
            get { return Version >= _convertedToUOPVersion; }
        }

        public InstallLocation(string directory)
        {
            Directory = directory;
        }

        public FileIndex CreateFileIndex(string uopFile)
        {
            uopFile = GetPath(uopFile);

            FileIndex fileIndex = new FileIndex(uopFile);

            if (!fileIndex.FilesExist)
            {
                Tracer.Warn(
                    "FileIndex was created but {0} was missing from {1}",
                    Path.GetFileName(uopFile), Directory);
            }

            return fileIndex;
        }

        public FileIndex CreateFileIndex(string idxFile, string mulFile)
        {
            idxFile = GetPath(idxFile);
            mulFile = GetPath(mulFile);

            FileIndex fileIndex = new FileIndex(idxFile, mulFile);

            if (!fileIndex.FilesExist)
            {
                Tracer.Warn(
                    "FileIndex was created but 1 or more files do not exist.  Either {0} or {1} were missing from {2}", 
                    Path.GetFileName(idxFile), 
                    Path.GetFileName(mulFile), Directory);
            }

            return fileIndex;
        }

        public StreamWriter CreateText(string filename)
        {
            filename = GetPath(filename);

            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);

            return File.CreateText(filename);
        }

        public FileStream Create(string filename)
        {
            filename = GetPath(filename);

            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);

            return File.Create(filename);
        }

        public FileStream OpenWrite(string filename)
        {
            filename = GetPath(filename);

            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);

            return File.OpenWrite(filename);
        }

        public FileStream OpenRead(string filename)
        {
            filename = GetPath(filename);

            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);

            return File.OpenRead(filename);
        }

        public string GetPath(string filename, params object[] args)
        {
            string path = Path.Combine(Directory, string.Format(filename, args));

            if (!File.Exists(path))
                Tracer.Warn("{0} does not exists.", path);

            return path;
        }

        private FileVersionInfo GetClientVersion()
        {
            string clientExe = GetPath("client.exe");

            if (!File.Exists(clientExe))
                throw new FileNotFoundException("client.exe");

            return FileVersionInfo.GetVersionInfo(clientExe);
        }

        public void CalculateLoginKeys(out uint key1, out uint key2)
        {
            FileVersionInfo info = GetClientVersion();
            
            uint major = (uint)info.ProductMajorPart;
            uint minor = (uint)info.ProductMinorPart;
            uint build = (uint)info.ProductBuildPart;

            ClientUtility.CalculateLoginKeys(major, minor, 0, build, out key1, out key2);
        }

        public static implicit operator string(InstallLocation a)
        {
            return a.Directory;
        }

        public static implicit operator InstallLocation(string a)
        {
            return new InstallLocation(a);
        }

        public override string ToString()
        {
            return Directory;
        }
    }

    public class InstallLocationTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value != null && value is string)
                return new InstallLocation((string)value);

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return ((InstallLocation)value).Directory;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
