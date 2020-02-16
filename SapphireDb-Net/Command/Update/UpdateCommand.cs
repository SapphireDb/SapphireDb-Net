namespace SapphireDb_Net.Command.Update
{
    public class UpdateCommand : CollectionCommandBase
    {
        public object UpdateValue { get; set; }

        public UpdateCommand(string collectionName, string contextName, object updateValue) : base(collectionName, contextName)
        {
            UpdateValue = updateValue;
        }
    }
}