namespace VirutalFactoryCore
{
    public class Machine
    {

        public enum State
        {
            Ready,
            Running,
            Error
        }

        private State _currentState;
        private SignalLight _signalLight;
        private JobManager _jobManager;
        private CancellationTokenSource _cancellationTokenSource;
        private Random _random;
        private int _machineId;

        public Machine(int machineId)
        {
            _machineId = machineId;
            _currentState = State.Ready;
            _random = new Random();
            _signalLight = new SignalLight();
            _jobManager = new JobManager();
        }

        public State CurrentState
        {
            get { return _currentState; }
            private set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    _signalLight.UpdateFromMachineState(_currentState);
                }
            }
        }

        public JobManager JobManager => _jobManager;

        public int MachineId => _machineId;

        public void Start()
        {
            if (CurrentState != State.Ready)
                return;

            Job currentJob = _jobManager.GetCurrentJob();
            if (currentJob == null)
                return;

            CurrentState = State.Running;
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => RunningProcess(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            if (CurrentState == State.Running)
            {
                _cancellationTokenSource?.Cancel();
                CurrentState = State.Ready;
            }
            else if (CurrentState == State.Error)
            {
                CurrentState = State.Ready;
            }
        }

        private async Task RunningProcess(CancellationToken cancellationToken)
        {
            Job currentJob = _jobManager.GetCurrentJob();
            int targetCycles = currentJob?.Quantity ?? 3;
            int processCount = currentJob?.QuantityProduced ?? 0;

            try
            {
                while (!cancellationToken.IsCancellationRequested && processCount < targetCycles)
                {
                    await Task.Delay(100, cancellationToken);
                    processCount++;
                    currentJob?.Produce(processCount);

                    if (_random.Next(100) < 2)
                    {
                        CurrentState = State.Error;
                        break;
                    }
                }

                if (!cancellationToken.IsCancellationRequested && CurrentState != State.Error && currentJob != null && processCount >= targetCycles)
                {
                    _jobManager.CompleteCurrentJob();
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
    }

    public class MachineManager
    {
        private List<Machine> _machines;

        public MachineManager()
        {
            _machines = new List<Machine>();
        }

        public void AddMachine(Machine machine)
        {
            _machines.Add(machine);
        }

        public void StartAllMachines()
        {
            foreach (var machine in _machines)
            {
                machine.Start();
            }
        }

        public void StopAllMachines()
        {
            foreach (var machine in _machines)
            {
                machine.Stop();
            }
        }

        public Machine GetMachineById(int machineId)
        {
            return _machines.FirstOrDefault(m => m.MachineId == machineId);
        }

        public IEnumerable<Machine> GetAllMachines()
        {
            return _machines;
        }
    }


    public interface IMachineService
    {
        void StartMachine(int machineId);
        void StopMachine(int machineId);
        void TriggerError(int machineId);
        void ResetMachine(int machineId);
        Machine.State GetMachineState(int machineId);
        Job GetCurrentJob(int machineId);
        void CreateJob(int machineId, string productName, int quantity);
        void StartJob(int machineId, int jobId);
        void CompleteCurrentJob(int machineId);
        void UpdateCurrentJobProgress(int machineId, int progress);
        void CancelJob(int machineId, int jobId);
    }



  
}