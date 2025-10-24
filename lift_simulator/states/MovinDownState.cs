using lift_simulator.Controllers;
using lift_simulator.Interfaces;
using System.Threading.Tasks;

namespace lift_simulator.States
{
    public class MovingDownState : ILiftState
    {
        public void Enter(LiftController controller)
        {
            controller.Log("Moving down...");
            controller.SetBusy(true);
            SimulateMovement(controller);
        }

        public void Exit(LiftController controller)
        {
            controller.Log("Stopped moving down");
        }

        public string GetStateName() => "MovingDownState";

        public void HandleRequest(LiftController controller, string request)
        {
            // Ignore requests while moving
            if (request.StartsWith("MoveToFloor"))
            {
                controller.Log("Already moving. Request queued.");
                return;
            }

            if (request == "OpenDoor")
            {
                controller.Log("Cannot open door while moving");
                return;
            }

            if (request == "CloseDoor")
            {
                controller.Log("Door is not open");
                return;
            }
        }

        private async void SimulateMovement(LiftController controller)
        {
            await Task.Delay(2000);
            controller.ArriveAtFloor(controller.TargetFloor);
            controller.TransitionToState(new IdleState());
        }
    }
}
