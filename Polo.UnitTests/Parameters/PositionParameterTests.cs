using FluentAssertions;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters;
using Polo.Parameters;
using System.Collections.Generic;
using Xunit;

namespace Polo.UnitTests.Parameters
{
    public class PositionParameterTests
    {
        private readonly IParameter<string> _sut = new PositionParameter();

        [Fact]
        public void Initialize_Should_Return_Input_Parameter_Test()
        {
            // Arrange
            const string inputParameter = nameof(inputParameter);
            const string defaultValue = nameof(defaultValue);
            var inputParameters = new Dictionary<string, string>
            {
                { new PositionParameter().Name, inputParameter }
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
            const string defaultValue = nameof(defaultValue);
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
                { new PositionParameter().Name, parameterValue }
            };

            // Act
            var exception = Assert.Throws<ParameterAbsentException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{new PositionParameter().Name}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.WatermarkPosition)}'.", exception.Message);
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
            Assert.Contains($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{new PositionParameter().Name}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.WatermarkPosition)}'.", exception.Message);
        }
    }
}