﻿using ImageMagick;
using Polo.UnitTests.Models;
using System;
using System.IO;
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
                using var image = new MagickImage(new MagickColor("#000000"), fotoFile.Width, fotoFile.Height);
                image.Write(fullFileName);
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

                if (fileExtensionUpper == "JPEG" || fileExtensionUpper == "JPG")
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
            catch (Exception) { }

            return (0, 0);
        }


        public static string CreateTestFolder()
        {
            var directoryInfo = Directory.CreateDirectory(TestFolderFullPath);

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
    }
}
