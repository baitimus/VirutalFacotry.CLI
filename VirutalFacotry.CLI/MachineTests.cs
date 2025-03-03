using NUnit.Framework;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

[TestFixture]
public class MachineTests
{
    private Mock<ConsoleDisplay> _mockDisplay;
    private Mock<JobManager> _mockJobManager;
    private Machine _machine;

    [SetUp]
    public void SetUp()
    {
        _mockDisplay = new Mock<ConsoleDisplay>();
        _mockJobManager = new Mock<JobManager>(_mockDisplay.Object);
        _machine = new Machine(_mockDisplay.Object);
    }

    [Test]
    public void Machine_InitialState_ShouldBeReady()
    {
        Assert.That(_machine.CurrentState, Is.EqualTo(Machine.State.Ready));
    }

    [Test]
    public void Start_WhenMachineIsReadyAndNoJob_ShouldDisplayErrorMessage()
    {
        _mockJobManager.Setup(j => j.GetCurrentJob()).Returns((Job)null);
        _machine.Start();
        _mockDisplay.Verify(d => d.UpdateMessage("Cannot start machine without an active job. Use 'select job' command first."), Times.Once);
    }

    [Test]
    public void Start_WhenMachineIsReadyAndJobExists_ShouldStartMachine()
    {
        var job = new Job(1, "TestProduct", 5);
        _mockJobManager.Setup(j => j.GetCurrentJob()).Returns(job);
        _machine.Start();
        Assert.That(_machine.CurrentState, Is.EqualTo(Machine.State.Running));
        _mockDisplay.Verify(d => d.UpdateMessage($"Machine started with job #{job.JobId}"), Times.Once);
    }

    [Test]
    public void Start_WhenMachineIsRunning_ShouldDisplayAlreadyRunningMessage()
    {
        var job = new Job(1, "TestProduct", 5);
        _mockJobManager.Setup(j => j.GetCurrentJob()).Returns(job);
        _machine.Start();
        _machine.Start();
        _mockDisplay.Verify(d => d.UpdateMessage("Machine is already running"), Times.Once);
    }

    [Test]
    public void Start_WhenMachineIsInErrorState_ShouldDisplayErrorMessage()
    {
        _machine.GetType().GetProperty("CurrentState").SetValue(_machine, Machine.State.Error);
        _machine.Start();
        _mockDisplay.Verify(d => d.UpdateMessage("Cannot start machine while in Error state. Use 'stop' command to reset."), Times.Once);
    }

    [Test]
    public void Stop_WhenMachineIsReady_ShouldDisplayAlreadyStoppedMessage()
    {
        _machine.Stop();
        _mockDisplay.Verify(d => d.UpdateMessage("Machine is already stopped"), Times.Once);
    }

    [Test]
    public void Stop_WhenMachineIsRunning_ShouldStopMachine()
    {
        var job = new Job(1, "TestProduct", 5);
        job.Produce(2);
        _mockJobManager.Setup(j => j.GetCurrentJob()).Returns(job);
        _machine.Start();
        _machine.Stop();
        Assert.That(_machine.CurrentState, Is.EqualTo(Machine.State.Ready));
        _mockDisplay.Verify(d => d.UpdateMessage("Machine stopped" + $"Produced {job.QuantityProduced} / {job.Quantity}"), Times.Once);
    }

    [Test]
    public void Stop_WhenMachineIsInErrorState_ShouldResetMachine()
    {
        _machine.GetType().GetProperty("CurrentState").SetValue(_machine, Machine.State.Error);
        _machine.Stop();
        Assert.That(_machine.CurrentState, Is.EqualTo(Machine.State.Ready));
        _mockDisplay.Verify(d => d.UpdateMessage("Machine has been reset from Error state"), Times.Once);
    }

    [Test]
    public async Task RunningProcess_ShouldCompleteJobSuccessfully()
    {
        var job = new Job(1, "TestProduct", 3);
        _mockJobManager.Setup(j => j.GetCurrentJob()).Returns(job);
        _machine.Start();
        await Task.Delay(10000); // Wait for the process to complete
        _mockJobManager.Verify(j => j.CompleteCurrentJob(), Times.Once);
        _mockDisplay.Verify(d => d.UpdateMessage($"Job {job.JobId} completed successfully" + $"Produced: {job.QuantityProduced} "), Times.Once);
    }

    [Test]
    public async Task RunningProcess_ShouldHandleCancellation()
    {
        var job = new Job(1, "TestProduct", 3);
        _mockJobManager.Setup(j => j.GetCurrentJob()).Returns(job);
        _machine.Start();
        _machine.Stop();
        await Task.Delay(1000); // Wait for the process to handle cancellation
        Assert.That(_machine.CurrentState, Is.EqualTo(Machine.State.Ready));
    }

    [Test]
    public async Task RunningProcess_ShouldHandleError()
    {
        var job = new Job(1, "TestProduct", 3);
        _mockJobManager.Setup(j => j.GetCurrentJob()).Returns(job);
        _machine.GetType().GetField("_random", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_machine, new Random(0)); // Force error
        _machine.Start();
        await Task.Delay(10000); // Wait for the process to complete
        Assert.That(_machine.CurrentState, Is.EqualTo(Machine.State.Error));
        _mockDisplay.Verify(d => d.UpdateMessage("Critical error occurred! Use 'stop' command to reset machine."), Times.Once);
    }
}
