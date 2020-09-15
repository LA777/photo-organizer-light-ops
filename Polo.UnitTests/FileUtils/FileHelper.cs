using Polo.UnitTests.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Polo.UnitTests.FileUtils
{
    public class FileHelper
    {
        public const int FileLimit = 5;
        public const string JpegExtension = "JPG";
        public const string RawExtension = "ORF";
        public const string TestFolderName = "UnitTestTemp";
        public const string FilePrefix = "UTP-";
        public const string RawFolderName = "RAW";

        public static string UnitTestFolder { get; set; } = Directory.GetCurrentDirectory();
        // "C:\\Git\\LA777\\photo-organizer-light-ops\\Polo.UnitTests\\bin\\Debug"

        public static string TestFolderFullPath { get; set; } = Path.Join(UnitTestFolder, TestFolderName);

        public static void CreateJpegFiles(string folderPath)
        {
            CreateFiles(folderPath, JpegExtension);
        }

        public static void DeleteRandomJpegFiles(string folderPath, int filesDeleteCount)
        {
            for (var i = 0; i < filesDeleteCount; i++)
            {
                var jpegFiles = Directory.EnumerateFiles(folderPath, $"*.{JpegExtension}", SearchOption.TopDirectoryOnly).ToList();
                var random = new Random(42);
                var randomIndex = random.Next(0, jpegFiles.Count);
                var randomFilePath = jpegFiles[randomIndex];
                File.Delete(randomFilePath);
            }
        }

        public static void CreateRawFiles(string folderPath)
        {
            CreateFiles(folderPath, RawExtension);
        }
        public static string CreateFoldersAndFilesByStructure(Folder folderStructure)
        {
            var testFolderFullPath = CreateTestFolder();
            foreach (var subFolder in folderStructure.SubFolders)
            {
                CreateFoldersAndFiles(subFolder, testFolderFullPath);
            }

            return testFolderFullPath;
        }

        private static void CreateFoldersAndFiles(Folder folderStructure, string parentFolderFullPath)
        {
            var currentFolderPath = Path.Combine(parentFolderFullPath, folderStructure.Name);
            if (!Directory.Exists(currentFolderPath))
            {
                Directory.CreateDirectory(currentFolderPath);
            }

            foreach (var fileNameWithExtension in folderStructure.Files)
            {
                CreateFileByFullName(currentFolderPath, fileNameWithExtension);
            }

            foreach (var folder in folderStructure.SubFolders)
            {
                CreateFoldersAndFiles(folder, currentFolderPath);
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
                var fileInfo = new FileInfo(file);
                folderStructure.Files.Add(fileInfo.Name);
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


        public static string CreateTestFolder()
        {
            var directoryInfo = Directory.CreateDirectory(TestFolderFullPath);

            return directoryInfo.FullName;
        }

        public static string CreateRawFolder()
        {
            var rawFolderPath = Path.Join(TestFolderFullPath, RawFolderName);
            var directoryInfo = Directory.CreateDirectory(rawFolderPath);

            return directoryInfo.FullName;
        }

        public static void TryDeleteTestFolder(int retryCount = 3)
        {
            if (!Directory.Exists(TestFolderFullPath))
            {
                return;
            }

            Environment.CurrentDirectory = Directory.GetParent(TestFolderFullPath).FullName;

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

        private static void CreateFiles(string folderPath, string fileExtensions)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            for (var i = 0; i < FileLimit; i++)
            {
                var fileName = $"{FilePrefix}{i}";
                CreateFile(folderPath, fileName, fileExtensions);
            }
        }

        private static void CreateFile(string folderPath, string fileName, string fileExtensions)
        {
            var fileNameWithExtensions = $"{fileName}.{fileExtensions}";
            CreateFileByFullName(folderPath, fileNameWithExtensions);
        }

        private static void CreateFileByFullName(string folderPath, string fileNameWithExtension)
        {
            var fullFileName = Path.Join(folderPath, fileNameWithExtension);

            using var fileStream = File.Create(fullFileName);
            using var writer = new BinaryWriter(fileStream);
            writer.Write($"{fileNameWithExtension}-{Guid.NewGuid()}");
            writer.Dispose();
            fileStream.Dispose();
        }
    }
}
