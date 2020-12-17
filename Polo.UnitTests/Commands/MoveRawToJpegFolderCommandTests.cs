using FluentAssertions;
using Moq;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Services;
using Polo.Commands;
using Polo.UnitTests.FileUtils;
using Polo.UnitTests.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Polo.UnitTests.Commands
{
    [Collection("Sequential")]
    public class MoveRawToJpegFolderCommandTests : CommandTestBase
    {
        private readonly ICommand _sut;

        private readonly Folder folderStructureInitial = new Folder()
        {
            SubFolders = new List<Folder>()
            {
                new Folder()
                {
                    Name = "New Fotos",
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = "RAW",
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
                        "UTP-2.jpg"
                    }
                },
                new Folder()
                {
                    Name = "Fotos 2",
                    Files = new List<string>()
                    {
                        "UTP-3.jpg",
                        "UTP-4.jpg"
                    }
                },
                new Folder()
                {
                    Name = "Fotos 3",
                    Files = new List<string>()
                    {
                        "UTP-5.jpg",
                        "UTP-6.jpg"
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
                    Name = "New Fotos",
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = "RAW"
                        }
                    }
                },
                new Folder()
                {
                    Name = "Fotos 1",
                    Files = new List<string>()
                    {
                        "UTP-1.jpg",
                        "UTP-2.jpg"
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = "RAW",
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
                        "UTP-4.jpg"
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = "RAW",
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
                        "UTP-6.jpg"
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = "RAW",
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
            var consoleServiceMock = new Mock<IConsoleService>();
            _sut = new MoveRawToJpegFolderCommand(consoleServiceMock.Object);
        }

        [Fact]
        public void Action_Should_Move_Raw_Files_To_Jpeg_Folder_Test()
        {
            // Arrange
            var testFolderFullPath = FileHelper.CreateFoldersAndFilesByStructure(folderStructureInitial);
            Environment.CurrentDirectory = Path.Combine(testFolderFullPath, "New Fotos");

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
