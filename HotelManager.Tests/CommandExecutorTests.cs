using HotelManager.Commands;
using Moq;
using CSharpFunctionalExtensions;

namespace HotelManager.Tests;

public class CommandExecutorTests
{
    private Mock<IConsoleCommand> _mockCommand1;
    private Mock<IConsoleCommand> _mockCommand2;
    private CommandExecutor _commandExecutor;

    [SetUp]
    public void Setup()
    {
        _mockCommand1 = new Mock<IConsoleCommand>();
        _mockCommand2 = new Mock<IConsoleCommand>();

        var commands = new List<IConsoleCommand>
        {
            _mockCommand1.Object,
            _mockCommand2.Object
        };

        _commandExecutor = new CommandExecutor(commands);
    }

    [Test]
    public void Execute_Should_Return_Success_When_Valid_Command_Is_Entered()
    {
        // Arrange
        const string input = "ValidCommand";
        const string output = "Command handled successfully";

        _mockCommand1.Setup(c => c.CanHandle(input)).Returns(true);
        _mockCommand1.Setup(c => c.Handle(input)).Returns(Result.Success(output));

        // Act
        var result = _commandExecutor.Execute(input);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(output));
        _mockCommand2.Verify(c => c.CanHandle(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void Execute_Should_Return_Failure_When_No_Command_Can_Handle_Input()
    {
        // Arrange
        const string input = "InvalidCommand";

        _mockCommand1.Setup(c => c.CanHandle(input)).Returns(false);
        _mockCommand2.Setup(c => c.CanHandle(input)).Returns(false);

        // Act
        var result = _commandExecutor.Execute(input);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.EqualTo($"Invalid input: {input}"));
    }

    [Test]
    public void Execute_Should_Return_Success_When_Second_Command_Handles_Input()
    {
        // Arrange
        const string input = "SecondCommand";
        const string output = "Handled by second command";

        _mockCommand1.Setup(c => c.CanHandle(input)).Returns(false);
        _mockCommand2.Setup(c => c.CanHandle(input)).Returns(true);
        _mockCommand2.Setup(c => c.Handle(input)).Returns(Result.Success(output));

        // Act
        var result = _commandExecutor.Execute(input);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(output));
        _mockCommand1.Verify(c => c.CanHandle(input), Times.Once);
        _mockCommand2.Verify(c => c.CanHandle(input), Times.Once);
        _mockCommand2.Verify(c => c.Handle(input), Times.Once);
    }
}