using System.ComponentModel.DataAnnotations;

namespace Polo.Abstractions.Options
{
    public class ApplicationSettings
    {
        public string LogFilePath { get; set; }

        [Required(ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public string DefaultSourceFolderPath { get; set; } = string.Empty;

        [Required(ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public string RawFolderName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "ERROR: Value for {0} should be between {1} and {2}.")]
        public int ImageResizeLongSideLimit { get; set; }

        [Range(0.001f, int.MaxValue, ErrorMessage = "ERROR: Value for {0} should be between {1} and {2}.")]
        public float ImageResizeMegaPixelsLimit { get; set; }

        [Required(ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public string WatermarkPath { get; set; } = string.Empty;

        [Required(ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public string OutputSubfolderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public string WatermarkPosition { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "ERROR: Value for {0} should be between {1} and {2}.")]
        public int WatermarkTransparencyPercent { get; set; }

        [Range(0, 100, ErrorMessage = "ERROR: Value for {0} should be between {1} and {2}.")]
        public int ImageQuality { get; set; }

        [MinLength(1, ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public ICollection<string> FileForProcessExtensions { get; set; } = new List<string>();

        [MinLength(1, ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public ICollection<string> ImageFileExtensions { get; set; } = new List<string>();

        [MinLength(1, ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public ICollection<string> RawFileExtensions { get; set; } = new List<string>();

        [MinLength(1, ErrorMessage = "ERROR: Value for {0} should contain some data.")]
        public ICollection<string> VideoFileExtensions { get; set; } = new List<string>();

        public ICollection<string> RedundantFiles { get; set; } = new List<string>();
    }
}