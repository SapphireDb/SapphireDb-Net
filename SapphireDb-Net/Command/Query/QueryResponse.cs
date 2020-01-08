using Newtonsoft.Json.Linq;

namespace SapphireDb_Net.Command.Query
{
    public class QueryResponse : ResponseBase
    {
        public JToken Result { get; set; }
    }
}