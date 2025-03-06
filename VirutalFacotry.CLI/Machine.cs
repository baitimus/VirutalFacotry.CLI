// Machine.cs
using System;
using System.Threading;
using System.Threading.Tasks;

public class Machine
{
    // Define machine states
    public enum State
    {
        Ready,
        Running,
        Error
    }

    // Current state of the machine
    private State _currentState;

    // Reference to the associated signal light
    private SignalLight _signalLight;

    // Reference to the display
    private ConsoleDisplay _display;

    // Reference to the job manager
    private JobManager _jobManager;

    // CancellationTokenSource to control the background task
    private CancellationTokenSource _cancellationTokenSource;

    // Random generator for simulating errors
    private Random _random;


    public Machine(ConsoleDisplay display)
    {
        _currentState = State.Ready;
        _random = new Random();
        _display = display;
        _signalLight = new SignalLight(display);
        _jobManager = new JobManager(display);

        _display.UpdateMachineState(_currentState);
        _display.UpdateMessage("Machine initialized and ready");
    }

    // Property to get current state
    public State CurrentState
    {
        get { return _currentState; }
        private set
        {
            if (_currentState != value)
            {
                _currentState = value;
                _display.UpdateMachineState(_currentState);

                // Update the signal light whenever state changes
                _signalLight.UpdateFromMachineState(_currentState);
            }
        }
    }

    // Get access to the job manager
    public JobManager JobManager
    {
        get { return _jobManager; }
    }
    
    // Start the machine
    public void Start()
    {
        switch (CurrentState)
        {
            case State.Ready:
                // Only start if there's a job in InWork state
                Job currentJob = _jobManager.GetCurrentJob();
                if (currentJob == null)
                {
                    _display.UpdateMessage("Cannot start machine without an active job. Use 'select job' command first.");
                    return;
                }

                CurrentState = State.Running;
                _display.UpdateMessage($"Machine started with job #{currentJob.JobId}");

                // Start the background process
                _cancellationTokenSource = new CancellationTokenSource();
                Task.Run(() => RunningProcess(_cancellationTokenSource.Token));
                break;

            case State.Running:
                _display.UpdateMessage("Machine is already running");
                break;

            case State.Error:
                _display.UpdateMessage("Cannot start machine while in Error state. Use 'stop' command to reset.");
                break;
        }
    }











    // Stop the machine
  public void Stop()
    {
        switch (CurrentState)
        {
            case State.Ready:
                _display.UpdateMessage("Machine is already stopped");
                break;

            case State.Running:
                // Cancel the background process
                _cancellationTokenSource?.Cancel();
                CurrentState = State.Ready;
                Job currentJob = _jobManager.GetCurrentJob();
                

                _display.UpdateMessage("Machine stopped" + $"Produced {currentJob.QuantityProduced} / {currentJob.Quantity}");
                break;

            case State.Error:
                // Reset from error state
                CurrentState = State.Ready;
                _display.UpdateMessage("Machine has been reset from Error state");
                break;
        }
    }

    // Simulate machine running process
    private async Task RunningProcess(CancellationToken cancellationToken)
    {

        int processCount = 0;
        Job currentJob = _jobManager.GetCurrentJob();
        int targetCycles = currentJob?.Quantity ?? 3; // Default to 3 cycles if no job quantity specified

        processCount = currentJob.QuantityProduced;


        try
        {
            while (!cancellationToken.IsCancellationRequested && processCount < targetCycles)
            {

                // Simulate processing time delay at the begginning to prevent instant completion when restarting production
                await Task.Delay(100, cancellationToken);


                processCount++;
                currentJob.Produce(processCount);
                

                // Update message 
                _display.UpdateMessage($"Processing job {currentJob.JobId}: {currentJob.ProductName} (cycle {processCount}/{targetCycles})");

                // Randomly generate an error (2% chance)
                if (_random.Next(100) < 2)
                {
                    CurrentState = State.Error;
                    _display.UpdateMessage("Critical error occurred! Use 'stop' command to reset machine.");
                    break;
                }

              
            }

            // If we completed all cycles without error or cancellation, mark the job as completed
            if (!cancellationToken.IsCancellationRequested && CurrentState != State.Error && currentJob != null && processCount >= targetCycles)
            {
                _jobManager.CompleteCurrentJob();
                _display.UpdateMessage($"Job {currentJob.JobId} completed successfully" + $"Produced: {currentJob.QuantityProduced} ");
            }
        }
        catch (OperationCanceledException)
        {
            // Normal cancellation, do nothing
        }
        finally
        {
            if (CurrentState == State.Running)
            {
                CurrentState = State.Ready;
            }
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is Machine machine &&
               _currentState == machine._currentState &&
               EqualityComparer<SignalLight>.Default.Equals(_signalLight, machine._signalLight) &&
               EqualityComparer<ConsoleDisplay>.Default.Equals(_display, machine._display) &&
               EqualityComparer<JobManager>.Default.Equals(_jobManager, machine._jobManager) &&
               EqualityComparer<CancellationTokenSource>.Default.Equals(_cancellationTokenSource, machine._cancellationTokenSource) &&
               EqualityComparer<Random>.Default.Equals(_random, machine._random) &&
               CurrentState == machine.CurrentState &&
               EqualityComparer<JobManager>.Default.Equals(JobManager, machine.JobManager);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_currentState, _signalLight, _display, _jobManager, _cancellationTokenSource, _random, CurrentState, JobManager);
    }
}
