using FluentAssertions;
using Polo.Comparers;
using Xunit;

namespace Polo.UnitTests.Comparers
{
    public class FileNameComparerTests
    {
        private readonly FileNameComparer _sut = new();

        [Theory]
        [InlineData(@"C:\Photo\Album1\img1111.jpg", @"C:\Photo\Album1\RAW\img1111.jpg")]
        [InlineData(@"C:\Photo\Album1\img1111.jpg", @"C:\Photo\Album1\img1111.jpg")]
        [InlineData(@"img1111.jpg", @"C:\Photo\Album1\img1111.jpg")]
        [InlineData(@"img1111.jpg", @"C:\Photo\Album1\img1111.JPG")]
        public void Equals_Should_Return_True_If_FileNames_Are_Equal_Test(string x, string y)
        {
            // Act
            var result = _sut.Equals(x, y);

            // Assert
            result.Should().BeTrue();
        }


        [Theory]
        [InlineData(@"C:\Photo\Album1\img1111.jpg", @"C:\Photo\Album1\RAW\img1111.ORF")]
        [InlineData(@"C:\Photo\Album1\img1111.jpg", @"C:\Photo\Album1\img1111.ORF")]
        [InlineData(@"C:\Photo\Album12\img1111.jpg", @"C:\Photo\Album1\img1111.ORF")]
        [InlineData(@"C:\Photo\Album12\IMG1111.jpg", @"C:\Photo\Album1\img1111.ORF")]
        [InlineData(@"C:\Photo\Album12\їхали1.jpg", @"C:\Photo\Album1\Їхали1.ORF")]
        public void Equals_Should_Return_True_If_FileNames_Are_Not_Equal_Test(string x, string y)
        {
            // Act
            var result = _sut.Equals(x, y);

            // Assert
            result.Should().BeFalse();
        }
    }
}
