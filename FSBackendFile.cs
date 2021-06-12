using System;
using Mutagen.Bethesda.Archives;
using NC.DokanFS;

namespace BsaFS
{
    public class FSBackendFile:IDokanFileContext
    {
        private IArchiveFile _archiveFile;
        public FSBackendFile(IArchiveFile file)
        {
            _archiveFile = file;
        }

        public void Dispose()
        {
            return;
        }

        public int Read(byte[] buffer, long offset)
        {
            var mslice = _archiveFile.GetBytes();
            var min = (buffer.Length < mslice.Length ? buffer.Length : mslice.Length);
            Array.Copy(mslice,buffer,min);
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