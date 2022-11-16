using Dapper.Contrib.Extensions;
using Polo.Abstractions.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Polo.Abstractions.Entities
{
    [Dapper.Contrib.Extensions.Table("FileList")]
    public class FsivFile
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [ForeignKey("folderID")]
        public int FolderId { get; set; }

        [Column("itemNO")]
        public int ItemNo { get; set; }

        [Column("isFolder")]
        public FsivItemType IsFolder { get; set; }

        [Column("fileName")]
        public string? FileName { get; set; }

        [Column("isTag")]
        public int IsTag { get; set; }

        [Column("ratings")]
        public int Ratings { get; set; }

        [Column("XP_rating")]
        public int Xp_Rating { get; set; }

        [Column("fileTime")]
        public int FileTime { get; set; }

        [Column("exifTime")]
        public int ExifTime { get; set; }

        [Column("fileSize")]
        public int FileSize { get; set; }

        [Column("width")]
        public int Width { get; set; }

        [Column("height")]
        public int Height { get; set; }

        [Column("pages")]
        public int Pages { get; set; }

        [Column("imageType")]
        public FsivImageType ImageType { get; set; }

        [Column("imgSize1")]
        public int ImgSize1 { get; set; }

        [Column("imgSize2")]
        public int ImgSize2 { get; set; }

        [Column("img1")]
        public byte[]? Img1 { get; set; }

        [Column("img2")]
        public byte[]? Img2 { get; set; }
    }
}