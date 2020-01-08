using System;

namespace SapphireDb_Net.Command.Unsubscribe
{
    public class UnsubscribeCommand : CollectionCommandBase
    {
        public UnsubscribeCommand(string collectionName, string contextName, Guid referenceId) : base(collectionName, contextName)
        {
            ReferenceId = referenceId;
        }
    }
}