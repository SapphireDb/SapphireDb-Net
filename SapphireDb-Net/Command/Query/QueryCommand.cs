using System.Collections.Generic;
using System.Threading.Tasks;
using SapphireDb_Net.Collection.Prefilter;

namespace SapphireDb_Net.Command.Query
{
    public class QueryCommand : CollectionCommandBase
    {
        public List<IPrefilter> Prefilters { get; set; }
        
        public QueryCommand(string collectionName, string contextName, List<IPrefilter> prefilters) : base(collectionName, contextName)
        {
            Prefilters = prefilters;
        }
    }
}