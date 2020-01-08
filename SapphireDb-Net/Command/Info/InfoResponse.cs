using System.Collections.Generic;

namespace SapphireDb_Net.Command.Info
{
    public class InfoResponse : ResponseBase
    {
        public List<string> PrimaryKeys { get; set; }
    }
}