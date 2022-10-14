using FluentAssertions;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Parameters;
using System;
using System.Collections.Generic;
using Xunit;

namespace Polo.UnitTests.Parameters
{
    public class LongSideLimitParameterTests
    {
        private readonly IParameter<int> _sut = new LongSideLimitParameter();

        [Fact]
        public void Initialize_Should_Return_Input_Parameter_Test()
        {
            // Arrange
            const int inputParameter = 1221;
            const int defaultValue = 1234;
            var inputParameters = new Dictionary<string, string>
            {
                { new LongSideLimitParameter().Name, inputParameter.ToString() }
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
            const int defaultValue = 1234;
            var inputParameters = new Dictionary<string, string>();

            // Act
            var result = _sut.Initialize(inputParameters, defaultValue);

            // Assert
            result.Should().Be(defaultValue);
        }

        [Fact]
        public void Initialize_Should_Throw_ArgumentOutOfRangeException_If_Input_Parameter_Is_OutOfRange_Test()
        {
            // Arrange
            var inputParameter = LongSideLimitParameter.Min - 1;
            const int defaultValue = 1234;
            var inputParameters = new Dictionary<string, string>
            {
                { new LongSideLimitParameter().Name, inputParameter.ToString() }
            };

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{new LongSideLimitParameter().Name}' should be higher than {LongSideLimitParameter.Min - 1}.", exception.Message);
        }

        [Fact]
        public void Initialize_Should_Throw_ArgumentOutOfRangeException_If_Default_Parameter_Is_OutOfRange_Test()
        {
            // Arrange
            var defaultValue = LongSideLimitParameter.Min - 1;
            var inputParameters = new Dictionary<string, string>();

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{new LongSideLimitParameter().Name}' should be higher than {LongSideLimitParameter.Min - 1}.", exception.Message);
        }

        [Fact]
        public void Initialize_Should_Throw_ParameterParseException_If_Input_Parameter_Is_Not_A_Number_Test()
        {
            // Arrange
            const string inputParameter = "a1";
            const int defaultValue = 1234;
            var inputParameters = new Dictionary<string, string>
            {
                { new LongSideLimitParameter().Name, inputParameter }
            };

            // Act
            var exception = Assert.Throws<ParameterParseException>(() => _sut.Initialize(inputParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{new LongSideLimitParameter().Name}' is not a number.", exception.Message);
        }
    }
}