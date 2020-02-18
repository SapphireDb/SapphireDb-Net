using System.Collections.Generic;
using SapphireDb_Net.Command.Delete;

namespace SapphireDb_Net.Command.DeleteRange
{
    public class DeleteRangeResponse : ResponseBase
    {
        public List<DeleteResponse> Results { get; set; }
    }
}