﻿using System.Collections.Generic;

namespace Polo.Abstractions.Options
{
    public class ApplicationSettingsReadOnly
    {
        public string LogFilePath { get; private set; }

        public string DefaultSourceFolderPath { get; private set; }

        public string RawFolderName { get; private set; }

        public string ResizedImageSubfolderName { get; private set; }

        public int ImageResizeLongSideLimit { get; private set; }

        public string WatermarkPath { get; private set; }

        public string WatermarkOutputFolderName { get; private set; }

        public string WatermarkPosition { get; private set; }

        public int WatermarkTransparencyPercent { get; private set; }

        public int ImageQuality { get; private set; }

        public IReadOnlyCollection<string> FileForProcessExtensions { get; private set; } = new List<string>();

        public IReadOnlyCollection<string> RawFileExtensions { get; private set; } = new List<string>();

        public IReadOnlyCollection<string> VideoFileExtensions { get; private set; } = new List<string>();

        public ApplicationSettingsReadOnly(ApplicationSettings applicationSettings)
        {
            LogFilePath = applicationSettings.LogFilePath;
            DefaultSourceFolderPath = applicationSettings.DefaultSourceFolderPath;
            RawFolderName = applicationSettings.RawFolderName;
            ResizedImageSubfolderName = applicationSettings.ResizedImageSubfolderName;
            ImageResizeLongSideLimit = applicationSettings.ImageResizeLongSideLimit;
            WatermarkPath = applicationSettings.WatermarkPath;
            WatermarkOutputFolderName = applicationSettings.WatermarkOutputFolderName;
            WatermarkPosition = applicationSettings.WatermarkPosition;
            ImageQuality = applicationSettings.ImageQuality;
            WatermarkTransparencyPercent = applicationSettings.WatermarkTransparencyPercent;
            FileForProcessExtensions = new List<string>(applicationSettings.FileForProcessExtensions);
            RawFileExtensions = new List<string>(applicationSettings.RawFileExtensions);
            VideoFileExtensions = new List<string>(applicationSettings.VideoFileExtensions);
        }
    }
}
