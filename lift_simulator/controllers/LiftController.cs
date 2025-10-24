using lift_simulator.Database;
using lift_simulator.Interfaces;
using lift_simulator.States;
using System;
using System.Collections.Generic;

namespace lift_simulator.Controllers
{
    public class LiftController
    {
        private ILiftState _currentState;
        private readonly DbConnection _db;
        private Queue<int> _floorQueue = new Queue<int>();

        public int CurrentFloor { get; set; } = 0;
        public int TargetFloor { get; set; } = 0;
        public bool IsDoorOpen { get; set; } = false;
        public bool IsBusy { get; private set; } = false;

        // Events - ONLY controller fires these
        public event Action<string> OnStatusChanged;      // State name changed
        public event Action<int> OnFloorChanged;          // Floor changed
        public event Action<bool> OnDoorStateChanged;     // Door opened/closed

        public LiftController(DbConnection db)
        {
            _db = db;
            _currentState = new IdleState();
            _currentState.Enter(this);
        }

        public LiftController()
        {
            _db = new DbConnection();
            _currentState = new IdleState();
            _currentState.Enter(this);
        }

        // ONLY controller manages state transitions
        public void TransitionToState(ILiftState newState)
        {
            _currentState.Exit(this);
            _currentState = newState;
            _currentState.Enter(this);

            // Fire event AFTER state transition
            OnStatusChanged?.Invoke(_currentState.GetStateName());

            // If entering a door state, fire door event
            if (newState is DoorOpeningState || newState is DoorOpenState)
            {
                OnDoorStateChanged?.Invoke(true);  // Door opening/open
            }
            else if (newState is DoorClosingState || newState is DoorClosedState)
            {
                OnDoorStateChanged?.Invoke(false); // Door closing/closed
            }
        }

        // External call to move to a floor
        public void CallFloor(int targetFloor)
        {
            if (IsDoorOpen)
            {
                Log($"Cannot move to floor {targetFloor}. Door is open!");
                return;
            }

            if (targetFloor == CurrentFloor)
            {
                Log($"Already at floor {targetFloor}");
                _currentState.HandleRequest(this, "OpenDoor");
                return;
            }

            if (!IsBusy)
            {
                TargetFloor = targetFloor;
                _currentState.HandleRequest(this, $"MoveToFloor:{targetFloor}");
            }
            else
            {
                _floorQueue.Enqueue(targetFloor);
                Log($"Lift is busy. Added floor {targetFloor} to queue");
            }
        }

        // Door requests from UI
        public void RequestDoorOpen()
        {
            _currentState.HandleRequest(this, "OpenDoor");
        }

        public void RequestDoorClose()
        {
            _currentState.HandleRequest(this, "CloseDoor");
        }

        // States call this to move to a floor
        public void MoveToFloor(int targetFloor)
        {
            TargetFloor = targetFloor;

            if (targetFloor > CurrentFloor)
            {
                TransitionToState(new MovingUpState());
            }
            else if (targetFloor < CurrentFloor)
            {
                TransitionToState(new MovingDownState());
            }
        }

        // States call this when they arrive at target
        public void ArriveAtFloor(int floor)
        {
            CurrentFloor = floor;
            IsBusy = false;
            OnFloorChanged?.Invoke(CurrentFloor);
            Log($"Lift arrived at floor {CurrentFloor}");

            RequestDoorOpen();
            ProcessQueue();
        }

        // Process queued floor requests
        public void ProcessQueue()
        {
            if (IsDoorOpen)
            {
                Log("Door is open. Waiting to close before next move");
                return;
            }

            if (_floorQueue.Count > 0)
            {
                int nextFloor = _floorQueue.Dequeue();
                Log($"Processing queued request: Moving to floor {nextFloor}");
                CallFloor(nextFloor);
            }
        }

        // Utility methods
        public void SetBusy(bool busy) => IsBusy = busy;
        public void Log(string msg) => _db.LogEvent(msg);
        public DbConnection GetDatabase() => _db;
    }
}