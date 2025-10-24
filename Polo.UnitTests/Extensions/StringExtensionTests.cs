using FluentAssertions;
using ImageMagick;
using Polo.Abstractions.Exceptions;
using Polo.Extensions;
using Xunit;

namespace Polo.UnitTests.Extensions;

public class StringExtensionTests
{
    [Theory]
    [InlineData("top-left", Gravity.Northwest)]
    [InlineData("top-right", Gravity.Northeast)]
    [InlineData("top-center", Gravity.North)]
    [InlineData("center-left", Gravity.West)]
    [InlineData("center-right", Gravity.East)]
    [InlineData("center-center", Gravity.Center)]
    [InlineData("bottom-left", Gravity.Southwest)]
    [InlineData("bottom-right", Gravity.Southeast)]
    [InlineData("bottom-center", Gravity.South)]
    public void ParsePosition_Should_Parse_Test(string position, Gravity expected)
    {
        // Arrange
        // Act
        var result = position.ParsePosition();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ParsePosition_Should_Throw_Exception_With_Incorrect_Values_Test()
    {
        // Arrange
        var position = "bottom-bottom";

        // Act
        var exception = Assert.Throws<ParameterParseException>(() => position.ParsePosition());

        // Assert
        exception.Message.Should().BeEquivalentTo($"ERROR: Invalid parameter value '{position}'. Incorrect position.");
    }

    [Fact]
    public void ParsePosition_Should_Throw_Exception_Without_Delimiter_Test()
    {
        // Arrange
        var position = "bottom";

        // Act
        var exception = Assert.Throws<ParameterParseException>(() => position.ParsePosition());

        // Assert
        exception.Message.Should().BeEquivalentTo($"ERROR: Invalid parameter value '{position}'. Should be only single delimiter '{CommandParser.ShortCommandPrefix}'.");
    }

    [Fact]
    public void ParsePosition_Should_Throw_Exception_With_Several_Delimiters_Test()
    {
        // Arrange
        var position = "bottom-left-center";

        // Act
        var exception = Assert.Throws<ParameterParseException>(() => position.ParsePosition());

        // Assert
        exception.Message.Should().BeEquivalentTo($"ERROR: Invalid parameter value '{position}'. Should be only single delimiter '{CommandParser.ShortCommandPrefix}'.");
    }

    [Fact]
    public void ParsePosition_Should_Throw_Exception_With_Invalid_Values_Test()
    {
        // Arrange
        var position = "up-down";

        // Act
        var exception = Assert.Throws<ParameterParseException>(() => position.ParsePosition());

        // Assert
        exception.Message.Should().BeEquivalentTo($"ERROR: Invalid parameter value '{position}'. Unsupported position name.");
    }

    [Theory]
    [InlineData(".jpg", MagickFormat.Jpg)]
    [InlineData(".jpeg", MagickFormat.Jpeg)]
    [InlineData(".png", MagickFormat.Png)]
    [InlineData(".bmp", MagickFormat.Bmp)]
    [InlineData(".gif", MagickFormat.Gif)]
    [InlineData(".svg", MagickFormat.Svg)]
    [InlineData(".tiff", MagickFormat.Tiff)]
    [InlineData(".webp", MagickFormat.WebP)]
    // New extensions for raw formats:
    [InlineData(".orf", MagickFormat.Orf)]
    [InlineData(".crw", MagickFormat.Crw)]
    [InlineData(".cr2", MagickFormat.Cr2)]
    [InlineData(".cr3", MagickFormat.Cr3)]
    [InlineData(".3fr", MagickFormat.ThreeFr)]
    [InlineData(".mef", MagickFormat.Mef)]
    [InlineData(".nef", MagickFormat.Nef)]
    [InlineData(".nrw", MagickFormat.Nrw)]
    [InlineData(".pef", MagickFormat.Pef)]
    [InlineData(".ptx", MagickFormat.Unknown)]
    [InlineData(".rw2", MagickFormat.Rw2)]
    [InlineData(".arw", MagickFormat.Arw)]
    [InlineData(".srf", MagickFormat.Srf)]
    [InlineData(".sr2", MagickFormat.Sr2)]
    [InlineData(".gpr", MagickFormat.Unknown)]
    [InlineData(".raf", MagickFormat.Raf)]
    [InlineData(".raw", MagickFormat.Raw)]
    [InlineData(".rwl", MagickFormat.Rwl)]
    [InlineData(".dng", MagickFormat.Dng)]
    [InlineData(".srw", MagickFormat.Srw)]
    [InlineData(".x3f", MagickFormat.X3f)]
    // Video file extensions
    [InlineData(".avi", MagickFormat.Avi)]
    [InlineData(".mp4", MagickFormat.Mp4)]
    [InlineData(".mpg", MagickFormat.Mpg)]
    [InlineData(".mpeg", MagickFormat.Mpeg)]
    [InlineData(".mov", MagickFormat.Mov)]
    [InlineData(".mkv", MagickFormat.Mkv)]
    [InlineData(".m2ts", MagickFormat.Unknown)]
    [InlineData(".ts", MagickFormat.Unknown)]
    [InlineData(".m4v", MagickFormat.M4v)]
    [InlineData(".3gp", MagickFormat.ThreeGp)]
    public void GetMagickFormatByFileExtension_Should_Return_Correct_Value_Test(string fileExtension, MagickFormat expectedMagickFormat)
    {
        // Arrange & Act
        var actual = fileExtension.GetMagickFormatByFileExtension();

        // Assert
        actual.Should().Be(expectedMagickFormat);
    }

}
