using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lift_simulator
{
    public interface ILiftState
    {
        void Enter(LiftContext context);   // Called when the state starts
        void Execute(LiftContext context); // Called while the lift is in this state
        void Exit(LiftContext context);    // Called when leaving this state
    }
}
