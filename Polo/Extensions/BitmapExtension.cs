using ImageMagick;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Polo.Extensions
{
    public static class BitmapExtension
    {
        public static MagickImage ConvertToTransparentMagickImage1(this Bitmap bitmap, int transparencyPercent)
        {
            var transparentBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            using var graphics = Graphics.FromImage(transparentBitmap);
            float transparency = transparencyPercent / 100;
            var colormatrix = new ColorMatrix
            {
                Matrix33 = transparency
            };
            var imageAttribute = new ImageAttributes();
            imageAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(bitmap, new Rectangle(0, 0, transparentBitmap.Width, transparentBitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttribute);

            using var memoryStream = new MemoryStream();
            transparentBitmap.Save(memoryStream, bitmap.RawFormat);
            memoryStream.Position = 0;
            // var magickImage = new MagickImage(memoryStream, watermark.Format);
            var magickImage = new MagickImage(memoryStream);

            return magickImage;
        }

        public static MagickImage ConvertToTransparentMagickImage(this MagickImage magickImage, int transparencyPercent)
        {
            var transparentBitmap = new Bitmap(magickImage.Width, magickImage.Height);
            using var graphics = Graphics.FromImage(transparentBitmap);
            float transparency = transparencyPercent / 100;
            var colormatrix = new ColorMatrix
            {
                Matrix33 = transparency
            };
            var imageAttribute = new ImageAttributes();
            imageAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(magickImage.ToBitmap(), new Rectangle(0, 0, transparentBitmap.Width, transparentBitmap.Height), 0, 0, magickImage.Width, magickImage.Height, GraphicsUnit.Pixel, imageAttribute);

            using var memoryStream = new MemoryStream();
            transparentBitmap.Save(memoryStream, ImageFormat.Bmp);
            memoryStream.Position = 0;
            var magickImageOutput = new MagickImage(memoryStream, magickImage.Format);

            return magickImageOutput;
        }
    }
}
