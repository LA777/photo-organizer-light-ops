using FluentAssertions;
using Moq;
using Polo.Abstractions.Wrappers;
using Polo.Commands;
using Serilog;
using System.Threading.Tasks;
using Xunit;

namespace Polo.UnitTests.Commands
{
    public class VersionCommandTests
    {
        [Fact]
        public async Task Action_Should_Write_Application_Version_To_The_Log_Test_Async()
        {
            // Arrange
            var resultVersionLog = string.Empty;
            var resultVersionConsole = string.Empty;
            var loggerMock = new Mock<ILogger>();
            var consoleWrapperMock = new Mock<IConsoleWrapper>();
            loggerMock.Setup(x => x.Verbose(It.IsAny<string>()))
                .Callback<string>(v => resultVersionLog = v);
            consoleWrapperMock.Setup(x => x.WriteLine(It.IsAny<string>()))
                .Callback<string>(v => resultVersionConsole = v);
            var expectedVersion = typeof(Program).Assembly.GetName().Version?.ToString();
            var sut = new VersionCommand(consoleWrapperMock.Object, loggerMock.Object);

            // Act
            await sut.ActionAsync();

            // Assert
            resultVersionLog.Should().BeEquivalentTo(expectedVersion);
            resultVersionConsole.Should().BeEquivalentTo(expectedVersion);
        }
    }
}