namespace SapphireDb_Net.Command.CreateRange
{
    public class CreateRangeCommand<T> : CollectionCommandBase
    {
        public T[] Values { get; set; }

        public CreateRangeCommand(string collectionName, string contextName, T[] values) : base(collectionName, contextName)
        {
            Values = values;
        }
    }
}