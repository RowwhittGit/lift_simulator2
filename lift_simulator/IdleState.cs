using lift_simulator.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lift_simulator.States
{
    public class IdleState : ILiftState
    {
        public void Enter(LiftContext context)
        {
            context.Db.LogEvent("Lift Idle", $"Lift is idle at floor {context.CurrentFloor}.");
        }

        public void Execute(LiftContext context)
        {
            // Nothing happens here unless user presses a button
        }

        public void Exit(LiftContext context)
        {
            // Nothing special yet
        }
    }
}
