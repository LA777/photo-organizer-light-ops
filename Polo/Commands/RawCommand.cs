using System;
using System.IO;

namespace Polo.Commands
{
    public class RawCommand : ICommand
    {
        public string Name => "raw";

        public string ShortName => "r";

        public string Description => "Creates RAW sub-folder in the current folder and moves all RAW files to this sub-folder.";

        public void Action()
        {
            var currentDirectory = Environment.CurrentDirectory;
            Console.WriteLine(currentDirectory);

            var rawFolderPath = Path.Join(currentDirectory, "RAW");
            Directory.CreateDirectory(rawFolderPath);

            var rawFiles = Directory.EnumerateFiles(currentDirectory, "*.ORF", SearchOption.TopDirectoryOnly);

            foreach (var rawFilePath in rawFiles)
            {
                var fileInfo = new FileInfo(rawFilePath);
                var destinationFilePath = Path.Join(rawFolderPath, fileInfo.Name);
                File.Move(rawFilePath, destinationFilePath);
                Console.WriteLine($"Moved: {fileInfo.Name}");
            }
        }
    }
}
