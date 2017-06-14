namespace RetriX.Shared.Services
{
    public class GameStateOperationEventArgs
    {
        public enum Types { Save, Load };

        public Types Type { get; private set; }
        public uint SlotID { get; private set; }

        public GameStateOperationEventArgs(Types type, uint slotID)
        {
            Type = type;
            SlotID = slotID;
        }
    }
}
