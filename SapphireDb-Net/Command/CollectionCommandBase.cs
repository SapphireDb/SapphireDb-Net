namespace SapphireDb_Net.Command
{
    public class CollectionCommandBase : CommandBase
    {
        public string CollectionName { get; set; }
        
        public string ContextName { get; set; }
        
        public CollectionCommandBase(string collectionName, string contextName)
        {
            CollectionName = collectionName;
            ContextName = contextName;
        }
    }
}