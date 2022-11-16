using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Polo.Abstractions.Entities
{
    [Dapper.Contrib.Extensions.Table("FolderList")]
    public class FsivFolder
    {
        [Key]
        [Column("folderID")]
        public int FolderId { get; set; }

        [Column("folderName")]
        public string? FolderName { get; set; } // C:\T\PICS\

        [Column("lastAccess")]
        public int LastAccess { get; set; }
    }
}