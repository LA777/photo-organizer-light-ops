using System;
using System.IO;
using System.Linq;

namespace Polo.Commands
{
    public class RemoveOrphanageRawCommand : ICommand
    {
        public string Name => "remove-orphanage-raw";

        public string ShortName => "ror";

        public string Description => "Removes orphanage raw files from RAW folder.";

        public void Action()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var jpegFiles = Directory.EnumerateFiles(currentDirectory, "*.JPG", SearchOption.TopDirectoryOnly).ToList();

            var rawFolderPath = Path.Join(currentDirectory, "RAW");
            var rawFiles = Directory.EnumerateFiles(rawFolderPath, "*.ORF", SearchOption.TopDirectoryOnly).ToList();

            var jpegFilesNames = jpegFiles.Select(Path.GetFileNameWithoutExtension);
            var rawFilesNames = rawFiles.Select(Path.GetFileNameWithoutExtension);

            var orphanageRawFilesNames = rawFilesNames.Except(jpegFilesNames);
            var orphanageRawFiles = orphanageRawFilesNames.Select(x => Path.Join(rawFolderPath, $"{x}.ORF"));

            foreach (var rawFile in orphanageRawFiles)
            {
                File.Delete(rawFile);
                var fileInfo = new FileInfo(rawFile);
                Console.WriteLine($"RAW file deleted: {fileInfo.Name}");
            }
        }
    }
}
