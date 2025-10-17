using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lift_simulator.States
{
    public class DoorOpenState : ILiftState
    {
        public void Enter(LiftContext context)
        {
            if (context?.Db != null)
            {
                context.Db.LogEvent("Door Open", $"Lift door opened at floor {context.CurrentFloor}");
            }
            context.IsDoorOpen = true;
        }

        public void Execute(LiftContext context)
        {
            // Door remains open
        }

        public void Exit(LiftContext context)
        {
            // Transitioning away from open state
        }
    }
}