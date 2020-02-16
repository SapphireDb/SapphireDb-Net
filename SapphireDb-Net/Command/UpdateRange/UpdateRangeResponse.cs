using System.Collections.Generic;
using SapphireDb_Net.Command.Update;

namespace SapphireDb_Net.Command.UpdateRange
{
    public class UpdateRangeResponse : ResponseBase
    {
        public List<UpdateResponse> Results { get; set; }
    }
}