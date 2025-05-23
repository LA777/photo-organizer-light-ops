namespace Polo.UnitTests.Models;

public class PhotoFile
{
    public string Name { get; set; }
    public string Extension { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }

    public PhotoFile(string name, string extension, uint width = 0, uint height = 0)
    {
        Name = name;
        Extension = extension;
        Width = width;
        Height = height;
    }

    public string GetNameWithExtension()
    {
        return $"{Name}{Extension}";
    }
}
