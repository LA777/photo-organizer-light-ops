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
using Xunit;

namespace Polo.UnitTests.Commands
{
    [Collection("Sequential")]
    public class MoveRawToJpegFolderCommandTests : CommandTestBase
    {
        private static readonly string _albumName = "Album1";
        private static readonly string _rawFolderName = "RAW";
        private static readonly ICollection<string> _rawFileExtensions = new List<string>() { "orf", "crw", "cr2", "cr3", "3fr", "mef", "nef", "nrw", "pef", "ptx", "rw2", "arw", "srf", "sr2", "gpr", "raf", "raw", "rwl", "dng", "srw", "x3f" };
        private static readonly ICollection<string> _jpegFileExtensions = new List<string>() { "jpg", "jpeg" };
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings()
        {
            FileForProcessExtensions = _jpegFileExtensions,
            RawFileExtensions = _rawFileExtensions,
            RawFolderName = _rawFolderName
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
                            Name = _rawFolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-1", "ORF"),
                                new FotoFile("UTP-2", "ORF"),
                                new FotoFile("UTP-3", "ORF"),
                                new FotoFile("UTP-4", "ORF"),
                                new FotoFile("UTP-5", "ORF"),
                                new FotoFile("UTP-6", "ORF")
                            }
                        }
                    }
                },
                new Folder()
                {
                    Name = "Fotos 1",
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-1", "jpg"),
                        new FotoFile("UTP-2", "jpeg")
                    }
                },
                new Folder()
                {
                    Name = "Fotos 2",
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-3", "jpg"),
                        new FotoFile("UTP-4", "jpeg")
                    }
                },
                new Folder()
                {
                    Name = "Fotos 3",
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-5", "jpg"),
                        new FotoFile("UTP-6", "jpeg")
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
                            Name = _rawFolderName
                        }
                    }
                },
                new Folder()
                {
                    Name = "Fotos 1",
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-1", "jpg"),
                        new FotoFile("UTP-2", "jpeg")
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _rawFolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-1", "ORF"),
                                new FotoFile("UTP-2", "ORF")
                            }
                        }
                    }
                },
                new Folder()
                {
                    Name = "Fotos 2",
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-3", "jpg"),
                        new FotoFile("UTP-4", "jpeg")
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _rawFolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-3", "ORF"),
                                new FotoFile("UTP-4", "ORF")
                            }
                        }
                    }
                },
                new Folder()
                {
                    Name = "Fotos 3",
                    Files = new List<FotoFile>()
                    {
                        new FotoFile("UTP-5", "jpg"),
                        new FotoFile("UTP-6", "jpeg")
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _rawFolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-5", "ORF"),
                                new FotoFile("UTP-6", "ORF")
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

            foreach (var extension in _rawFileExtensions)
            {
                var rawFile = new FotoFile($"image-{extension}", extension);
                var jpgFile = new FotoFile($"image-{extension}", "jpg");
                rawFiles.Add(rawFile);
                jpgFiles.Add(jpgFile);
            }

            _folderStructureInitial.SubFolders.Where(x => x.Name == _albumName).ToList()
                .ForEach(x => x.SubFolders.Where(folder => folder.Name == _rawFolderName).ToList()
                    .ForEach(folder => folder.Files.AddRange(rawFiles)));
            _folderStructureInitial.SubFolders.Where(x => x.Name == "Fotos 1").ToList()
                .ForEach(x => x.Files.AddRange(jpgFiles));

            _folderStructureExpected.SubFolders.Where(x => x.Name == "Fotos 1").ToList()
                .ForEach(x =>
                {
                    x.Files.AddRange(jpgFiles);
                    x.SubFolders.Where(folder => folder.Name == _rawFolderName).ToList().ForEach(folder => folder.Files.AddRange(rawFiles));
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
            folderStructureActual.Should().BeEquivalentTo(_folderStructureExpected);
        }

        [Fact]
        public void Action_Should_Move_Raw_Files_To_Jpeg_Folder_If_Setting_Have_Duplicate_JpegFileExtension_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

            var jpegFileExtensionsWithDuplicates = new List<string>() { "jpeg", "jpg", "jpeg", "jpg" };
            var applicationSettings = new ApplicationSettings() { FileForProcessExtensions = jpegFileExtensionsWithDuplicates, RawFileExtensions = _rawFileExtensions, RawFolderName = _rawFolderName };
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

            var rawFileExtensionsWithDuplicates = new List<string>() { "orf", "crw", "cr2", "cr3", "3fr", "mef", "nef", "nrw", "pef", "ptx", "rw2", "arw", "srf", "sr2", "gpr", "raf", "raw", "rwl", "dng", "srw", "x3f",
                                                                       "orf", "crw", "cr2", "cr3", "3fr", "mef", "nef", "nrw", "pef", "ptx", "rw2", "arw", "srf", "sr2", "gpr", "raf", "raw", "rwl", "dng", "srw", "x3f" };
            var applicationSettings = new ApplicationSettings()
            {
                FileForProcessExtensions = _jpegFileExtensions,
                RawFileExtensions = rawFileExtensionsWithDuplicates,
                RawFolderName = _rawFolderName
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
