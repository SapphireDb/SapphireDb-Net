using Newtonsoft.Json.Linq;

namespace SapphireDb_Net.Command.Update
{
    public class UpdateResponse : ValidatedResponseBase
    {
        public JToken UpdatedObject { get; set; }
    }
}