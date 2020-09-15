using System;
using System.IO;
using System.Linq;

namespace Polo.Commands
{
    public class MoveRawToJpegFolderCommand : ICommand
    {
        public string Name => "moveraw";

        public string ShortName => "mr";

        public string Description => "Move RAW files to the RAW sub-folder in the JPEG folder.";

        public void Action()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var rawFolderPath = Path.Join(currentDirectory, "RAW");

            // get all RAW files
            var rawFiles = Directory.EnumerateFiles(rawFolderPath, "*.ORF", SearchOption.TopDirectoryOnly);

            foreach (var rawFilePath in rawFiles)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(rawFilePath);
                var allFotosFolder = Directory.GetParent(currentDirectory).FullName;
                var jpegFiles = Directory.EnumerateFiles(allFotosFolder, $"{fileNameWithoutExtension}.jpg", SearchOption.AllDirectories);
                if (jpegFiles.Count() > 1)
                {
                    throw new Exception($"Files more than one. {jpegFiles.First()}");
                }

                foreach (var jpegFile in jpegFiles)
                {
                    var parentFolderFullPath = Directory.GetParent(jpegFile).FullName;
                    var rawSubFolderPath = Path.Combine(parentFolderFullPath, "RAW");
                    Directory.CreateDirectory(rawSubFolderPath);

                    var fileInfo = new FileInfo(rawFilePath);
                    var destinationFilePath = Path.Join(rawSubFolderPath, fileInfo.Name);
                    File.Move(rawFilePath, destinationFilePath);
                    Console.WriteLine($"Moved: {fileInfo.Name}");
                }
            }
        }
    }
}
