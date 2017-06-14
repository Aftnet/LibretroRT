namespace RetriX.Shared.Messages
{
    public class StateOperationMessage
    {
        public enum Types { Save, Load };

        public Types Type { get; private set; }
        public uint SlotID { get; private set; }

        public StateOperationMessage(Types type, uint slotID)
        {
            Type = type;
            SlotID = slotID;
        }
    }
}
