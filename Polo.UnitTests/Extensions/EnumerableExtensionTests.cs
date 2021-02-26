using FluentAssertions;
using Polo.Extensions;
using Xunit;

namespace Polo.UnitTests.Extensions
{
    public class EnumerableExtensionTests
    {
        [Fact]
        public void IsNullOrEmpty_Should_Return_True_If_Collection_Is_Null_Test()
        {
            // Arrange & Act
            var result = ((string[])null).IsNullOrEmpty();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_Should_Return_True_If_Collection_Is_Empty_Test()
        {
            // Arrange
            string[] sut = { };

            // Act
            var result = sut.IsNullOrEmpty();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_Should_Return_False_If_Collection_Is_Not_Empty_Test()
        {
            // Arrange
            string[] sut = { "hello" };

            // Act
            var result = sut.IsNullOrEmpty();

            // Assert
            result.Should().BeFalse();
        }
    }
}
