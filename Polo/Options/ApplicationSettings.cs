using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Polo.Options
{
    public class ApplicationSettings // TODO LA - Move to Abstractions?
    {
        public string LogFilePath { get; set; }

        [Required(ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public string DefaultSourceFolderPath { get; set; } = string.Empty;

        [Required(ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public string RawFolderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public string ResizedImageSubfolderName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "ERROR: Value for {0} should be between {1} and {2}.")]
        public int ImageResizeLongSideLimit { get; set; } = 0;

        [Required(ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public string WatermarkPath { get; set; } = string.Empty;

        [Required(ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public string WatermarkOutputFolderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public string WatermarkPosition { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "ERROR: Value for {0} should be between {1} and {2}.")]
        public int WatermarkTransparencyPercent { get; set; } = 0;

        [MinLength(1, ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public ICollection<string> JpegFileExtensions { get; set; } = new List<string>();

        [MinLength(1, ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public ICollection<string> RawFileExtensions { get; set; } = new List<string>();

        [MinLength(1, ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public ICollection<string> VideoFileExtensions { get; set; } = new List<string>();

        //public ApplicationSettings()
        //{

        //}

        //public ApplicationSettings(string logFilePath = null,
        //    string defaultSourceFolderPath = null,
        //    string rawFolderName = null,
        //    string resizedImageSubfolderName = null,
        //    int imageResizeLongSideLimit = 0,
        //    string watermarkPath = null,
        //    string watermarkOutputFolderName = null,
        //    string watermarkPosition = null,
        //    int watermarkTransparencyPercent = 0,
        //    IEnumerable<string> jpegFileExtensions = null,
        //    IEnumerable<string> rawFileExtensions = null,
        //    IEnumerable<string> videoFileExtensions = null)
        //{
        //    LogFilePath = logFilePath;
        //    DefaultSourceFolderPath = defaultSourceFolderPath;
        //    RawFolderName = rawFolderName;
        //    ResizedImageSubfolderName = resizedImageSubfolderName;
        //    ImageResizeLongSideLimit = imageResizeLongSideLimit;
        //    WatermarkPath = watermarkPath;
        //    WatermarkOutputFolderName = watermarkOutputFolderName;
        //    WatermarkPosition = watermarkPosition;
        //    WatermarkTransparencyPercent = watermarkTransparencyPercent;

        //    JpegFileExtensions = new List<string>(jpegFileExtensions ?? throw new ArgumentNullException(nameof(jpegFileExtensions)));
        //    RawFileExtensions = new List<string>(rawFileExtensions ?? throw new ArgumentNullException(nameof(rawFileExtensions)));
        //    VideoFileExtensions = new List<string>(videoFileExtensions ?? throw new ArgumentNullException(nameof(videoFileExtensions)));
        //}
    }
}
