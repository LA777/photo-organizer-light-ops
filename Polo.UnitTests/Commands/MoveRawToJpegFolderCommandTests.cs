using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Services;
using Polo.Commands;
using Polo.Options;
using Polo.UnitTests.FileUtils;
using Polo.UnitTests.Models;
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
        private static readonly IEnumerable<string> _rawFileExtensions = new List<string>() { "orf", "crw", "cr2", "cr3", "3fr", "mef", "nef", "nrw", "pef", "ptx", "rw2", "arw", "srf", "sr2", "gpr", "raf", "raw", "rwl", "dng", "srw", "x3f" };
        private static readonly IEnumerable<string> _jpegFileExtensions = new List<string>() { "jpg", "jpeg" };
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings(jpegFileExtensions: _jpegFileExtensions, rawFileExtensions: _rawFileExtensions, rawFolderName: _rawFolderName);
        private static readonly IOptions<ApplicationSettings> _mockApplicationOptions = Microsoft.Extensions.Options.Options.Create(_validApplicationSettings);
        private static readonly Mock<IConsoleService> _consoleServiceMock = new Mock<IConsoleService>();
        private readonly ICommand _sut = new MoveRawToJpegFolderCommand(_mockApplicationOptions, _consoleServiceMock.Object);

        private readonly Folder folderStructureInitial = new Folder()
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
                            Files = new List<string>()
                            {
                                "UTP-1.ORF",
                                "UTP-2.ORF",
                                "UTP-3.ORF",
                                "UTP-4.ORF",
                                "UTP-5.ORF",
                                "UTP-6.ORF"
                            }
                        }
                    }
                },
                new Folder()
                {
                    Name = "Fotos 1",
                    Files = new List<string>()
                    {
                        "UTP-1.jpg",
                        "UTP-2.jpeg"
                    }
                },
                new Folder()
                {
                    Name = "Fotos 2",
                    Files = new List<string>()
                    {
                        "UTP-3.jpg",
                        "UTP-4.jpeg"
                    }
                },
                new Folder()
                {
                    Name = "Fotos 3",
                    Files = new List<string>()
                    {
                        "UTP-5.jpg",
                        "UTP-6.jpeg"
                    }
                }
            }
        };

        private readonly Folder folderStructureExpected = new Folder()
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
                    Files = new List<string>()
                    {
                        "UTP-1.jpg",
                        "UTP-2.jpeg"
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _rawFolderName,
                            Files = new List<string>()
                            {
                                "UTP-1.ORF",
                                "UTP-2.ORF"
                            }
                        }
                    }
                },
                new Folder()
                {
                    Name = "Fotos 2",
                    Files = new List<string>()
                    {
                        "UTP-3.jpg",
                        "UTP-4.jpeg"
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _rawFolderName,
                            Files = new List<string>()
                            {
                                "UTP-3.ORF",
                                "UTP-4.ORF"
                            }
                        }
                    }
                },
                new Folder()
                {
                    Name = "Fotos 3",
                    Files = new List<string>()
                    {
                        "UTP-5.jpg",
                        "UTP-6.jpeg"
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _rawFolderName,
                            Files = new List<string>()
                            {
                                "UTP-5.ORF",
                                "UTP-6.ORF"
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
            var rawfiles = new List<string>();
            var jpgfiles = new List<string>();
            foreach (var extension in _rawFileExtensions)
            {
                var rawFileName = $"image-{extension}.{extension}";
                var jpgFileName = $"image-{extension}.jpg";
                rawfiles.Add(rawFileName);
                jpgfiles.Add(jpgFileName);
            }

            folderStructureInitial.SubFolders.Where(x => x.Name == _albumName).ToList()
                .ForEach(x => x.SubFolders.Where(x => x.Name == _rawFolderName).ToList()
                    .ForEach(x => x.Files.AddRange(rawfiles)));
            folderStructureInitial.SubFolders.Where(x => x.Name == "Fotos 1").ToList()
                .ForEach(x => x.Files.AddRange(jpgfiles));

            folderStructureExpected.SubFolders.Where(x => x.Name == "Fotos 1").ToList()
                .ForEach(x =>
                {
                    x.Files.AddRange(jpgfiles);
                    x.SubFolders.Where(x => x.Name == _rawFolderName).ToList().ForEach(x => x.Files.AddRange(rawfiles));
                });
        }

        [Fact]
        public void Action_Should_Move_Raw_Files_To_Jpeg_Folder_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, _albumName);

            // Act
            _sut.Action();

            // Assert
            var folderStructureActual = FileHelper.CreateFolderStructureByFolderAndFiles(testFolderFullPath);
            folderStructureActual.Should().BeEquivalentTo(folderStructureExpected);
        }

        ~MoveRawToJpegFolderCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
