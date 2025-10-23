using lift_simulator.Database;
using lift_simulator.Interfaces;
using lift_simulator.States;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace lift_simulator.Controllers
{
    public class LiftController
    {
        private ILiftState _currentState;
        private readonly System.Windows.Forms.Timer _doorTimer;
        private readonly BackgroundWorker _worker;
        private readonly DbConnection _db;

        public int CurrentFloor { get; private set; } = 0;
        public bool IsDoorOpen { get; set; } = false; // Changed to public setter
        public bool IsBusy { get; private set; } = false;

        public event Action<string> OnStatusChanged;
        public event Action<int> OnFloorChanged;
        public event Action<string> OnDoorStateChanged;

        // Add constructor that accepts DbConnection
        public LiftController(DbConnection db)
        {
            _db = db;
            _doorTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _doorTimer.Tick += DoorTimer_Tick;

            _worker = new BackgroundWorker();
            _worker.DoWork += MoveLift_DoWork;
            _worker.RunWorkerCompleted += MoveLift_Completed;

            _currentState = new IdleState();
        }

        // Keep the parameterless constructor for backward compatibility
        public LiftController()
        {
            _db = new DbConnection();
            _doorTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _doorTimer.Tick += DoorTimer_Tick;

            _worker = new BackgroundWorker();
            _worker.DoWork += MoveLift_DoWork;
            _worker.RunWorkerCompleted += MoveLift_Completed;

            _currentState = new IdleState();
        }

        // Add missing methods
        public void OpenDoor()
        {
            if (!IsBusy)
            {
                HandleRequest("OpenDoor");
            }
        }

        public void CloseDoor()
        {
            if (!IsBusy)
            {
                HandleRequest("CloseDoor");
            }
        }

        public void MoveToFloor(int targetFloor)
        {
            if (!IsBusy && targetFloor != CurrentFloor)
            {
                _worker.RunWorkerAsync(targetFloor);
            }
            else if (targetFloor == CurrentFloor)
            {
                Log($"Already at floor {targetFloor}");
                OpenDoor();
            }
        }

        public void HandleRequest(string request)
        {
            _currentState.HandleRequest(this, request);
        }

        public void SetState(ILiftState newState)
        {
            _currentState.Exit(this);
            _currentState = newState;
            _currentState.Enter(this);
            OnStatusChanged?.Invoke(newState.GetType().Name);
        }

        private void MoveLift_DoWork(object? sender, DoWorkEventArgs e)
        {
            int targetFloor = (int)e.Argument;
            IsBusy = true;

            if (targetFloor > CurrentFloor)
            {
                SetState(new MovingUpState());
            }
            else if (targetFloor < CurrentFloor)
            {
                SetState(new MovingDownState());
            }

            System.Threading.Thread.Sleep(2000);
            CurrentFloor = targetFloor;
        }

        private void MoveLift_Completed(object? sender, RunWorkerCompletedEventArgs e)
        {
            SetState(new IdleState());
            IsBusy = false;
            OnFloorChanged?.Invoke(CurrentFloor);
            _db.LogEvent($"Lift arrived at floor {CurrentFloor}");

            // Automatically open door when arrived
            OpenDoor();
        }

        private void DoorTimer_Tick(object? sender, EventArgs e)
        {
            _doorTimer.Stop();
            OnDoorStateChanged?.Invoke(IsDoorOpen ? "Open" : "Closed");
        }

        public void StartDoorTimer() => _doorTimer.Start();
        public void Log(string msg) => _db.LogEvent(msg);
    }
}