namespace Polo.Comparers
{
    public class FileNameWithoutExtensionComparer : IEqualityComparer<string>
    {
        // TODO LA - Cover with UTs
        public bool Equals(string? x, string? y)
        {
            var fileNameWithoutExtensionX = Path.GetFileNameWithoutExtension(x);
            var fileNameWithoutExtensionY = Path.GetFileNameWithoutExtension(y);

            return string.Equals(fileNameWithoutExtensionX, fileNameWithoutExtensionY, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string fileFullPath)
        {
            // TODO LA - Cover with UTs
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileFullPath).ToUpper();

            return fileNameWithoutExtension.GetHashCode();
        }
    }
}