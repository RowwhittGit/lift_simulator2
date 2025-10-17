using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lift_simulator.States
{
    public class DoorClosedState : ILiftState
    {
        public void Enter(LiftContext context)
        {
            if (context?.Db != null)
            {
                context.Db.LogEvent("Door Closed", $"Lift door closed at floor {context.CurrentFloor}");
            }
            context.IsDoorOpen = false;
        }

        public void Execute(LiftContext context)
        {
            // Door remains closed
        }

        public void Exit(LiftContext context)
        {
            // Transitioning away from closed state
        }
    }
}