namespace SapphireDb_Net.Command.UpdateRange
{
    public class UpdateRangeCommand<T> : CollectionCommandBase
    {
        public T[] UpdateValues { get; set; }

        public UpdateRangeCommand(string collectionName, string contextName, T[] updateValues) : base(collectionName, contextName)
        {
            UpdateValues = updateValues;
        }
    }
}