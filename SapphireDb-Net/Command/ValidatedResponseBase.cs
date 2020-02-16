using System.Collections.Generic;

namespace SapphireDb_Net.Command
{
    public class ValidatedResponseBase : ResponseBase
    {
        public Dictionary<string, string[]> ValidationResults { get; set; }
    }
}