using lift_simulator.Database;
using lift_simulator.Interfaces;
using lift_simulator.States;
using System;
using System.Collections.Generic;
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

        // Add a queue to store floor requests
        private Queue<int> _floorQueue = new Queue<int>();

        public int CurrentFloor { get; private set; } = 0;
        public bool IsDoorOpen { get; set; } = false;
        public bool IsBusy { get; private set; } = false;

        public event Action<string> OnStatusChanged;
        public event Action<int> OnFloorChanged;
        public event Action<string> OnDoorStateChanged;

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

        // IMPROVED: MoveToFloor now adds to queue instead of ignoring
        public void MoveToFloor(int targetFloor)
        {
            // Check if door is open - don't move if it is
            if (IsDoorOpen)
            {
                Log($"Cannot move to floor {targetFloor}. Door is open!");
                return;
            }

            // Check if already at floor
            if (targetFloor == CurrentFloor)
            {
                Log($"Already at floor {targetFloor}");
                OpenDoor();
                return;
            }

            // If lift is not busy, start moving immediately
            if (!IsBusy)
            {
                Log($"Moving to floor {targetFloor}");
                _worker.RunWorkerAsync(targetFloor);
            }
            else
            {
                // If lift IS busy, add to queue to be processed later
                _floorQueue.Enqueue(targetFloor);
                Log($"Lift is busy. Added floor {targetFloor} to queue");
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

            // NEW: After completing this move, check if there are more floors in queue
            ProcessQueue();
        }

        // NEW: Process the queue of waiting floor requests
        public void ProcessQueue()
        {
            // If door is open, wait for it to close before processing next request
            if (IsDoorOpen)
            {
                Log("Door is open. Waiting to close before next move");
                return;
            }

            // If there are floors waiting in queue, move to the next one
            if (_floorQueue.Count > 0)
            {
                int nextFloor = _floorQueue.Dequeue();
                Log($"Processing queued request: Moving to floor {nextFloor}");
                MoveToFloor(nextFloor);  // This will check conditions again
            }
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