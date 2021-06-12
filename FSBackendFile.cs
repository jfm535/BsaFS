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
            throw new System.NotImplementedException();
        }

        public int Read(byte[] buffer, long offset)
        {
            var mslice = _archiveFile.GetSpan()[(int) offset..];
            buffer = mslice.ToArray();
            return mslice.Length;
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