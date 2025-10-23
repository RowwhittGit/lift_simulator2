using System;
using lift_simulator.States;
using lift_simulator.Interfaces; // Add this
using lift_simulator.Database;

namespace lift_simulator
{
    public class LiftContext
    {
        public ILiftState CurrentState { get; private set; }
        public ILiftState DoorState { get; private set; }
        public DbConnection Db { get; private set; }
        public int CurrentFloor { get; set; } = 0;
        public bool IsDoorOpen { get; set; } = false;

        public LiftContext(DbConnection db)
        {
            Db = db;
            CurrentState = new IdleState();
            DoorState = new DoorClosingState();
        }

        public void SetState(ILiftState newState)
        {
            CurrentState = newState;
        }

        public void SetDoorState(ILiftState newDoorState)
        {
            DoorState = newDoorState;
        }
    }
}