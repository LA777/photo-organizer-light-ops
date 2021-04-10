using FluentAssertions;
using Moq;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Commands;
using Polo.Extensions;
using Polo.UnitTests.FileUtils;
using Polo.UnitTests.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Xunit;

namespace Polo.UnitTests.Commands
{
    [Collection("Sequential")]
    public class AddWatermarkCommandTests : CommandTestBase
    {
        private static readonly ICollection<string> _jpegFileExtensions = new List<string>() { "jpeg", "jpg" };
        private static readonly string _watermarkOutputFolderName = "watermark-output";
        private static readonly string _watermarkFolderName = "sign";
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings()
        {
            FileForProcessExtensions = _jpegFileExtensions,
            WatermarkPath = "",
            WatermarkOutputFolderName = _watermarkOutputFolderName,
            WatermarkPosition = "center-center",
            WatermarkTransparencyPercent = 20
        };

        private static readonly string _albumName = "Album1";
        private static readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly ICommand _sut = new AddWatermarkCommand(GetOptions(_validApplicationSettings), _loggerMock.Object);

        private readonly Folder _folderStructureInitial = new Folder()
        {
            SubFolders = new List<Folder>()
            {
                new Folder()
                {
                    Name = _albumName,
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("video-1", "mp4"),
                        new FotoFile("UTP-1", "ORF"),
                        new FotoFile("UTP-1", "jpg", 120, 90),
                        new FotoFile("UTP-2", "jpeg", 200, 100)
                    }
                },
                new Folder()
                {
                    Name = _watermarkFolderName,
                    Files = new List<FotoFile>()
                    {
                        FileHelper.Watermark
                    }
                }
            }
        };

        private readonly Folder _folderStructureExpected = new Folder()
        {
            SubFolders = new List<Folder>()
            {
                new Folder()
                {
                    Name = _albumName,
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("video-1", "mp4"),
                        new FotoFile("UTP-1", "ORF"),
                        new FotoFile("UTP-1", "jpg", 120, 90),
                        new FotoFile("UTP-2", "jpeg", 200, 100)
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _watermarkOutputFolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-1", "jpg", 120, 90),
                                new FotoFile("UTP-2", "jpeg", 200, 100)
                            }
                        }
                    }
                },
                new Folder()
                {
                    Name = _watermarkFolderName,
                    Files = new List<FotoFile>()
                    {
                        FileHelper.Watermark
                    }
                }
            }
        };

        [Fact]
        public void Action_Should_Add_Watermark_And_Copy_To_Output_Folder_Test()
        {
            // TODO LA - Complete
            // TODO LA - Check transparency watermark

            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            Environment.CurrentDirectory = @"d:\tmp dev\images\";//Path.Combine(testFolderFullPath, _albumName);

            // TODO LA - Refactor
            _validApplicationSettings.WatermarkPath = @"d:\tmp dev\images\sign\white.png";//Path.Combine(testFolderFullPath, _watermarkFolderName, FileHelper.Watermark.GetNameWithExtension());
            var sut = new AddWatermarkCommand(GetOptions(_validApplicationSettings), _loggerMock.Object);

            // Act
            sut.Action();

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);

            // Get output file path
            //var outputFolder = _folderStructureExpected.SubFolders.First(x => x.Name == _albumName)
            //    .SubFolders.First(y => y.Name == _watermarkOutputFolderName);

            //var watermarkPath = Path.Combine(testFolderFullPath, _watermarkFolderName, FileHelper.Watermark.GetNameWithExtension());
            //var watemarkImage = new Bitmap(watermarkPath);


            //foreach (var file in outputFolder.Files)
            //{
            //    var filePath = Path.Combine(testFolderFullPath, _albumName, _watermarkOutputFolderName, file.GetNameWithExtension());
            //    // Get byte array by path
            //    var image = new Bitmap(filePath);
            //    var pixels = GetWatermarkPixels(filePath, _validApplicationSettings.WatermarkPosition, FileHelper.Watermark);



            //    var img = Image.FromFile(watermarkPath);
            //    var opBmp = ChangeOpacity(img, 0.2f);

            //    var pixel = watemarkImage.GetPixel(0, 0);
            //    var pixel1 = pixels[0];
            //    var pixelOp = opBmp.GetPixel(0, 0);

            //    var resultPercent1 = CompareImagesForDifference1(watemarkImage, opBmp);
            //    var resultPercent2 = CompareImagesForDifference(opBmp, pixels);

            //    var resultPercent = CompareImagesForDifference(watemarkImage, pixels);
            //    float expectedDifference = 0.1f;
            //    resultPercent.Should().BeLessThan(expectedDifference);
            //}
        }

        private Bitmap ChangeOpacity(Image img, float opacityValue)
        {
            var bmp = new Bitmap(img.Width, img.Height);
            using var graphics = Graphics.FromImage(bmp);
            var colormatrix = new ColorMatrix
            {
                Matrix33 = opacityValue
            };
            var imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            //graphics.Dispose();   // Releasing all resource used by graphics
            return bmp;
        }

        private float CompareImagesForDifference(Bitmap imageA, List<Color> pixelsB)
        {
            float difference = 0;
            for (int x = 0, i = 0; x < imageA.Width; x++)
            {
                for (int y = 0; y < imageA.Height; y++, i++)
                {
                    Color pixelA = imageA.GetPixel(x, y);
                    Color pixelB = pixelsB[i];

                    difference += Math.Abs(pixelA.A - pixelB.A);
                    difference += Math.Abs(pixelA.R - pixelB.R);
                    difference += Math.Abs(pixelA.G - pixelB.G);
                    difference += Math.Abs(pixelA.B - pixelB.B);
                }
            }

            var result = 100 * (difference / 255) / (pixelsB.Count * 4);

            return result;
        }

        private float CompareImagesForDifference1(Bitmap imageA, Bitmap imageB)
        {
            float difference = 0;
            for (int x = 0; x < imageA.Width; x++)
            {
                for (int y = 0; y < imageA.Height; y++)
                {
                    Color pixelA = imageA.GetPixel(x, y);
                    Color pixelB = imageB.GetPixel(x, y);

                    difference += Math.Abs(pixelA.A - pixelB.A);
                    difference += Math.Abs(pixelA.R - pixelB.R);
                    difference += Math.Abs(pixelA.G - pixelB.G);
                    difference += Math.Abs(pixelA.B - pixelB.B);
                }
            }

            var result = 100 * (difference / 255) / (imageA.Width * imageA.Height * 4);

            return result;
        }

        // TODO LA - Complete
        private List<Color> GetWatermarkPixels(string filePath, string position, FotoFile watermark)
        {
            // X - width
            // Y - height
            var image = new Bitmap(filePath);
            var width = image.Width;
            var height = image.Height;
            var wWidth = watermark.Width;
            var wHeight = watermark.Height;
            var pos = position.ParsePosition();
            var p1x = 0;
            var p1y = 0;
            var p4x = 0;
            var p4y = 0;

            switch (pos)
            {
                case ImageMagick.Gravity.Undefined:
                    throw new Exception();
                case ImageMagick.Gravity.Northwest:
                    p1x = 0;
                    p1y = 0;
                    p4x = wWidth;
                    p4y = wHeight;
                    break;
                case ImageMagick.Gravity.North:
                    p1x = (width / 2) - (wWidth / 2);
                    p1y = 0;
                    p4x = (width / 2) + (wWidth / 2);
                    p4y = wHeight;
                    break;
                case ImageMagick.Gravity.Northeast:
                    p1x = width - wWidth;
                    p1y = 0;
                    p4x = width;
                    p4y = wHeight;
                    break;
                case ImageMagick.Gravity.West:
                    p1x = 0;
                    p1y = (height / 2) - (wHeight / 2);
                    p4x = wWidth;
                    p4y = (height / 2) + (wHeight / 2);
                    break;
                case ImageMagick.Gravity.Center:
                    p1x = (width / 2) - (wWidth / 2);
                    p1y = (height / 2) - (wHeight / 2);
                    p4x = (width / 2) + (wWidth / 2);
                    p4y = (height / 2) + (wHeight / 2);
                    break;
                case ImageMagick.Gravity.East:
                    p1x = width - wWidth;
                    p1y = (height / 2) - (wHeight / 2);
                    p4x = width;
                    p4y = (height / 2) + (wHeight / 2);
                    break;
                case ImageMagick.Gravity.Southwest:
                    p1x = 0;
                    p1y = height - wHeight;
                    p4x = wWidth;
                    p4y = height;
                    break;
                case ImageMagick.Gravity.South:
                    p1x = (width / 2) - (wWidth / 2);
                    p1y = height - wHeight;
                    p4x = (width / 2) + (wWidth / 2);
                    p4y = height;
                    break;
                case ImageMagick.Gravity.Southeast:
                    p1x = width - wWidth;
                    p1y = height - wHeight;
                    p4x = width;
                    p4y = height;
                    break;
                default:
                    throw new Exception();
            }

            var wPixels = GetPixels(new Point(p1x, p1y), new Point(p4x, p4y), wWidth, image);

            return wPixels;
        }

        private List<Color> GetPixels(Point p1, Point p4, int size, Bitmap image)
        {
            var result = new List<Color>();
            for (int x = p1.X; x < p4.X; x++)
            {
                for (int y = p1.Y; y < p4.Y; y++)
                {
                    result.Add(image.GetPixel(x, y));
                }
            }

            return result;
        }

        ~AddWatermarkCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
