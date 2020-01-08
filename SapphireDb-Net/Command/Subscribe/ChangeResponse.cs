using Newtonsoft.Json.Linq;

namespace SapphireDb_Net.Command.Subscribe
{
    public class ChangeResponse : ResponseBase
    {
        public ChangeState State { get; set; }
        
        public JToken Value { get; set; }
        
        public enum ChangeState
        {
            Added, Deleted, Modified
        }
    }
}