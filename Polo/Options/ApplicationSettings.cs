using System.Collections.Generic;

namespace Polo.Options
{
    public class ApplicationSettings
    {
        public string DefaultSourceDriveName { get; private set; }
        public IEnumerable<string> JpegFileExtensions { get; private set; }
        public IEnumerable<string> RawFileExtensions { get; private set; }
        public IEnumerable<string> VideoFileExtensions { get; private set; }

        public ApplicationSettings(string defaultSourceDriveName,
            IEnumerable<string> jpegFileExtensions,
            IEnumerable<string> rawFileExtensions,
            IEnumerable<string> videoFileExtensions)
        {
            DefaultSourceDriveName = defaultSourceDriveName;
            JpegFileExtensions = new List<string>(jpegFileExtensions);
            RawFileExtensions = new List<string>(rawFileExtensions);
            VideoFileExtensions = new List<string>(videoFileExtensions);
        }
    }
}
