using System;

namespace SapphireDb_Net.Command
{
    public class ResponseBase
    {
        public string ResponseType { get; set; }

        public Guid? ReferenceId { get; set; }

        public object Error { get; set; }
    }
}