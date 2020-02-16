using System.Collections.Generic;
using SapphireDb_Net.Command.Create;

namespace SapphireDb_Net.Command.CreateRange
{
    public class CreateRangeResponse : ResponseBase
    {
        public List<CreateResponse> Results { get; set; }
    }
}