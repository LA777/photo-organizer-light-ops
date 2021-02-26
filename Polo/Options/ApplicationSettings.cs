using System.Collections.Generic;

namespace Polo.Options
{
    public class ApplicationSettings // TODO LA - Move to Abstractions
    {
        public string LogFilePath { get; private set; } = "logs\\polo-log-.txt";
        public string DefaultSourceFolderName { get; private set; } = string.Empty;
        public string RawFolderName { get; private set; } = "RAW";
        public string ResizedImageSubfolderName { get; private set; } = "small";
        public int ImageResizeLongSideLimit { get; private set; } = 1600;
        public List<string> JpegFileExtensions { get; private set; } = new List<string>();
        public List<string> RawFileExtensions { get; private set; } = new List<string>();
        public List<string> VideoFileExtensions { get; private set; } = new List<string>();

        public ApplicationSettings() { }

        public ApplicationSettings(string logFilePath = null,
            string defaultSourceFolderName = null,
            string rawFolderName = null,
            string resizedImageSubfolderName = null,
            int imageResizeLongSideLimit = 0,
            IEnumerable<string> jpegFileExtensions = null,
            IEnumerable<string> rawFileExtensions = null,
            IEnumerable<string> videoFileExtensions = null)
        {
            LogFilePath = logFilePath;
            DefaultSourceFolderName = defaultSourceFolderName;
            RawFolderName = rawFolderName;
            ResizedImageSubfolderName = resizedImageSubfolderName;
            ImageResizeLongSideLimit = imageResizeLongSideLimit;

            JpegFileExtensions = new List<string>();
            if (jpegFileExtensions != null)
            {
                JpegFileExtensions.AddRange(jpegFileExtensions);
            }

            RawFileExtensions = new List<string>();
            if (rawFileExtensions != null)
            {
                RawFileExtensions.AddRange(rawFileExtensions);
            }

            VideoFileExtensions = new List<string>();
            if (videoFileExtensions != null)
            {
                VideoFileExtensions.AddRange(videoFileExtensions);
            }
        }
    }
}
