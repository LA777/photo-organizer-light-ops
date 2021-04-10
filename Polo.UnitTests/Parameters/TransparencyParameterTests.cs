using FluentAssertions;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Parameters;
using System;
using System.Collections.Generic;
using Xunit;

namespace Polo.UnitTests.Parameters
{
    public class TransparencyParameterTests
    {
        private const int MinOutOfRange = -1;
        private const int MaxOutOfRange = 101;
        private readonly IParameter<int> _sut = new TransparencyParameter();

        [Fact]
        public void Initialize_Should_Return_Input_Parameter_Test()
        {
            // Arrange
            const int inputParameter = 42;
            const int defaultValue = 73;
            var inputParameters = new Dictionary<string, string>
            {
                { TransparencyParameter.Name, inputParameter.ToString() }
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
            const int defaultValue = 73;
            var inputParameters = new Dictionary<string, string>();

            // Act
            var result = _sut.Initialize(inputParameters, defaultValue);

            // Assert
            result.Should().Be(defaultValue);
        }

        [Theory]
        [InlineData(MinOutOfRange)]
        [InlineData(MaxOutOfRange)]
        public void Initialize_Should_Throw_ArgumentOutOfRangeException_If_Input_Parameter_Is_OutOfRange_Test(int value)
        {
            // Arrange
            var inputParameter = value;
            const int defaultValue = 12;
            var inputParameters = new Dictionary<string, string>
            {
                { TransparencyParameter.Name, inputParameter.ToString() }
            };

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{TransparencyParameter.Name}' should be in the range from '{TransparencyParameter.Min}' to '{TransparencyParameter.Max}'.", exception.Message);
        }

        [Theory]
        [InlineData(MinOutOfRange)]
        [InlineData(MaxOutOfRange)]
        public void Initialize_Should_Throw_ArgumentOutOfRangeException_If_Default_Parameter_Is_OutOfRange_Test(int value)
        {
            // Arrange
            var defaultValue = value;
            var inputParameters = new Dictionary<string, string>();

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{TransparencyParameter.Name}' should be in the range from '{TransparencyParameter.Min}' to '{TransparencyParameter.Max}'.", exception.Message);
        }

        [Fact]
        public void Initialize_Should_Throw_ParameterParseException_If_Input_Parameter_Is_Not_A_Number_Test()
        {
            // Arrange
            const string inputParameter = "a1";
            const int defaultValue = 12;
            var inputParameters = new Dictionary<string, string>
            {
                { TransparencyParameter.Name, inputParameter }
            };

            // Act
            var exception = Assert.Throws<ParameterParseException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{TransparencyParameter.Name}' is not a number.", exception.Message);
        }
    }
}
