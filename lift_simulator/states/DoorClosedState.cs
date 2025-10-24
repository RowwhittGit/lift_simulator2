using lift_simulator.Controllers;
using lift_simulator.Interfaces;

namespace lift_simulator.States
{
    public class DoorClosedState : ILiftState
    {
        public void Enter(LiftController controller)
        {
            controller.Log("Door is closed");
            controller.IsDoorOpen = false;
            controller.ProcessQueue();
        }

        public void Exit(LiftController controller)
        {
            controller.Log("Door state changing");
        }

        public string GetStateName() => "DoorClosedState";

        public void HandleRequest(LiftController controller, string request)
        {
            if (request == "OpenDoor")
            {
                controller.TransitionToState(new DoorOpeningState());
                return;
            }

            if (request.StartsWith("MoveToFloor"))
            {
                int targetFloor = int.Parse(request.Split(':')[1]);
                controller.MoveToFloor(targetFloor);
                return;
            }

            controller.TransitionToState(new IdleState());
        }
    }
}