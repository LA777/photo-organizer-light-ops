using FluentAssertions;
using ImageMagick;
using Polo.Abstractions.Exceptions;
using Polo.Extensions;
using Xunit;

namespace Polo.UnitTests.Extensions
{
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
    }
}
