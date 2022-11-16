using FluentAssertions;
using Moq;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Commands;
using Polo.UnitTests.FileUtils;
using Polo.UnitTests.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Polo.UnitTests.Commands
{
    [Collection("Sequential")]
    public class MoveRawToJpegFolderCommandTests : CommandTestBase
    {
        private static readonly string _albumName = "Album1";
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings()
        {
            FileForProcessExtensions = FileExtension.JpegExtensions,
            RawFileExtensions = FileExtension.RawExtensions,
            RawFolderName = Constants.RawFolderName
        };
        private static readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly ICommand _sut = new MoveRawToJpegFolderCommand(GetOptions(_validApplicationSettings), _loggerMock.Object);

        private readonly Folder _folderStructureInitial = new Folder()
        {
            SubFolders = new List<Folder>()
            {
                new Folder()
                {
                    Name = _albumName,
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = Constants.RawFolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-1",FileExtension.Orf),
                                new FotoFile("UTP-2",FileExtension.Orf),
                                new FotoFile("UTP-3",FileExtension.Orf),
                                new FotoFile("UTP-4",FileExtension.Orf),
                                new FotoFile("UTP-5",FileExtension.Orf),
                                new FotoFile("UTP-6",FileExtension.Orf)
                            }
                        }
                    }
                },
                new Folder()
                {
                    Name = "Fotos 1",
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-1", FileExtension.Jpg),
                        new FotoFile("UTP-2", FileExtension.Jpeg)
                    }
                },
                new Folder()
                {
                    Name = "Fotos 2",
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-3", FileExtension.Jpg),
                        new FotoFile("UTP-4", FileExtension.Jpeg)
                    }
                },
                new Folder()
                {
                    Name = "Fotos 3",
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-5", FileExtension.Jpg),
                        new FotoFile("UTP-6", FileExtension.Jpeg)
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
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = Constants.RawFolderName
                        }
                    }
                },
                new Folder()
                {
                    Name = "Fotos 1",
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-1", FileExtension.Jpg),
                        new FotoFile("UTP-2", FileExtension.Jpeg)
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = Constants.RawFolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-1",FileExtension.Orf),
                                new FotoFile("UTP-2",FileExtension.Orf)
                            }
                        }
                    }
                },
                new Folder()
                {
                    Name = "Fotos 2",
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-3", FileExtension.Jpg),
                        new FotoFile("UTP-4", FileExtension.Jpeg)
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = Constants.RawFolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-3", FileExtension.Orf),
                                new FotoFile("UTP-4", FileExtension.Orf)
                            }
                        }
                    }
                },
                new Folder()
                {
                    Name = "Fotos 3",
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-5", FileExtension.Jpg),
                        new FotoFile("UTP-6", FileExtension.Jpeg)
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = Constants.RawFolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-5", FileExtension.Orf),
                                new FotoFile("UTP-6", FileExtension.Orf)
                            }
                        }
                    }
                }
            }
        };

        public MoveRawToJpegFolderCommandTests()
        {
            AddRawFilesToStructure();
        }

        private void AddRawFilesToStructure()
        {
            var rawFiles = new List<FotoFile>();
            var jpgFiles = new List<FotoFile>();

            foreach (var extension in FileExtension.RawExtensions)
            {
                var rawFile = new FotoFile($"image-{extension.TrimStart('.')}", extension);
                var jpgFile = new FotoFile($"image-{extension.TrimStart('.')}", FileExtension.Jpg);
                rawFiles.Add(rawFile);
                jpgFiles.Add(jpgFile);
            }

            _folderStructureInitial.SubFolders.Where(x => x.Name == _albumName).ToList()
                .ForEach(x => x.SubFolders.Where(folder => folder.Name == Constants.RawFolderName).ToList()
                    .ForEach(folder => folder.Files.AddRange(rawFiles)));
            _folderStructureInitial.SubFolders.Where(x => x.Name == "Fotos 1").ToList()
                .ForEach(x => x.Files.AddRange(jpgFiles));

            _folderStructureExpected.SubFolders.Where(x => x.Name == "Fotos 1").ToList()
                .ForEach(x =>
                {
                    x.Files.AddRange(jpgFiles);
                    x.SubFolders.Where(folder => folder.Name == Constants.RawFolderName).ToList().ForEach(folder => folder.Files.AddRange(rawFiles));
                });
        }

        [Fact]
        public void Action_Should_Move_Raw_Files_To_Jpeg_Folder_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

            // Act
            _sut.Action();

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            var json1 = JsonSerializer.Serialize(_folderStructureExpected);
            var json2 = JsonSerializer.Serialize(folderStructureActual);





            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Move_Raw_Files_To_Jpeg_Folder_If_Setting_Have_Duplicate_JpegFileExtension_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

            var jpegFileExtensionsWithDuplicates = new List<string>() { FileExtension.Jpeg, FileExtension.Jpg, FileExtension.Jpeg, FileExtension.Jpg };
            var applicationSettings = new ApplicationSettings()
            {
                FileForProcessExtensions = jpegFileExtensionsWithDuplicates,
                RawFileExtensions = FileExtension.RawExtensions,
                RawFolderName = Constants.RawFolderName
            };
            var sut = new MoveRawToJpegFolderCommand(GetOptions(applicationSettings), _loggerMock.Object);

            // Act
            sut.Action();

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Move_Raw_Files_To_Jpeg_Folder_If_Setting_Have_Duplicate_RawFileExtension_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

            var rawFileExtensionsWithDuplicates = new List<string>();
            rawFileExtensionsWithDuplicates.AddRange(FileExtension.RawExtensions);
            rawFileExtensionsWithDuplicates.AddRange(FileExtension.RawExtensions);
            var applicationSettings = new ApplicationSettings()
            {
                FileForProcessExtensions = FileExtension.JpegExtensions,
                RawFileExtensions = rawFileExtensionsWithDuplicates,
                RawFolderName = Constants.RawFolderName
            };
            var sut = new MoveRawToJpegFolderCommand(GetOptions(applicationSettings), _loggerMock.Object);

            // Act
            sut.Action();

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        ~MoveRawToJpegFolderCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
