using System.Collections.Generic;
using System.Threading.Tasks;
using SapphireDb_Net.Collection.Prefilter;
using SapphireDb_Net.Command.Query;

namespace SapphireDb_Net.Command.Subscribe
{
    public class SubscribeCommand : QueryCommand
    {
        public SubscribeCommand(string collectionName, string contextName, List<IPrefilter> prefilters) : base(collectionName, contextName, prefilters)
        {
        }
    }

    public interface ISubscribeCommand
    {
    }
}