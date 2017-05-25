namespace LibretroRT.InputManager
{
    public interface IInputManager
    {
        short GetInputState(uint port, InputTypes inputType);
        void PollInput();
    }
}