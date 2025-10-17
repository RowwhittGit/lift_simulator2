using lift_simulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lift_simulator.States;

namespace lift_simulator
{
    public class LiftContext
    {
        public ILiftState CurrentState { get; private set; }
        public ILiftState DoorState { get; private set; }  // ← NEW: Track door state
        public DbConnection Db { get; private set; }
        public int CurrentFloor { get; set; } = 0;
        public bool IsDoorOpen { get; set; } = false;  // ← NEW: Track door status

        public LiftContext(DbConnection db)
        {
            Db = db;
            CurrentState = new IdleState();
            DoorState = new DoorClosedState();  // ← NEW: Start with doors closed
        }

        public void SetState(ILiftState newState)
        {
            CurrentState = newState;
        }

        public void SetDoorState(ILiftState newDoorState)  // ← NEW: Method to change door state
        {
            DoorState = newDoorState;
        }
    }
}