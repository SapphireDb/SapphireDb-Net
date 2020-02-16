using Newtonsoft.Json.Linq;

namespace SapphireDb_Net.Command.Create
{
    public class CreateResponse : ValidatedResponseBase
    {
        public JToken NewObject { get; set; }
    }
}