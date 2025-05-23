namespace Polo.UnitTests.Models;

public class TextFile
{
    public string Name { get; set; }
    public string Extension { get; set; }
    public string Text { get; set; }

    public TextFile(string name, string extension, string text)
    {
        Name = name;
        Extension = extension;
        Text = text;
    }

    public string GetNameWithExtension()
    {
        return $"{Name}{Extension}";
    }
}
