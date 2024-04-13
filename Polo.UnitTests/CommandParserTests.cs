using AutoFixture;
using FluentAssertions;
using Moq;
using Polo.Abstractions;
using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Polo.UnitTests
{
    public class CommandParserTests: BaseTest
    {
        private static readonly Mock<ICommand> _commandMock = new Mock<ICommand>();
        private readonly IEnumerable<ICommand> _commands = new List<ICommand> { _commandMock.Object };
        private static readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly ICommandParser _sut = new CommandParser(_loggerMock.Object);
        private Dictionary<string, string> _argumentsPassed;
        private IEnumerable<ICommand> _commandsPassed;

        public CommandParserTests()
        {
            _commandMock.Setup(x => x.Name).Returns(Fixture.Create<string>());
            _commandMock.Setup(x => x.ShortName).Returns(Fixture.Create<string>());
            _commandMock.Setup(
                x => x.ActionAsync(It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<IEnumerable<ICommand>>()))
                .Callback<IReadOnlyDictionary<string, string>, IEnumerable<ICommand>>(
                    (args, cmds) =>
                    {
                        _argumentsPassed = new Dictionary<string, string>(args);
                        _commandsPassed = cmds;
                    }
                );
        }

        [Fact]
        public async Task Parse_Should_Parse_Input_Arguments_To_Commands_And_DictionaryArguments_With_Full_CommandName_Test_Async()
        {
            // Arrange
            var parameterName = "param";
            var parameterValue = "42";
            var argumentLine = $"{CommandParser.CommandPrefix}{_commandMock.Object.Name} {CommandParser.ShortCommandPrefix}{parameterName}{CommandParser.ParameterDelimiter}{parameterValue}";
            var inputArguments = argumentLine.Split(' ');
            var parsedArguments = new Dictionary<string, string>
            {
                { parameterName, parameterValue }
            };
            _commandMock.Invocations.Clear();

            // Act
            await _sut.ParseAsync(inputArguments, _commands);


            // Assert
            _commandMock.Verify(x => x.ActionAsync(It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<IEnumerable<ICommand>>()), Times.Once);
            _argumentsPassed.Should().BeEquivalentTo(parsedArguments);
            _commandsPassed.Should().BeEquivalentTo(_commands);
        }

        [Fact]
        public async Task Parse_Should_Parse_Input_Arguments_To_Commands_And_DictionaryArguments_With_Short_CommandName_Test_Async()
        {
            // Arrange
            var parameterName = "param";
            var parameterValue = "42";
            var argumentLine = $"{CommandParser.ShortCommandPrefix}{_commandMock.Object.ShortName} {CommandParser.ShortCommandPrefix}{parameterName}{CommandParser.ParameterDelimiter}{parameterValue}";
            var inputArguments = argumentLine.Split(' ');
            var parsedArguments = new Dictionary<string, string>
            {
                { parameterName, parameterValue }
            };
            _commandMock.Invocations.Clear();

            // Act
            await _sut.ParseAsync(inputArguments, _commands);

            // Assert
            _commandMock.Verify(x => x.ActionAsync(It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<IEnumerable<ICommand>>()), Times.Once);
            _argumentsPassed.Should().BeEquivalentTo(parsedArguments);
            _commandsPassed.Should().BeEquivalentTo(_commands);
        }

        [Fact]
        public async Task Parse_Should_Parse_Input_Arguments_To_Commands_Without_Parameters_Test_Async()
        {
            // Arrange
            var argumentLine = $"{CommandParser.CommandPrefix}{_commandMock.Object.Name}";
            var inputArguments = argumentLine.Split(' ');
            _commandMock.Invocations.Clear();

            // Act
            await _sut.ParseAsync(inputArguments, _commands);

            // Assert
            _commandMock.Verify(x => x.ActionAsync(It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<IEnumerable<ICommand>>()), Times.Once);
            _argumentsPassed.Should().BeEmpty();
            _commandsPassed.Should().BeEquivalentTo(_commands);
        }

        [Fact]
        public async Task Parse_Should_Throw_ParseException_If_No_Command_Provided_Test_Async()
        {
            // Arrange
            var inputArguments = Array.Empty<string>();

            // Act
            var exception = await Assert.ThrowsAsync<ParameterParseException>(async() => await _sut.ParseAsync(inputArguments, _commands));

            // Assert
            Assert.Equal("ERROR: No command provided. Please enter --help to see available commands list.", exception.Message);
        }

        [Fact]
        public async Task Parse_Should_Throw_ParseException_If_No_Correct_Command_Provided_Test_Async()
        {
            // Arrange
            var argumentLine = $"{CommandParser.CommandPrefix}{_commandMock.Object.Name}zz";
            var inputArguments = argumentLine.Split(' ');

            // Act
            var exception = await Assert.ThrowsAsync<ParameterParseException>(async () => await _sut.ParseAsync(inputArguments, _commands));

            // Assert
            Assert.Equal("ERROR: Unknown command. Please enter --help to see available commands list.", exception.Message);
        }

        [Fact]
        public async Task Parse_Should_Throw_ParseException_If_Parameter_Provided_Without_Prefix_Test_Async()
        {
            // Arrange
            var parameterName = "param";
            var parameterValue = "42";
            var argumentLine = $"{CommandParser.ShortCommandPrefix}{_commandMock.Object.ShortName} {parameterName}{CommandParser.ParameterDelimiter}{parameterValue}";
            var inputArguments = argumentLine.Split(' ');

            // Act
            var exception = await Assert.ThrowsAsync<ParameterParseException>(async () => await _sut.ParseAsync(inputArguments, _commands));

            // Assert
            Assert.Equal("ERROR: Unknown parameter. Please enter --help to see available parameters list.", exception.Message);
        }

        [Fact]
        public async Task Parse_Should_Throw_ParseException_If_Parameter_Provided_Without_Delimiter_Test_Async()
        {
            // Arrange
            var parameterName = "param";
            var parameterValue = "42";
            var argumentLine = $"{CommandParser.ShortCommandPrefix}{_commandMock.Object.ShortName} {CommandParser.ShortCommandPrefix}{parameterName}{parameterValue}";
            var inputArguments = argumentLine.Split(' ');

            // Act
            var exception = await Assert.ThrowsAsync<ParameterParseException>(async () => await _sut.ParseAsync(inputArguments, _commands));

            // Assert
            Assert.Equal("ERROR: Parameter delimiter missed. Please enter --help to see correct parameter syntax.", exception.Message);
        }

        [Fact]
        public async Task Parse_Should_Parse_Input_Arguments_To_Commands_And_DictionaryArguments_If_Parameter_Value_Contains_Semicolon_Test()
        {
            // Arrange
            var parameterName = "param";
            var parameterValue = "c:\\\\folder";
            var argumentLine = $"{CommandParser.CommandPrefix}{_commandMock.Object.Name} {CommandParser.ShortCommandPrefix}{parameterName}{CommandParser.ParameterDelimiter}{parameterValue}";
            var inputArguments = argumentLine.Split(' ');
            var parsedArguments = new Dictionary<string, string>
            {
                { parameterName, parameterValue }
            };
            _commandMock.Invocations.Clear();

            // Act
            await _sut.ParseAsync(inputArguments, _commands);

            // Assert
            _commandMock.Verify(x => x.ActionAsync(It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<IEnumerable<ICommand>>()), Times.Once);
            _argumentsPassed.Should().BeEquivalentTo(parsedArguments);
            _commandsPassed.Should().BeEquivalentTo(_commands);
        }

        [Fact]
        public async Task Parse_Should_Parse_Input_Arguments_To_Commands_And_DictionaryArguments_If_Parameter_Value_Contains_Many_Semicolons_Test_Async()
        {
            // Arrange
            var parameterName = "param";
            var parameterValue = ":c:\\\\fol:der:";
            var argumentLine = $"{CommandParser.CommandPrefix}{_commandMock.Object.Name} {CommandParser.ShortCommandPrefix}{parameterName}{CommandParser.ParameterDelimiter}{parameterValue}";
            var inputArguments = argumentLine.Split(' ');
            var parsedArguments = new Dictionary<string, string>
            {
                { parameterName, parameterValue }
            };
            _commandMock.Invocations.Clear();

            // Act
            await _sut.ParseAsync(inputArguments, _commands);

            // Assert
            _commandMock.Verify(x => x.ActionAsync(It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<IEnumerable<ICommand>>()), Times.Once);
            _argumentsPassed.Should().BeEquivalentTo(parsedArguments);
            _commandsPassed.Should().BeEquivalentTo(_commands);
        }

        [Fact]
        public async Task Parse_Should_Parse_Input_Arguments_To_Commands_And_DictionaryArguments_If_Parameter_Value_Contains_Quotation_Marks_Test_Async()
        {
            // Arrange
            var parameterName = "param";
            var parameterValue = "'c:\\\\new folder'";
            var inputArguments = new[] { $"{CommandParser.CommandPrefix}{_commandMock.Object.Name}", $"{CommandParser.ShortCommandPrefix}{parameterName}{CommandParser.ParameterDelimiter}{parameterValue}" };
            var parsedArguments = new Dictionary<string, string>
            {
                { parameterName, parameterValue }
            };
            _commandMock.Invocations.Clear();

            // Act
            await _sut.ParseAsync(inputArguments, _commands);

            // Assert
            _commandMock.Verify(x => x.ActionAsync(It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<IEnumerable<ICommand>>()), Times.Once);
            _argumentsPassed.Should().BeEquivalentTo(parsedArguments);
            _commandsPassed.Should().BeEquivalentTo(_commands);
        }
    }
}
