using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;

namespace Polo.Extensions
{
    public static class FileInfoExtension
    {
        public static bool DeleteToRecycleBin(this FileInfo fileInfo)
        {
            fileInfo.Refresh();

            if (fileInfo is not { Exists: true })
            {
                return false;
            }

            FileSystem.DeleteFile(fileInfo.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

            return true;
        }

        public static string GenerateFileFullPath(this FileInfo fileInfo, string destinationFolderFullPath)
        {
            var destinationImagePath = Path.Combine(destinationFolderFullPath, fileInfo.Name);
            if (!File.Exists(destinationImagePath))
            {
                return destinationImagePath;
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);

            const string regexPattern = "\\([0-9]+\\)$";
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            if (!regex.IsMatch(fileNameWithoutExtension))
            {
                destinationImagePath = Path.Combine(destinationFolderFullPath, $"{fileNameWithoutExtension} (0){fileInfo.Extension}");

                if (!File.Exists(destinationImagePath))
                {
                    return destinationImagePath;
                }
            }

            string nameWithoutIndex;
            // ReSharper disable once TooWideLocalVariableScope
            int index;

            do
            {
                const string regexPatternForIndex = "[0-9]+";
                var regexIndex = new Regex(regexPatternForIndex, RegexOptions.IgnoreCase);

                var destinationFileNameWithoutExtension = Path.GetFileNameWithoutExtension(destinationImagePath); // '101 (0)'
                var indexInBraces = regex.Match(destinationFileNameWithoutExtension); // '(0)'

                var match = regexIndex.Match(indexInBraces.Value);
                var value = match.Value; // '0'
                index = Convert.ToInt32(value); // 0
                index++;

                var length = indexInBraces.Length;
                nameWithoutIndex = destinationFileNameWithoutExtension.Substring(0, destinationFileNameWithoutExtension.Length - length); // '101'

                destinationImagePath = Path.Combine(destinationFolderFullPath, $"{nameWithoutIndex}({index}){fileInfo.Extension}");
            } while (File.Exists(destinationImagePath));

            return destinationImagePath;
        }
    }
}