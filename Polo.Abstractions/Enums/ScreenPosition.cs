namespace Polo.Abstractions.Enums
{
    [Flags]
    public enum ScreenPosition : short
    {
        None = 0,
        Top = 1,
        Right = 2,
        Bottom = 4,
        Left = 8,
        Center = 16
    }
}