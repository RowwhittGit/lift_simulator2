using lift_simulator.Interfaces;
using lift_simulator.Controllers;

namespace lift_simulator.States
{
    public class DoorClosingState : ILiftState
    {
        public void Enter(LiftController controller)
        {
            controller.Log("Door is closing...");
            controller.IsDoorOpen = false;
            controller.StartDoorTimer();
        }

        public void Exit(LiftController controller)
        {
            controller.Log("Door finished closing.");
        }

        public void HandleRequest(LiftController controller, string request)
        {
            if (request == "OpenDoor")
                controller.SetState(new DoorOpeningState());
        }
    }
}
