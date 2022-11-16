using ImageMagick;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace Polo.Extensions
{
    public static class MagickImageExtension
    {
        [SupportedOSPlatform("windows")]
        public static MagickImage ConvertToTransparentMagickImage(this MagickImage magickImage, int transparencyPercent)
        {
            var transparentBitmap = new Bitmap(magickImage.Width, magickImage.Height);
            using var graphics = Graphics.FromImage(transparentBitmap);
            var transparency = (float)transparencyPercent / 100;
            var colorMatrix = new ColorMatrix
            {
                Matrix33 = transparency
            };
            var imageAttribute = new ImageAttributes();
            imageAttribute.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(magickImage.ToBitmap(), new Rectangle(0, 0, transparentBitmap.Width, transparentBitmap.Height), 0, 0, magickImage.Width, magickImage.Height, GraphicsUnit.Pixel, imageAttribute);

            using var memoryStream = new MemoryStream();
            transparentBitmap.Save(memoryStream, ImageFormat.Bmp);
            memoryStream.Position = 0;
            var magickImageOutput = new MagickImage(memoryStream, MagickFormat.Bmp);

            return magickImageOutput;
        }

        public static void ResizeByMegapixelsLimit(this MagickImage magickImage, float megaPixelsLimit, MagickImageInfo magickImageInfo)
        {
            var aspectRatio = (double)magickImageInfo.Width / magickImageInfo.Height;
            var diff = megaPixelsLimit * 1000000 * aspectRatio;
            var expectedWidth = (int)Math.Floor(Math.Sqrt(diff));

            if (expectedWidth % 2 != 0)
            {
                --expectedWidth;
            }

            magickImage.Resize(expectedWidth, 0);
        }

        public static void ResizeByLongSide(this MagickImage magickImage, int longSideLimit, MagickImageInfo magickImageInfo)
        {
            var width = magickImageInfo.Width;
            var height = magickImageInfo.Height;

            if (width >= height && width > longSideLimit)
            {
                magickImage.Resize(longSideLimit, 0);
            }
            else if (height > width && height > longSideLimit)
            {
                magickImage.Resize(0, longSideLimit);
            }
        }
    }
}