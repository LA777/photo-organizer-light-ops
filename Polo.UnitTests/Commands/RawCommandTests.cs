using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Polo.Abstractions.Commands;
using Polo.Commands;
using Polo.Options;
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
    public class RawCommandTests : CommandTestBase
    {
        private static readonly string _albumName = "Album1";
        private static readonly string _rawFolderName = "RAW";
        private static readonly IEnumerable<string> _rawFileExtensions = new List<string>() { "orf", "crw", "cr2", "cr3", "3fr", "mef", "nef", "nrw", "pef", "ptx", "rw2", "arw", "srf", "sr2", "gpr", "raf", "raw", "rwl", "dng", "srw", "x3f" };
        private static readonly IEnumerable<string> _jpegFileExtensions = new List<string>() { "jpg", "jpeg" };
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings(jpegFileExtensions: _jpegFileExtensions, rawFileExtensions: _rawFileExtensions, rawFolderName: _rawFolderName);
        private static readonly IOptions<ApplicationSettings> _mockApplicationOptions = Microsoft.Extensions.Options.Options.Create(_validApplicationSettings);
        private static readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly ICommand _sut = new RawCommand(_mockApplicationOptions, _loggerMock.Object);


        private readonly Folder folderStructureInitial = new Folder()
        {
            SubFolders = new List<Folder>()
            {
                new Folder()
                {
                    Name = _albumName,
                    Files = new List<string>()
                    {
                        "UTP-1.ORF",
                        "UTP-2.ORF",
                        "UTP-3.ORF",
                        "UTP-1.jpg",
                        "UTP-2.JPEG",
                        "UTP-3.JPG"
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
                    Files = new List<string>()
                    {
                        "UTP-1.jpg",
                        "UTP-2.JPEG",
                        "UTP-3.JPG"
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _rawFolderName,
                            Files = new List<string>()
                            {
                                "UTP-1.ORF",
                                "UTP-2.ORF",
                                "UTP-3.ORF"
                            }
                        }

                    }
                }
            }
        };

        public RawCommandTests()
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
                .ForEach(x =>
                {
                    x.Files.AddRange(rawfiles);
                    x.Files.AddRange(jpgfiles);
                });

            folderStructureExpected.SubFolders.Where(x => x.Name == _albumName).ToList()
                .ForEach(x =>
                {
                    x.Files.AddRange(jpgfiles);
                    x.SubFolders.Where(x => x.Name == _rawFolderName).ToList().ForEach(x => x.Files.AddRange(rawfiles));
                });
        }

        [Fact]
        public void Action_Should_Create_Raw_Folder_And_Move_Raw_Files_Test()
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

        ~RawCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
