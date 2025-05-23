using ImageMagick;
using Polo.UnitTests.Models;
using System;
using System.IO;
using System.Threading;

namespace Polo.UnitTests.FileUtils;

public class FileHelper
{
    private const string TestFolderName = "UnitTestTemp";
    private const string WatermarkFolderName = "watermark";
    public static readonly PhotoFile Watermark = new("watermark", ".png", 10, 10);
    private readonly string _testFolderFullPath;

    public FileHelper(string testFolderPath)
    {
        var unitTestFolder = !string.IsNullOrWhiteSpace(testFolderPath) ? testFolderPath : Directory.GetCurrentDirectory(); // "photo-organizer-light-ops\\Polo.UnitTests\\bin\\Debug"
        _testFolderFullPath = Path.Join(unitTestFolder, TestFolderName);
    }

    public string CreateFoldersAndFilesByStructure(Folder folderStructure)
    {
        var testFolderFullPath = CreateTestFolder();

        foreach (var photoFile in folderStructure.Files)
        {
            CreateFileByPhotoFile(testFolderFullPath, photoFile);
        }

        foreach (var textFile in folderStructure.TextFiles)
        {
            CreateFileByTextFile(testFolderFullPath, textFile);
        }

        foreach (var subFolder in folderStructure.SubFolders)
        {
            CreateFoldersAndFiles(subFolder, testFolderFullPath);
        }

        return testFolderFullPath;
    }

    public string CreateWatermark()
    {
        var watermarkPath = Path.Combine(_testFolderFullPath, WatermarkFolderName, $"{Watermark.Name}{Watermark.Extension}");

        if (File.Exists(watermarkPath))
        {
            return watermarkPath;
        }

        var folderStructureInitial = new Folder()
        {
            SubFolders =
            [
                new Folder
                {
                    Name = WatermarkFolderName,
                    Files = [Watermark]
                }
            ]
        };

        CreateFoldersAndFilesByStructure(folderStructureInitial);

        return Path.Combine(_testFolderFullPath, WatermarkFolderName, $"{Watermark.Name}{Watermark.Extension}");
    }

    private string CreateTestFolder()
    {
        if (Directory.Exists(_testFolderFullPath))
        {
            Directory.Delete(_testFolderFullPath, true);
        }

        var directoryInfo = Directory.CreateDirectory(_testFolderFullPath);

        return directoryInfo.FullName;
    }

    public void TryDeleteTestFolder(int retryCount = 3)
    {
        if (!Directory.Exists(_testFolderFullPath))
        {
            return;
        }

        Environment.CurrentDirectory = Directory.GetParent(_testFolderFullPath)?.FullName ?? throw new DirectoryNotFoundException(_testFolderFullPath);

        try
        {
            Directory.Delete(_testFolderFullPath, true);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);

            if (retryCount < 0)
            {
                return;
            }

            Thread.Sleep(1000);
            TryDeleteTestFolder(--retryCount);
        }
    }

    public Folder CreateFolderStructureByFolderAndFiles(string folderFullPath)
    {
        var folderStructure = CreateFolderStructure(folderFullPath);

        return folderStructure;
    }

    private Folder CreateFolderStructure(string folderFullPath)
    {
        var folderStructure = new Folder();

        var files = Directory.EnumerateFiles(folderFullPath, "*", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var fileExtension = Path.GetExtension(file);
            var photoFile = new PhotoFile(fileName, fileExtension);
            var fileExtensionUpper = fileExtension.ToUpper();

            if (fileExtensionUpper is ".JPEG" or ".JPG" or ".PNG")
            {
                var (width, height) = TryGetWidthAndHeight(file);
                photoFile.Width = width;
                photoFile.Height = height;
            }

            folderStructure.Files.Add(photoFile);
        }

        var subFoldersFullPath = Directory.EnumerateDirectories(folderFullPath, "*", SearchOption.TopDirectoryOnly);

        foreach (var subFolderFullPath in subFoldersFullPath)
        {
            var subFolderStructure = CreateFolderStructure(subFolderFullPath);
            subFolderStructure.Name = new DirectoryInfo(subFolderFullPath).Name;
            folderStructure.SubFolders.Add(subFolderStructure);
        }

        return folderStructure;
    }

    private (uint, uint) TryGetWidthAndHeight(string filePath)
    {
        try
        {
            var info = new MagickImageInfo(filePath);

            return (info.Width, info.Height);
        }
        catch (Exception)
        {
            // ignored
        }

        return (0, 0);
    }

    private void CreateFoldersAndFiles(Folder folderStructure, string parentFolderFullPath)
    {
        var currentFolderPath = Path.Combine(parentFolderFullPath, folderStructure.Name);
        if (!Directory.Exists(currentFolderPath))
        {
            Directory.CreateDirectory(currentFolderPath);
        }

        foreach (var photoFile in folderStructure.Files)
        {
            CreateFileByPhotoFile(currentFolderPath, photoFile);
        }

        foreach (var textFile in folderStructure.TextFiles)
        {
            CreateFileByTextFile(currentFolderPath, textFile);
        }

        foreach (var folder in folderStructure.SubFolders)
        {
            CreateFoldersAndFiles(folder, currentFolderPath);
        }
    }

    private void CreateFileByPhotoFile(string folderPath, PhotoFile photoFile)
    {
        var fullFileName = Path.Join(folderPath, photoFile.GetNameWithExtension());

        if (photoFile.Width > 0 || photoFile.Height > 0)
        {
            if (photoFile == Watermark)
            {
                using var image = new MagickImage(new MagickColor("#FFFFFF"), photoFile.Width, photoFile.Height);
                image.Write(fullFileName);
            }
            else
            {
                using var image = new MagickImage(new MagickColor("#000000"), photoFile.Width, photoFile.Height);
                image.Write(fullFileName);
            }
        }
        else
        {
            using var fileStream = File.Create(fullFileName);
            using var writer = new BinaryWriter(fileStream);
            writer.Write($"{photoFile.GetNameWithExtension()}-{Guid.NewGuid()}");
        }
    }

    private void CreateFileByTextFile(string folderPath, TextFile textFile)
    {
        var fullFileName = Path.Join(folderPath, textFile.GetNameWithExtension());
        using var outputFile = new StreamWriter(fullFileName);
        outputFile.WriteLine(textFile);
    }
}
