using ImageMagick;
using Polo.UnitTests.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Polo.UnitTests.FileUtils
{
    public static class FileHelper
    {
        public const int FileLimit = 5;
        public const string JpegExtension = "JPG";
        public const string RawExtension = "ORF";
        private const string TestFolderName = "UnitTestTemp";
        public const string FilePrefix = "UTP-";
        public const string RawFolderName = "RAW";
        public const string WatermarkFolderName = "watermark";
        public static readonly FotoFile Watermark = new FotoFile("watermark", "png", 10, 10);

        private static string UnitTestFolder
        {
            get
            {
                try
                {
                    var testFodlerPath = Environment.GetEnvironmentVariable("TEST_FOLDER");
                    if (!string.IsNullOrWhiteSpace(testFodlerPath))
                    {
                        return testFodlerPath;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                return Directory.GetCurrentDirectory(); // "photo-organizer-light-ops\\Polo.UnitTests\\bin\\Debug"
            }
        }

        private static string TestFolderFullPath { get; set; } = Path.Join(UnitTestFolder, TestFolderName);

        public static string CreateFoldersAndFilesByStructure(Folder folderStructure)
        {
            var testFolderFullPath = CreateTestFolder();

            foreach (var fotoFile in folderStructure.Files)
            {
                CreateFileByFotoFile(testFolderFullPath, fotoFile);
            }

            foreach (var subFolder in folderStructure.SubFolders)
            {
                CreateFoldersAndFiles(subFolder, testFolderFullPath);
            }

            return testFolderFullPath;
        }

        public static string CreateWatermark()
        {
            var watermarkPath = Path.Combine(TestFolderFullPath, WatermarkFolderName, $"{Watermark.Name}.{Watermark.Extension}");

            if (File.Exists(watermarkPath))
            {
                return watermarkPath;
            }

            var folderStructureInitial = new Folder()
            {
                SubFolders = new List<Folder>()
                {
                    new Folder()
                    {
                        Name = WatermarkFolderName,
                        Files = new List<FotoFile>()
                        {
                            Watermark
                        }
                    }
                }
            };

            CreateFoldersAndFilesByStructure(folderStructureInitial);

            return Path.Combine(TestFolderFullPath, WatermarkFolderName, $"{Watermark.Name}.{Watermark.Extension}");
        }

        private static string CreateTestFolder()
        {
            if (Directory.Exists(TestFolderFullPath))
            {
                Directory.Delete(TestFolderFullPath, true);
            }

            var directoryInfo = Directory.CreateDirectory(TestFolderFullPath);

            return directoryInfo.FullName;
        }

        public static void TryDeleteTestFolder(int retryCount = 3)
        {
            if (!Directory.Exists(TestFolderFullPath))
            {
                return;
            }

            Environment.CurrentDirectory = Directory.GetParent(TestFolderFullPath)?.FullName ?? throw new DirectoryNotFoundException(TestFolderFullPath);

            try
            {
                Directory.Delete(TestFolderFullPath, true);
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

        public static Folder CreateFolderStructureByFolderAndFiles(string folderFullPath)
        {
            var folderStructure = CreateFolderStructure(folderFullPath);

            return folderStructure;
        }

        private static Folder CreateFolderStructure(string folderFullPath)
        {
            var folderStructure = new Folder();

            var files = Directory.EnumerateFiles(folderFullPath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var fileExtension = Path.GetExtension(file).TrimStart('.');
                var fotoFile = new FotoFile(fileName, fileExtension);
                var fileExtensionUpper = fileExtension.ToUpper();

                if (fileExtensionUpper == "JPEG" || fileExtensionUpper == "JPG" || fileExtensionUpper == "PNG")
                {
                    var (width, height) = TryGetWidthAndHeight(file);
                    fotoFile.Width = width;
                    fotoFile.Height = height;
                }

                folderStructure.Files.Add(fotoFile);
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

        private static (int, int) TryGetWidthAndHeight(string filePath)
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

        private static void CreateFoldersAndFiles(Folder folderStructure, string parentFolderFullPath)
        {
            var currentFolderPath = Path.Combine(parentFolderFullPath, folderStructure.Name);
            if (!Directory.Exists(currentFolderPath))
            {
                Directory.CreateDirectory(currentFolderPath);
            }

            foreach (var fotoFile in folderStructure.Files)
            {
                CreateFileByFotoFile(currentFolderPath, fotoFile);
            }

            foreach (var folder in folderStructure.SubFolders)
            {
                CreateFoldersAndFiles(folder, currentFolderPath);
            }
        }

        private static void CreateFileByFotoFile(string folderPath, FotoFile fotoFile)
        {
            var fullFileName = Path.Join(folderPath, fotoFile.GetNameWithExtension());

            if (fotoFile.Width > 0 || fotoFile.Height > 0)
            {
                if (fotoFile == Watermark)
                {
                    using var image = new MagickImage(new MagickColor("#FFFFFF"), fotoFile.Width, fotoFile.Height);
                    image.Write(fullFileName);
                }
                else
                {
                    using var image = new MagickImage(new MagickColor("#000000"), fotoFile.Width, fotoFile.Height);
                    image.Write(fullFileName);
                }
            }
            else
            {
                using var fileStream = File.Create(fullFileName);
                using var writer = new BinaryWriter(fileStream);
                writer.Write($"{fotoFile.GetNameWithExtension()}-{Guid.NewGuid()}");
                writer.Dispose();
                fileStream.Dispose();
            }
        }
    }
}
