using Moq;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Options;
using Polo.Commands;
using Polo.UnitTests.Models;
using Serilog;
using System.Collections.Generic;
using Xunit;

namespace Polo.UnitTests.Commands
{
    [Collection("Sequential")]
    public class AddWatermarkCommandTests : CommandTestBase
    {
        private static readonly ICollection<string> _jpegFileExtensions = new List<string>() { "jpeg", "jpg" };
        private static readonly string _watermarkOutputFolderName = "watermark";
        private static readonly ApplicationSettings _validApplicationSettings = new ApplicationSettings()
        {
            JpegFileExtensions = _jpegFileExtensions,
            WatermarkOutputFolderName = _watermarkOutputFolderName
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
                        new FotoFile("UTP-1", "jpg", 90, 30),
                        new FotoFile("UTP-2", "jpeg", 30, 90),
                        new FotoFile("UTP-3", "jpeg", 120, 90),
                        new FotoFile("UTP-4", "jpeg", 90, 120),
                        new FotoFile("UTP-5", "jpeg", 120, 120),
                        new FotoFile("UTP-6", "jpeg", 100, 90),
                        new FotoFile("UTP-7", "jpeg", 90, 100),
                        new FotoFile("UTP-8", "jpeg", 100, 100)
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
                        new FotoFile("UTP-1", "jpg", 90, 30),
                        new FotoFile("UTP-2", "jpeg", 30, 90),
                        new FotoFile("UTP-3", "jpeg", 120, 90),
                        new FotoFile("UTP-4", "jpeg", 90, 120),
                        new FotoFile("UTP-5", "jpeg", 120, 120),
                        new FotoFile("UTP-6", "jpeg", 100, 90),
                        new FotoFile("UTP-7", "jpeg", 90, 100),
                        new FotoFile("UTP-8", "jpeg", 100, 100)
                    },
                    SubFolders = new List<Folder>()
                    {
                        new Folder()
                        {
                            Name = _watermarkOutputFolderName,
                            Files = new List<FotoFile>()
                            {
                                new FotoFile("UTP-1", "jpg", 90, 30),
                                new FotoFile("UTP-2", "jpeg", 30, 90),
                                new FotoFile("UTP-3", "jpeg", 100, 75),
                                new FotoFile("UTP-4", "jpeg", 75, 100),
                                new FotoFile("UTP-5", "jpeg", 100, 100),
                                new FotoFile("UTP-6", "jpeg", 100, 90),
                                new FotoFile("UTP-7", "jpeg", 90, 100),
                                new FotoFile("UTP-8", "jpeg", 100, 100)
                            }
                        }
                    }
                }
            }
        };

        // TODO LA - Complete



        ~AddWatermarkCommandTests()
        {
            ReleaseUnmanagedResources();
        }
    }
}
