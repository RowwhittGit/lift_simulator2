using lift_simulator.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lift_simulator.Interfaces
{
    public interface ILiftState
    {
        void Enter(LiftController controller);
        void Exit(LiftController controller);
        void HandleRequest(LiftController controller, string request);
    }
}


