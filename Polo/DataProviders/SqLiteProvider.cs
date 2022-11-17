using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using Polo.Abstractions.DataProviders;
using Polo.Abstractions.Entities;

namespace Polo.DataProviders
{
    public class SqLiteProvider : ISqLiteProvider
    {
        private readonly SqliteConnection _sqliteConnection;

        public SqLiteProvider(string databaseFilePath)
        {
            _sqliteConnection = new SqliteConnection($"Data Source={databaseFilePath}");
            _sqliteConnection.Open();
        }

        public void Dispose()
        {
            _sqliteConnection.Dispose();
        }

        public FsivFolder GetFsivFolderByFolderName(string folderFullPath)
        {
            const string sql = "select folderID, folderName from FolderList where folderName = @FolderName";
            var fsivFolder = _sqliteConnection.QueryFirstOrDefault<FsivFolder>(sql, new { FolderName = folderFullPath });
            return fsivFolder;
        }

        public int AddFolder(FsivFolder fsivFolder)
        {
            var id = (int)_sqliteConnection.Insert(fsivFolder);
            return id;
        }

        public FsivFile GetFsivFileByFileNameAndParentFolderId(string fileName, int folderId)
        {
            const string sql = "select * from FileList where fileName = @FileName and folderID = @FolderId";
            var fsivFile = _sqliteConnection.QueryFirstOrDefault<FsivFile>(sql, new { FileName = fileName, FolderId = folderId });
            return fsivFile;
        }

        public int AddFile(FsivFile fsivFile)
        {
            var id = (int)_sqliteConnection.Insert(fsivFile);
            return id;
        }

        public bool UpdateFile(FsivFile fsivFile)
        {
            var result = _sqliteConnection.Update(fsivFile);
            return result;
        }
    }
}