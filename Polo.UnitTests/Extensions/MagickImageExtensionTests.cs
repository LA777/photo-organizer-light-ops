using FluentAssertions;
using ImageMagick;
using Polo.Extensions;
using System;
using Xunit;

namespace Polo.UnitTests.Extensions
{
    public class MagickImageExtensionTests
    {
        [Fact]
        public void ConvertToTransparentMagickImage_Should_Convert_Succesfully_Test()
        {
            // Arrange
            var readSettings = new MagickReadSettings
            {
                BackgroundColor = MagickColors.White,
                Height = 80,
                Width = 140
            };
            using var magickImage = new MagickImage("caption:white.png", readSettings);
            const int transparencyPercent = 10;
            const int expectedDelta = 1000;
            using var pixels = magickImage.GetPixels();
            var color = pixels.GetPixel(0, 0).ToColor();

            // Act
            using var result = magickImage.ConvertToTransparentMagickImage(transparencyPercent);

            // Assert
            using var resultPixels = result.GetPixels();
            var resultColor = resultPixels.GetPixel(0, 0).ToColor();
            Math.Abs(resultColor.R - color.R).Should().BeLessThan(expectedDelta);
            Math.Abs(resultColor.G - color.G).Should().BeLessThan(expectedDelta);
            Math.Abs(resultColor.B - color.B).Should().BeLessThan(expectedDelta);
            Math.Abs(resultColor.A - (ushort.MaxValue / transparencyPercent)).Should().BeLessThan(expectedDelta);
        }
    }
}
