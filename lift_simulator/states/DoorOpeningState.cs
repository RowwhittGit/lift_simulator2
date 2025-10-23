using lift_simulator.Interfaces;
using lift_simulator.Controllers;

namespace lift_simulator.States
{
    public class DoorOpeningState : ILiftState
    {
        public void Enter(LiftController controller)
        {
            controller.Log("Door is opening...");
            controller.IsDoorOpen = true;
            controller.StartDoorTimer();
        }

        public void Exit(LiftController controller)
        {
            controller.Log("Door finished opening.");
        }

        public void HandleRequest(LiftController controller, string request)
        {
            if (request == "CloseDoor")
                controller.SetState(new DoorClosingState());
        }
    }
}
