using Polo.Abstractions.Entities;

namespace Polo.Abstractions.DataProviders
{
    public interface ISqLiteProvider : IDisposable
    {
        public FsivFolder GetFsivFolderByFolderName(string folderFullPath);
        public int AddFolder(FsivFolder fsivFolder);
        public FsivFile GetFsivFileByFileNameAndParentFolderId(string fileName, int folderId);
        public int AddFile(FsivFile fsivFile);
        public bool UpdateFile(FsivFile fsivFile);
    }
}