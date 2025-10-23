using lift_simulator.Interfaces;
using lift_simulator.Controllers;

namespace lift_simulator.States
{
    public class MovingDownState : ILiftState
    {
        public void Enter(LiftController controller)
        {
            controller.Log("Moving down...");
        }

        public void Exit(LiftController controller)
        {
            controller.Log("Stopped moving down.");
        }

        public void HandleRequest(LiftController controller, string request)
        {
            if (request == "Arrived")
                controller.SetState(new DoorOpeningState());
        }
    }
}
