using FluentAssertions;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Parameters;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Polo.UnitTests.Parameters
{
    public class DestinationParameterTests
    {
        private readonly IParameter<string> _sut = new DestinationParameter();
        private readonly string _validFolderPath = Directory.GetCurrentDirectory();

        [Fact]
        public void Initialize_Should_Return_Input_Parameter_Test()
        {
            // Arrange
            var inputParameter = _validFolderPath;
            const string defaultValue = nameof(defaultValue);
            var inputParameters = new Dictionary<string, string>
            {
                { new DestinationParameter().Name, inputParameter }
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
            var defaultValue = _validFolderPath;
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
                { new DestinationParameter().Name, parameterValue }
            };

            // Act
            var exception = Assert.Throws<ParameterAbsentException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{new DestinationParameter().Name}' parameter.", exception.Message);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void Initialize_Should_Throw_ParameterAbsentException_If_Default_Parameter_Is_Null_Or_WhiteSpace_Test(string parameterValue)
        {
            // Arrange
            var inputParameter = new Dictionary<string, string>();

            // Act
            var exception = Assert.Throws<ParameterAbsentException>(() => _sut.Initialize(inputParameter, parameterValue));

            // Assert
            Assert.Contains($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{new DestinationParameter().Name}' parameter.", exception.Message);
        }

        [Fact]
        public void Initialize_Should_Throw_DirectoryNotFoundException_If_Input_Parameter_Is_NonValid_Path_Test()
        {
            // Arrange
            const string defaultValue = nameof(defaultValue);
            const string inputParameter = nameof(inputParameter);
            var inputParameters = new Dictionary<string, string>
            {
                { new DestinationParameter().Name, inputParameter }
            };

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Directory '{inputParameter}' does not exist.", exception.Message);
        }

        [Fact]
        public void Initialize_Should_Throw_DirectoryNotFoundException_If_Default_Parameter_Is_NonValid_Path_Test()
        {
            // Arrange
            const string defaultValue = nameof(defaultValue);
            var inputParameters = new Dictionary<string, string>();

            // Act
            var exception = Assert.Throws<DirectoryNotFoundException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Directory '{defaultValue}' does not exist.", exception.Message);
        }
    }
}