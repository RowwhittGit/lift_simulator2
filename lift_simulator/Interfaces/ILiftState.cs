using lift_simulator.Controllers;

namespace lift_simulator.Interfaces
{
    public interface ILiftState
    {
        void Enter(LiftController controller);
        void Exit(LiftController controller);
        void HandleRequest(LiftController controller, string request);
        string GetStateName();  
    }
}