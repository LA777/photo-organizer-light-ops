using FluentAssertions;
using Polo.Abstractions.Enums;
using Polo.Abstractions.Parameters;
using Polo.Parameters;
using System;
using System.Collections.Generic;
using Xunit;

namespace Polo.UnitTests.Parameters
{
    public class OutputFileTypeParameterTests
    {
        private readonly IParameter<OutputFileType> _sut = new OutputFileTypeParameter();

        [Fact]
        public void Initialize_Should_Return_Input_Parameter_Test()
        {
            // Arrange
            const OutputFileType defaultValue = OutputFileType.SQLITE;
            string inputParameter = defaultValue.ToString().ToLower();
            var inputParameters = new Dictionary<string, string>
            {
                { new OutputFileTypeParameter().Name, inputParameter }
            };

            // Act
            var result = _sut.Initialize(inputParameters, defaultValue);

            // Assert
            result.Should().Be(defaultValue);
        }

        [Fact]
        public void Initialize_Should_Return_Default_Parameter_Test()
        {
            // Arrange
            const OutputFileType defaultValue = OutputFileType.SQLITE;
            var inputParameters = new Dictionary<string, string>();

            // Act
            var result = _sut.Initialize(inputParameters, defaultValue);

            // Assert
            result.Should().Be(defaultValue);
        }

        [Fact]
        public void Initialize_Should_Return_JSON_Parameter_Test()
        {
            // Arrange
            const OutputFileType defaultValue = OutputFileType.JSON;
            string inputParameter = defaultValue.ToString().ToLower();
            var inputParameters = new Dictionary<string, string>
            {
                { new OutputFileTypeParameter().Name, inputParameter }
            };

            // Act
            var result = _sut.Initialize(inputParameters, defaultValue);

            // Assert
            result.Should().Be(defaultValue);
        }


        [Fact]
        public void Initialize_Should_Return_CSV_Parameter_Test()
        {
            // Arrange
            const OutputFileType defaultValue = OutputFileType.CSV;
            string inputParameter = defaultValue.ToString().ToLower();
            var inputParameters = new Dictionary<string, string>
            {
                { new OutputFileTypeParameter().Name, inputParameter }
            };

            // Act
            var result = _sut.Initialize(inputParameters, defaultValue);

            // Assert
            result.Should().Be(defaultValue);
        }

        [Fact]
        public void Initialize_Should_Return_SQLITE_Parameter_Test()
        {
            // Arrange
            const OutputFileType defaultValue = OutputFileType.SQLITE;
            string inputParameter = defaultValue.ToString().ToLower();
            var inputParameters = new Dictionary<string, string>
            {
                { new OutputFileTypeParameter().Name, inputParameter }
            };

            // Act
            var result = _sut.Initialize(inputParameters, defaultValue);

            // Assert
            result.Should().Be(defaultValue);
        }

        [Fact]
        public void Initialize_Should_Return_TXT_Parameter_Test()
        {
            // Arrange
            const OutputFileType defaultValue = OutputFileType.TXT;
            string inputParameter = defaultValue.ToString().ToLower();
            var inputParameters = new Dictionary<string, string>
            {
                { new OutputFileTypeParameter().Name, inputParameter }
            };

            // Act
            var result = _sut.Initialize(inputParameters, defaultValue);

            // Assert
            result.Should().Be(defaultValue);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("img")]
        [InlineData("jpg")]
        public void Initialize_Should_Throw_Exception_If_Input_Parameter_Is_Ambiguous_Test(string parameterValue)
        {
            // Arrange
            const OutputFileType defaultValue = OutputFileType.SQLITE;
            var inputParameters = new Dictionary<string, string>
            {
                { new OutputFileTypeParameter().Name, parameterValue }
            };

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{new OutputFileTypeParameter().Name}' has ambiguous value.", exception.Message);
        }
    }
}