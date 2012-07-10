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
using System.IO;
using OpenUO.Core;
namespace OpenUO.Ultima.Transactions
{
    public sealed class FileIndexWriteTransaction : Transaction, IIndexedWriteTransaction
    {
        private bool _committed;
        private FileIndex _fileIndex;
        private List<WriteEntry> _transactionWrites;

        public event EventHandler WriteBegin;
        public event EventHandler<ProgressEventArgs> WriteProgress;
        public event EventHandler WriteEnd;

        public FileIndexWriteTransaction(FileIndex fileIndex)
        {
            _fileIndex = fileIndex;
            _transactionWrites = new List<WriteEntry>();
        }

        public void Write(int index, int extra, byte[] buffer)
        {
            _transactionWrites.Add(new WriteEntry()
            {
                Index = index,
                Extra = extra,
                Data = buffer
            });
        }

        public override void Commit()
        {
            if(_committed)
                return;

            bool isOpen = _fileIndex.IsOpen;

            if (!isOpen)
                _fileIndex.Open();

            string workingDirectory = Path.GetDirectoryName(_fileIndex.IndexPath);

            string tmpIdx = Path.Combine(workingDirectory, string.Format("{0}.tmp", _fileIndex.IndexPath));
            string tmpMul = Path.Combine(workingDirectory, string.Format("{0}.tmp", _fileIndex.MulPath));

            _transactionWrites.Sort((l, r) => l.Index.CompareTo(r.Index));

            byte[] buffer = new byte[10 * 1024 * 1024]; // 10mb 

            var writeBegin = WriteBegin;

            if (writeBegin != null)
                writeBegin(this, EventArgs.Empty);

            int totalIndices = _fileIndex.Length;

            if (File.Exists(tmpIdx))
                File.Delete(tmpIdx);

            if (File.Exists(tmpMul))
                File.Delete(tmpMul);

            int lastIndex = 0;

            using (BinaryWriter idxWriter = new BinaryWriter(File.Open(tmpIdx, FileMode.Create)))
            using (BinaryWriter mulWriter = new BinaryWriter(File.Open(tmpMul, FileMode.Create)))
            {
                for (int i = 0; i < _transactionWrites.Count; i++)
                {
                    WriteEntry entry = _transactionWrites[i];

                    for (int j = i; j < entry.Index; j++)
                    {
                        WriteIndex(j, buffer, idxWriter, mulWriter);
                        OnWriteProgress(j, totalIndices);
                    }

                    idxWriter.Write((int)mulWriter.BaseStream.Position);
                    idxWriter.Write(entry.Data.Length);
                    idxWriter.Write(entry.Extra);

                    mulWriter.Write(entry.Data, 0, entry.Data.Length);
                    OnWriteProgress(i, totalIndices);

                    lastIndex = entry.Index;
                }

                for (int i = lastIndex + 1; i < _fileIndex.Length; i++)
                {
                    WriteIndex(i, buffer, idxWriter, mulWriter);
                    OnWriteProgress(i, totalIndices);
                }
            }

            OnWriteProgress(totalIndices, totalIndices);

            buffer = null;
            _fileIndex.Close();

            DateTime now = DateTime.Now;

            string bakIdx = Path.Combine(workingDirectory, string.Format("{0}.{1:MM-dd-yy HH-mm-ss}.bak", _fileIndex.IndexPath, now));
            string bakMul = Path.Combine(workingDirectory, string.Format("{0}.{1:MM-dd-yy HH-mm-ss}.bak", _fileIndex.MulPath, now));

            int count = 1;
            while (File.Exists(bakIdx))
                bakIdx = Path.Combine(workingDirectory, string.Format("{0}.{1:MM-dd-yy HH-mm-ss} {2}.bak", _fileIndex.IndexPath, now, count++));

            count = 1;
            while (File.Exists(bakMul))
                bakMul = Path.Combine(workingDirectory, string.Format("{0}.{1:MM-dd-yy HH-mm-ss} {2}.bak", _fileIndex.MulPath, now, count++));

            File.Copy(_fileIndex.IndexPath, bakIdx);
            File.Copy(_fileIndex.MulPath, bakMul);

            File.Copy(tmpIdx, _fileIndex.IndexPath, true);
            File.Copy(tmpMul, _fileIndex.MulPath, true);

            File.Delete(tmpIdx);
            File.Delete(tmpMul);

            var writeEnd = WriteEnd;

            if (writeEnd != null)
                writeEnd(this, EventArgs.Empty);

            if (isOpen)
                _fileIndex.Open();

            _committed = true;
        }

        private void WriteIndex(int j, byte[] buffer, BinaryWriter idxWriter, BinaryWriter mulWriter)
        {
            int length, extra;
            Stream stream = _fileIndex.Seek(j, out length, out extra);

            if (stream == null || length == -1)
            {
                idxWriter.Write(-1); //lookup
                idxWriter.Write(-1); //length
                idxWriter.Write(-1); //extra
            }
            else
            {
                BinaryReader reader = new BinaryReader(stream);
                reader.Read(buffer, 0, length);

                idxWriter.Write((int)mulWriter.BaseStream.Position);
                idxWriter.Write(length);
                idxWriter.Write(extra);

                mulWriter.Write(buffer, 0, length);
            }
        }

        private void OnWriteProgress(int position, int totalIndices)
        {
            var handler = WriteProgress;

            if (handler != null)
                handler(this, new ProgressEventArgs(position, totalIndices));
        }

        public override void Rollback()
        {
            _transactionWrites.Clear();
        }

        public override void Dispose()
        {
            if (_committed)
                return;

            Commit();
        }

        private class WriteEntry
        {
            public int Extra;
            public int Index { get; set; }
            public byte[] Data { get; set; }
        }
    }
}
