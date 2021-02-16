namespace Polo.UnitTests.Models
{
    public class FotoFile
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public FotoFile(string name, string extension, int width = 0, int height = 0)
        {
            Name = name;
            Extension = extension;
            Width = width;
            Height = height;
        }

        public string GetNameWithExtension()
        {
            return $"{Name}.{Extension}";
        }
    }
}
