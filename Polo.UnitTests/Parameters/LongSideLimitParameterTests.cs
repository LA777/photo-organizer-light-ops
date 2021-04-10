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
        public void Initialize_Should_Return_Income_Parameter_Test()
        {
            // Arrange
            const int incomeParameter = 1221;
            const int defaultValue = 1234;
            var incomeParameters = new Dictionary<string, string>
            {
                { LongSideLimitParameter.Name, incomeParameter.ToString() }
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
            const int defaultValue = 1234;
            var incomeParameters = new Dictionary<string, string>();

            // Act
            var result = _sut.Initialize(incomeParameters, defaultValue);

            // Assert
            result.Should().Be(defaultValue);
        }

        [Fact]
        public void Initialize_Should_Throw_ArgumentOutOfRangeException_If_Input_Parameter_Is_OutOfRange_Test()
        {
            // Arrange
            var incomeParameter = LongSideLimitParameter.Min - 1;
            const int defaultValue = 1234;
            var incomeParameters = new Dictionary<string, string>
            {
                { LongSideLimitParameter.Name, incomeParameter.ToString() }
            };

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Initialize(incomeParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{LongSideLimitParameter.Name}' should be higher than {LongSideLimitParameter.Min - 1}.", exception.Message);
        }

        [Fact]
        public void Initialize_Should_Throw_ArgumentOutOfRangeException_If_Default_Parameter_Is_OutOfRange_Test()
        {
            // Arrange
            var defaultValue = LongSideLimitParameter.Min - 1;
            var incomeParameters = new Dictionary<string, string>();

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _sut.Initialize(incomeParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{LongSideLimitParameter.Name}' should be higher than {LongSideLimitParameter.Min - 1}.", exception.Message);
        }

        [Fact]
        public void Initialize_Should_Throw_ParameterParseException_If_Input_Parameter_Is_Not_A_Number_Test()
        {
            // Arrange
            var incomeParameter = "a1";
            const int defaultValue = 1234;
            var incomeParameters = new Dictionary<string, string>
            {
                { LongSideLimitParameter.Name, incomeParameter }
            };

            // Act
            var exception = Assert.Throws<ParameterParseException>(() => _sut.Initialize(incomeParameters, defaultValue));

            // Assert
            Assert.Contains($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{LongSideLimitParameter.Name}' is not a number.", exception.Message);
        }
    }
}
