using System.Collections.Generic;

namespace Polo.Options
{
    public class ApplicationSettings
    {
        public string DefaultSourceDriveName { get; private set; }
        public string RawFolderName { get; private set; }
        public List<string> JpegFileExtensions { get; private set; }
        public List<string> RawFileExtensions { get; private set; }
        public List<string> VideoFileExtensions { get; private set; }

        public ApplicationSettings(string defaultSourceDriveName = null,
            string rawFolderName = null,
            IEnumerable<string> jpegFileExtensions = null,
            IEnumerable<string> rawFileExtensions = null,
            IEnumerable<string> videoFileExtensions = null)
        {
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
