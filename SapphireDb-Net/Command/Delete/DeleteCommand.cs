namespace SapphireDb_Net.Command.Delete
{
    public class DeleteCommand : CollectionCommandBase
    {
        public object PrimaryKeys { get; set; }

        public DeleteCommand(string collectionName, string contextName, object primaryKeys) : base(collectionName, contextName)
        {
            PrimaryKeys = primaryKeys;
        }
    }
}