using CSharpFunctionalExtensions;
using HotelManager.Common;
using Moq;

namespace HotelManager.Tests;

public class InputLoopTests
{
    private Mock<IConsole> _mockConsole;
    private Mock<ICommandExecutor> _mockCommandExecutor;
    private InputLoop _inputLoop;

    [SetUp]
    public void Setup()
    {
        _mockConsole = new Mock<IConsole>();
        _mockCommandExecutor = new Mock<ICommandExecutor>();
        
        _inputLoop = new InputLoop(_mockConsole.Object, _mockCommandExecutor.Object);
    }

    [Test]
    public void Run_Should_Exit_When_Exit_Command_Is_Entered()
    {
        // Arrange
        _mockConsole.SetupSequence(c => c.Read())
            .Returns("Exit");

        // Act
        _inputLoop.Run();

        // Assert:
        _mockConsole.Verify(c => c.Read(), Times.Once);
        _mockConsole.Verify(c => c.WriteLine(It.IsAny<string>()), Times.Never);
        _mockConsole.Verify(c => c.WriteError(It.IsAny<string>()), Times.Never);
        _mockCommandExecutor.Verify(exec => exec.Execute(It.IsAny<string>()), Times.Never); 
    }

    [Test]
    public void Run_Should_Continue_When_Empty_Input_Is_Entered()
    {
        // Arrange
        _mockConsole.SetupSequence(c => c.Read())
            .Returns("")
            .Returns("Exit");

        // Act
        _inputLoop.Run();

        // Assert
        _mockConsole.Verify(c => c.Read(), Times.Exactly(2));
        _mockConsole.Verify(c => c.WriteLine(It.IsAny<string>()), Times.Never);
        _mockConsole.Verify(c => c.WriteError(It.IsAny<string>()), Times.Never);
        _mockCommandExecutor.Verify(exec => exec.Execute(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void Run_Should_Execute_Command_When_Valid_Input_Is_Entered()
    {
        // Arrange
        const string command = "ValidCommand";
        const string output = "Some output";
        
        _mockConsole.SetupSequence(c => c.Read())
            .Returns(command)
            .Returns("Exit");
        
        _mockCommandExecutor.Setup(exec => exec.Execute(command)).Returns(Result.Success(output));

        // Act
        _inputLoop.Run();

        // Assert
        _mockConsole.Verify(c => c.Read(), Times.Exactly(2));
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(x => x == output)), Times.Once);
        _mockConsole.Verify(c => c.WriteError(It.IsAny<string>()), Times.Never);
        _mockCommandExecutor.Verify(exec => exec.Execute(command), Times.Once);
    }

    [Test]
    public void Run_Should_Write_Error_When_Command_Fails()
    {
        // Arrange
        const string command = "InvalidCommand";
        const string error = "Command failed";

        _mockConsole.SetupSequence(c => c.Read())
            .Returns(command)
            .Returns("Exit");

        _mockCommandExecutor.Setup(exec => exec.Execute(command)).Returns(Result.Failure<string>(error));

        // Act
        _inputLoop.Run();

        // Assert
        _mockConsole.Verify(c => c.Read(), Times.Exactly(2));
        _mockConsole.Verify(c => c.WriteLine(It.IsAny<string>()), Times.Never);
        _mockConsole.Verify(c => c.WriteError(It.Is<string>(x => x == error)), Times.Once);
        _mockCommandExecutor.Verify(exec => exec.Execute(command), Times.Once);
    }
}