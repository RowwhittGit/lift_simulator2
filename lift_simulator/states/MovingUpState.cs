using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lift_simulator.States
{
    public class MovingUpState : ILiftState
    {
        public void Enter(LiftContext context)
        {
            context.Db.LogEvent("Lift Moving Up", "Lift started moving up...");
        }

        public void Execute(LiftContext context)
        {
            // Here we can simulate lift moving up step by step later using BackgroundWorker
        }

        public void Exit(LiftContext context)
        {
            context.Db.LogEvent("Lift Moving Up", "Lift stopped moving up.");
        }
    }
}

