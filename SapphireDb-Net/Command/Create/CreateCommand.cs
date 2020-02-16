namespace SapphireDb_Net.Command.Create
{
    public class CreateCommand : CollectionCommandBase
    {
        public object Value { get; set; }

        public CreateCommand(string collectionName, string contextName, object value) : base(collectionName, contextName)
        {
            Value = value;
        }
    }
}