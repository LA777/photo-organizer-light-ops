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
            for (var i = 0; i < FileLimit; i++)
            {
                var fileName = $"{FilePrefix}{i}";
                CreateFile(folderPath, fileName, fileExtensions);
            }
        }

        private static void CreateFile(string folderPath, string fileName, string fileExtensions)
        {
            var fileNameWithExtensions = $"{fileName}.{fileExtensions}";
            var fullFileName = Path.Join(folderPath, fileNameWithExtensions);

            using var fileStream = File.Create(fullFileName);
            using var writer = new BinaryWriter(fileStream);
            writer.Write($"{fileNameWithExtensions}-{Guid.NewGuid()}");
            writer.Dispose();
            fileStream.Dispose();
        }
    }
}
