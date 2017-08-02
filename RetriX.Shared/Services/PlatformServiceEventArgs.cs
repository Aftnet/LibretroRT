namespace RetriX.Shared.Services
{
    public class FullScreenChangeEventArgs
    {
        public FullScreenChangeType Type { get; private set; }

        public FullScreenChangeEventArgs(FullScreenChangeType type)
        {
            Type = type;
        }
    }

    public class GameStateOperationEventArgs
    {
        public enum GameStateOperationType { Save, Load };

        public GameStateOperationType Type { get; private set; }
        public uint SlotID { get; private set; }

        public GameStateOperationEventArgs(GameStateOperationType type, uint slotID)
        {
            Type = type;
            SlotID = slotID;
        }
    }
}
