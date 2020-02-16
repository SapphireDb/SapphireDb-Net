namespace SapphireDb_Net.Command.DeleteRange
{
    public class DeleteRangeCommand<T> : CollectionCommandBase
    {
        public T[] PrimaryKeyList { get; set; }

        public DeleteRangeCommand(string collectionName, string contextName, T[] primaryKeyList) : base(collectionName, contextName)
        {
            PrimaryKeyList = primaryKeyList;
        }
    }
}