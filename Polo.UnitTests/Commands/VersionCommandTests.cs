using FluentAssertions;
using Moq;
using Polo.Commands;
using Serilog;
using Xunit;

namespace Polo.UnitTests.Commands
{
    public class VersionCommandTests
    {
        [Fact]
        public void Action_Should_Write_Application_Version_To_The_Log_Test()
        {
            // Arrange
            var version = string.Empty;
            var loggerMock = new Mock<ILogger>();
            loggerMock.Setup(x => x.Verbose(It.IsAny<string>()))
                .Callback<string>(v => version = v);
            var expectedVersion = typeof(Program).Assembly.GetName().Version?.ToString();
            var sut = new VersionCommand(loggerMock.Object);

            // Act
            sut.Action();

            // Assert
            version.Should().BeEquivalentTo(expectedVersion);
        }
    }
}
