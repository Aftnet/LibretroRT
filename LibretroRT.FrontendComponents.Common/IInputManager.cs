namespace LibretroRT.FrontendComponents.Common
{
    public interface IInputManager
    {
        void PollInput();
        short GetInputState(uint port, InputTypes inputType);
    }
}