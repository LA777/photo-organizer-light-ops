using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace Polo.Extensions
{
    public static class FileInfoExtension
    {
        public static bool DeleteToRecycleBin(this FileInfo fileInfo)
        {
            fileInfo?.Refresh();

            if (fileInfo == null || !fileInfo.Exists)
            {
                return false;
            }

            FileSystem.DeleteFile(fileInfo.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

            return true;
        }
    }
}
