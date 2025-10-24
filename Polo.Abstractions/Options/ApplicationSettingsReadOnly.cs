namespace Polo.Abstractions.Options;

public class ApplicationSettingsReadOnly
{
    public ApplicationSettingsReadOnly(ApplicationSettings applicationSettings)
    {
        LogFilePath = applicationSettings.LogFilePath;
        DefaultSourceFolderPath = applicationSettings.DefaultSourceFolderPath;
        RawFolderName = applicationSettings.RawFolderName;
        ImageResizeLongSideLimit = applicationSettings.ImageResizeLongSideLimit;
        ImageResizeMegaPixelsLimit = applicationSettings.ImageResizeMegaPixelsLimit;
        WatermarkPath = applicationSettings.WatermarkPath;
        OutputSubfolderName = applicationSettings.OutputSubfolderName;
        OutputFileName = applicationSettings.OutputFileName;
        WatermarkPosition = applicationSettings.WatermarkPosition;
        ImageQuality = applicationSettings.ImageQuality;
        FsivThumbnailSize = applicationSettings.FsivThumbnailSize;
        WatermarkTransparencyPercent = applicationSettings.WatermarkTransparencyPercent;
        SqliteDbConnectionStrings = applicationSettings.SqliteDbConnectionStrings;
        FileForProcessExtensions = [.. applicationSettings.FileForProcessExtensions];
        ImageFileExtensions = [.. applicationSettings.ImageFileExtensions];
        RawFileExtensions = [.. applicationSettings.RawFileExtensions];
        VideoFileExtensions = [.. applicationSettings.VideoFileExtensions];
        RedundantFiles = [.. applicationSettings.RedundantFiles];
    }

    public string? LogFilePath { get; }

    public string DefaultSourceFolderPath { get; }

    public string RawFolderName { get; }

    public uint ImageResizeLongSideLimit { get; }

    public float ImageResizeMegaPixelsLimit { get; }

    public string WatermarkPath { get; }

    public string OutputSubfolderName { get; }

    public string OutputFileName { get; }

    public string WatermarkPosition { get; }

    public int WatermarkTransparencyPercent { get; }

    public uint ImageQuality { get; }

    public uint FsivThumbnailSize { get; }

    public string SqliteDbConnectionStrings { get; }

    public IReadOnlyCollection<string> FileForProcessExtensions { get; }

    public IReadOnlyCollection<string> ImageFileExtensions { get; }

    public IReadOnlyCollection<string> RawFileExtensions { get; }

    public IReadOnlyCollection<string> VideoFileExtensions { get; }

    public IReadOnlyCollection<string> RedundantFiles { get; }
}