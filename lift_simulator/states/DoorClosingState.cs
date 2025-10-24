using lift_simulator.Controllers;
using lift_simulator.Interfaces;
using System.Threading.Tasks;

namespace lift_simulator.States
{
    public class DoorClosingState : ILiftState
    {
        public void Enter(LiftController controller)
        {
            controller.Log("Door is closing...");
            controller.IsDoorOpen = false;
            SimulateDoorClosing(controller);
        }

        public void Exit(LiftController controller)
        {
            controller.Log("Door finished closing");
        }

        public string GetStateName() => "DoorClosingState";

        public void HandleRequest(LiftController controller, string request)
        {
            if (request == "OpenDoor")
            {
                controller.TransitionToState(new DoorOpeningState());
                return;
            }

            if (request == "CloseDoor")
            {
                controller.Log("Door is already closing");
                return;
            }

            if (request.StartsWith("MoveToFloor"))
            {
                controller.Log("Door is closing. Please wait.");
                return;
            }
        }

        private async void SimulateDoorClosing(LiftController controller)
        {
            await Task.Delay(1000);
            controller.TransitionToState(new DoorClosedState());
        }
    }
}
