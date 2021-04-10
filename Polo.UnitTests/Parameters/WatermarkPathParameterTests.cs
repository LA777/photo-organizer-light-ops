using FluentAssertions;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters;
using Polo.Parameters;
using Polo.UnitTests.Commands;
using Polo.UnitTests.FileUtils;
using Polo.UnitTests.Models;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Polo.UnitTests.Parameters
{
    [Collection("Sequential")]
    public class WatermarkPathParameterTests : CommandTestBase
    {
        private readonly IParameter<string> _sut = new WatermarkPathParameter();
        private static readonly FotoFile _watermarkFile = new FotoFile("watermark", "png");
        private const string WatermarkFolderName = "watermark";

        private readonly Folder _folderStructureInitial = new Folder()
        {
            SubFolders = new List<Folder>()
            {
                new Folder()
                {
                    Name = WatermarkFolderName,
                    Files = new List<FotoFile>()
                    {
                        _watermarkFile
                    }
                }
            }
        };

        [Fact]
        public void Initialize_Should_Return_Input_Parameter_Test()
        {
            // Arrange
            FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var inputParameter = _watermarkFile.FullFilePath;
            const string defaultValue = nameof(defaultValue);
            var inputParameters = new Dictionary<string, string>
            {
                { WatermarkPathParameter.Name, inputParameter }
            };

            // Act
            var result = _sut.Initialize(inputParameters, defaultValue);

            // Assert
            result.Should().Be(inputParameter);
        }

        [Fact]
        public void Initialize_Should_Return_Default_Parameter_Test()
        {
            // Arrange
            FileHelper.CreateFoldersAndFilesByStructure(_folderStructureInitial);
            var defaultValue = _watermarkFile.FullFilePath;
            var inputParameters = new Dictionary<string, string>();

            // Act
            var result = _sut.Initialize(inputParameters, defaultValue);

            // Assert
            result.Should().Be(defaultValue);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void Initialize_Should_Throw_ParameterAbsentException_If_Input_Parameter_Is_Null_Or_WhiteSpace_Test(string parameterValue)
        {
            // Arrange
            const string defaultValue = nameof(defaultValue);
            var inputParameters = new Dictionary<string, string>
            {
                { WatermarkPathParameter.Name, parameterValue }
            };

            // Act
            var exception = Assert.Throws<ParameterAbsentException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{WatermarkPathParameter.Name}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.WatermarkPath)}'.", exception.Message);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void Initialize_Should_Throw_ParameterAbsentException_If_Default_Parameter_Is_Null_Or_WhiteSpace_Test(string parameterValue)
        {
            // Arrange
            var inputParameters = new Dictionary<string, string>();

            // Act
            var exception = Assert.Throws<ParameterAbsentException>(() => _sut.Initialize(inputParameters, parameterValue));

            // Assert
            Assert.Contains($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{WatermarkPathParameter.Name}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.WatermarkPath)}'.", exception.Message);
        }

        [Fact]
        public void Initialize_Should_Throw_FileNotFoundException_If_Input_Parameter_Is_NonValid_Path_Test()
        {
            // Arrange
            const string defaultValue = nameof(defaultValue);
            const string inputParameter = nameof(inputParameter);
            var inputParameters = new Dictionary<string, string>
            {
                { SourceParameter.Name, inputParameter }
            };

            // Act
            var exception = Assert.Throws<FileNotFoundException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains("ERROR: Watermark file not found.", exception.Message);
        }

        [Fact]
        public void Initialize_Should_Throw_FileNotFoundException_If_Default_Parameter_Is_NonValid_Path_Test()
        {
            // Arrange
            const string defaultValue = nameof(defaultValue);
            var inputParameters = new Dictionary<string, string>();

            // Act
            var exception = Assert.Throws<FileNotFoundException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains("ERROR: Watermark file not found.", exception.Message);
        }
    }
}
