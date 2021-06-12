using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DokanNet;
using Mutagen.Bethesda.Archives;
using NC.DokanFS;
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
            return osPath.TrimStart('\\');
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
            if (path == "") return true;
            var isDirectory = myArchiveReader.Files.All(mfile => mfile.Path != path);
            return isDirectory;
        }

        public bool DirectoryExists(string path)
        {
            if (path == "") return true;
            var tryGetFolder =
                myArchiveReader.Files.Any(mfile => mfile.Path.StartsWith(path) && !mfile.Path.Equals(path));
            return tryGetFolder;
        }

        public bool IsDirectoryEmpty(string path)
        {
            return false;
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
            var filelist = new List<FileInformation>();
            var mdir = directory.TrimStart('\\');
            var Patterntrim = searchPattern.TrimStart('\\');
            var JoinedSearch = Path.Join(mdir, Patterntrim);
            if (mdir == "")
            {
                return FindFilesRootDir();
            }
            //Handle Joined
            if (IsDirectory(JoinedSearch) && !JoinedSearch.EndsWith("*"))
            {
                var foundFiles = myArchiveReader.Files.Select(mfile => mfile.Path)
                    .Where(mdir2 => mdir2.StartsWith(JoinedSearch));
                var StrippedFiles = foundFiles.Select(mdir2 => mdir2.Replace(JoinedSearch,"").TrimStart('\\'));
                var DirectoryContents = StrippedFiles.Select(mdir2 => mdir2.Split('\\')[0]).Distinct();
                foreach (var directoryContent in DirectoryContents)
                {
                    var fullentry = Path.Join(JoinedSearch, directoryContent);
                    if (FileExists(fullentry))
                    {
                        var mfilea = myArchiveReader.Files.Single(mfile => mfile.Path == fullentry);
                        filelist.Add(new FileInformation
                        {
                            Attributes = FileAttributes.Normal,
                            FileName = directoryContent,
                            Length = mfilea.Size
                        });
                    }
                    else
                    {
                        filelist.Add(new FileInformation
                        {
                            Attributes = FileAttributes.Directory,
                            FileName = directoryContent
                        });
                    }
                }
            }
            if (IsDirectory(mdir) && !FileExists(mdir))
            {
                var foundFiles = myArchiveReader.Files.Select(mfile => mfile.Path)
                    .Where(mdir2 => mdir2.StartsWith(mdir));
                    var StrippedFiles = foundFiles.Select(mdir2 => mdir2.Replace(mdir,"").TrimStart('\\'));
                var DirectoryContents = StrippedFiles.Select(mdir2 => mdir2.Split('\\')[0]).Distinct();
                foreach (var directoryContent in DirectoryContents)
                {
                    var fullentry = Path.Join(mdir, directoryContent);
                    if (FileExists(fullentry))
                    {
                        var mfilea = myArchiveReader.Files.Single(mfile => mfile.Path == fullentry);
                        filelist.Add(new FileInformation
                        {
                            Attributes = FileAttributes.Normal,
                            FileName = directoryContent,
                            Length = mfilea.Size
                        });
                    }
                    else
                    {
                        filelist.Add(new FileInformation
                        {
                            Attributes = FileAttributes.Directory,
                            FileName = directoryContent
                        });
                    }
                }
            }

            if (FileExists(mdir))
            {
                var file = myArchiveReader.Files.Single(mfile => mfile.Path == mdir);
                filelist.Add(new FileInformation
                {
                    Attributes = FileAttributes.Normal,
                    FileName = System.IO.Path.GetFileName(mdir),
                    Length = file.Size
                });
            }
            return filelist;
        }

        private IList<FileInformation> FindFilesRootDir()
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

        public bool GetFileInfo(string path, out FileInformation fi)
        {
            fi = new FileInformation();
            //handle directory
            if (path == "" || IsDirectory(path))
            {
                fi.Attributes = FileAttributes.Directory;
                return true;
            }
            //handle file doesnt exist
            if (!FileExists(path)) return false;
            
            //handle file
            var thefile = myArchiveReader.Files.Single(mfile => mfile.Path == path);
            fi.Attributes = FileAttributes.Normal;
            fi.Length = thefile.Size;
            fi.FileName = Path.GetFileName(path);
            return true;

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
        public uint MaximumComponentLength => 256;
    }
}