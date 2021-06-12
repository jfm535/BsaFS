using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DokanNet;
using Mutagen.Bethesda.Archives;
using NC.DokanFS;
using NC.DokanFS.Specialized;
using FileAccess = System.IO.FileAccess;

namespace BsaFS
{
    public class FSBackendDisk: IDokanDisk
    {
        private IArchiveReader myArchiveReader;
        public FSBackendDisk(IArchiveReader testing)
        {
            myArchiveReader = testing;
        }

        public string GetPath(string osPath)
        {
            return osPath;
        }

        public IDokanFileContext CreateFileContext(string path, FileMode mode, FileAccess access, FileShare share = FileShare.None,
            FileOptions options = FileOptions.None)
        {
            var dokanFileContext = new FSBackendFile(myArchiveReader.Files.Single(mfile => mfile.Path == path));
            return dokanFileContext;
        }

        public void CreateDirectory(string path)
        {
            throw new IOException();
        }

        public bool IsDirectory(string path)
        {
            if (path == "\\") return true;
           return myArchiveReader.TryGetFolder(path, out var mfolder);
        }

        public bool DirectoryExists(string path)
        {
            if (path == "\\")
            {
                return true;
            }
            var tryGetFolder = myArchiveReader.TryGetFolder(path, out var mfolder);
            return tryGetFolder;
        }

        public bool IsDirectoryEmpty(string path)
        {
            throw new NotImplementedException();
        }

        public void DeleteDirectory(string path)
        {
            throw new IOException();
        }

        public bool DirectoryCanBeDeleted(string path)
        {
            return false;
        }

        public void DeleteFile(string path)
        {
            throw new IOException();
        }

        public bool FileExists(string path)
        {
            return myArchiveReader.Files.Any(mfile => mfile.Path == path);
        }

        public IList<FileInformation> FindFiles(string directory, string searchPattern)
        {
            if (directory == "\\")
            {
                var listOfFoldersAndFiles = new HashSet<FileInformation>();
                var filePaths = myArchiveReader.Files.Select(mfile => mfile.Path);
                foreach (var filePath in filePaths)
                {
                    if (filePath.Contains("\\"))
                    {
                        listOfFoldersAndFiles.Add(new FileInformation
                        {
                            Attributes = FileAttributes.Directory,
                            FileName = filePath.Split("\\")[0]
                        });
                    }
                    else
                    {
                        listOfFoldersAndFiles.Add(new FileInformation
                        {
                            Attributes = FileAttributes.Normal,
                            FileName = filePath
                        });
                    }
                }
                return listOfFoldersAndFiles.ToList();
            }
            throw new NotImplementedException();
        }

        public bool GetFileInfo(string path, out FileInformation fi)
        {
            if (path == "\\")
            {
                fi = new FileInformation {Attributes = FileAttributes.Directory};
                return true;
            }
            throw new NotImplementedException();
        }

        public void GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes)
        {
            freeBytesAvailable = 0;
            totalNumberOfBytes = myArchiveReader.Files.Sum(mfile => mfile.Size);
            totalNumberOfFreeBytes = 0;
        }

        public void MoveDirectory(string oldPath, string newPath)
        {
            throw new IOException();
        }

        public void MoveFile(string oldPath, string newPath)
        {
            throw new IOException();
        }

        public void SetFileAttribute(string path, FileAttributes attr)
        {
            throw new IOException();
        }

        public void SetFileTime(string path, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime)
        {
            throw new IOException();
        }

        public IDokanFile Touch(string path, FileAttributes attributes)
        {
            throw new IOException();
        }

        public void UpdateFileInformation(IDokanFile file)
        {
            throw new IOException();
        }

        public void UpdateDirectoryInformation(IDokanDirectory dir)
        {
            throw new IOException();
        }

        public string Id { get; }
        public string VolumeLabel { get; }
        public string FileSystemName { get; }
        public FileSystemFeatures FileSystemFeatures => FileSystemFeatures.ReadOnlyVolume;
        public uint MaximumComponentLength { get; }
    }
}