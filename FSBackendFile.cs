using System;
using Mutagen.Bethesda.Archives;
using NC.DokanFS;

namespace BsaFS
{
    public class FSBackendFile:IDokanFileContext
    {
        private byte[] _archiveFile;
        public FSBackendFile(IArchiveFile file)
        {
            _archiveFile = file.GetBytes();
        }

        public void Dispose()
        {
            _archiveFile = null;
        }

        public int Read(byte[] buffer, long offset)
        {
            var min = (buffer.Length < _archiveFile.Length ? buffer.Length : _archiveFile.Length);
            Array.Copy(_archiveFile,buffer,min);
            return min;
        }

        public void Write(byte[] buffer, long offset)
        {
            throw new System.NotImplementedException();
        }

        public void Append(byte[] buffer)
        {
            throw new System.NotImplementedException();
        }

        public void Flush()
        {
            throw new System.NotImplementedException();
        }

        public void SetLength(long length)
        {
            throw new System.NotImplementedException();
        }

        public void Lock(long offset, long length)
        {
            throw new System.NotImplementedException();
        }

        public void Unlock(long offset, long length)
        {
            throw new System.NotImplementedException();
        }

        public IDokanDisk Disk { get; }
        public IDokanFile File { get; }
    }
}