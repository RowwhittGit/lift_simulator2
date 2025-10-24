using lift_simulator.Controllers;
using lift_simulator.Interfaces;

namespace lift_simulator.States
{
    public class DoorOpenState : ILiftState
    {
        public void Enter(LiftController controller)
        {
            controller.Log("Door is open");
            controller.IsDoorOpen = true;
        }

        public void Exit(LiftController controller)
        {
            controller.Log("Door was open, now closing");
        }

        public string GetStateName() => "DoorOpenState";

        public void HandleRequest(LiftController controller, string request)
        {
            if (request == "CloseDoor")
            {
                controller.TransitionToState(new DoorClosingState());
                return;
            }

            if (request == "OpenDoor")
            {
                controller.Log("Door is already open");
                return;
            }

            if (request.StartsWith("MoveToFloor"))
            {
                controller.Log("Cannot move - door is open. Please close the door first.");
                return;
            }
        }
    }
}
