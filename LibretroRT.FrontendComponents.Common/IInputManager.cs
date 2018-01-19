using LibRetriX;

namespace LibretroRT.FrontendComponents.Common
{
    public interface IInputManager
    {
        void InjectInputPlayer1(InputTypes inputType);
        void PollInput();
        short GetInputState(uint port, InputTypes inputType);
    }
}