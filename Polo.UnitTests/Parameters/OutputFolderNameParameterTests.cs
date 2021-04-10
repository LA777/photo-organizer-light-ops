using FluentAssertions;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters;
using Polo.Parameters;
using System.Collections.Generic;
using Xunit;

namespace Polo.UnitTests.Parameters
{
    public class OutputFolderNameParameterTests
    {
        private readonly IParameter<string> _sut = new OutputFolderNameParameter();

        [Fact]
        public void Initialize_Should_Return_Income_Parameter_Test()
        {
            // Arrange
            const string incomeParameter = nameof(incomeParameter);
            const string defaultValue = nameof(defaultValue);
            var incomeParameters = new Dictionary<string, string>
            {
                { OutputFolderNameParameter.Name, incomeParameter }
            };

            // Act
            var result = _sut.Initialize(incomeParameters, defaultValue);

            // Assert
            result.Should().Be(incomeParameter);
        }

        [Fact]
        public void Initialize_Should_Return_Default_Parameter_Test()
        {
            // Arrange
            const string defaultValue = nameof(defaultValue);
            var incomeParameters = new Dictionary<string, string>();

            // Act
            var result = _sut.Initialize(incomeParameters, defaultValue);

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
            var incomeParameters = new Dictionary<string, string>
            {
                { OutputFolderNameParameter.Name, parameterValue }
            };

            // Act
            var exception = Assert.Throws<ParameterAbsentException>(() => _sut.Initialize(incomeParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{OutputFolderNameParameter.Name}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.WatermarkOutputFolderName)}'.", exception.Message);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void Initialize_Should_Throw_ParameterAbsentException_If_Default_Parameter_Is_Null_Or_WhiteSpace_Test(string parameterValue)
        {
            // Arrange
            var incomeParameters = new Dictionary<string, string>();

            // Act
            var exception = Assert.Throws<ParameterAbsentException>(() => _sut.Initialize(incomeParameters, parameterValue));

            // Assert
            Assert.Contains($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{OutputFolderNameParameter.Name}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.WatermarkOutputFolderName)}'.", exception.Message);
        }
    }
}
