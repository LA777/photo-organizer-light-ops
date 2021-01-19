using System.Collections.Generic;

namespace Polo.Options
{
    public class ApplicationSettings
    {
        public string LogFilePath { get; private set; } = "logs\\polo-log-.txt";
        public string DefaultSourceDriveName { get; private set; } = string.Empty;
        public string RawFolderName { get; private set; } = "RAW";
        public List<string> JpegFileExtensions { get; private set; } = new List<string>();
        public List<string> RawFileExtensions { get; private set; } = new List<string>();
        public List<string> VideoFileExtensions { get; private set; } = new List<string>();

        public ApplicationSettings() { }

        public ApplicationSettings(string logFilePath = null,
            string defaultSourceDriveName = null,
            string rawFolderName = null,
            IEnumerable<string> jpegFileExtensions = null,
            IEnumerable<string> rawFileExtensions = null,
            IEnumerable<string> videoFileExtensions = null)
        {
            LogFilePath = logFilePath;
            DefaultSourceDriveName = defaultSourceDriveName;
            RawFolderName = rawFolderName;

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
