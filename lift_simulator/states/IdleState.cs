using lift_simulator.Controllers;
using lift_simulator.Interfaces;

namespace lift_simulator.States
{
    public class IdleState : ILiftState
    {
        public void Enter(LiftController controller)
        {
            controller.Log("Lift is now idle");
            controller.SetBusy(false);
        }

        public void Exit(LiftController controller)
        {
            controller.Log("Lift leaving idle state");
        }

        public string GetStateName() => "IdleState";

        public void HandleRequest(LiftController controller, string request)
        {
            if (request.StartsWith("MoveToFloor:"))
            {
                int targetFloor = int.Parse(request.Split(':')[1]);
                controller.MoveToFloor(targetFloor);
                return;
            }

            if (request == "OpenDoor")
            {
                controller.TransitionToState(new DoorOpeningState());
                return;
            }

            if (request == "CloseDoor")
            {
                controller.TransitionToState(new DoorClosingState());
                return;
            }

            controller.Log($"Ignoring request while idle: {request}");
        }
    }
}
