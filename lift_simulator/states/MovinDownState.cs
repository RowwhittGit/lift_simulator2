using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lift_simulator.States
{
    public class MovingDownState : ILiftState
    {
        public void Enter(LiftContext context)
        {
            context.Db.LogEvent("Lift Moving Down", "Lift started moving down...");
        }

        public void Execute(LiftContext context)
        {
            // Will simulate lift moving down step by step later
        }

        public void Exit(LiftContext context)
        {
            context.Db.LogEvent("Lift Moving Down", "Lift stopped moving down.");
        }
    }
}

