using lift_simulator.Controllers;
using lift_simulator.Interfaces;

namespace lift_simulator.States
{
    public class IdleState : ILiftState
    {
        public void Enter(LiftController controller)
        {
            controller.Log("Lift is idle.");
        }

        public void Exit(LiftController controller)
        {
            controller.Log("Lift leaving idle state.");
        }

        public void HandleRequest(LiftController controller, string request)
        {
            switch (request)
            {
                case "OpenDoor":
                    controller.SetState(new DoorOpeningState());
                    break;
                case "MoveToFloor1":
                    controller.SetState(new MovingUpState());
                    break;
                case "MoveToFloor0":
                    controller.SetState(new MovingDownState());
                    break;
                default:
                    controller.Log($"Unhandled request {request} in IdleState.");
                    break;
            }
        }
    }
}