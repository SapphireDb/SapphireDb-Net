using System;

namespace SapphireDb_Net.Command.Connection
{
    public class ConnectionResponse : ResponseBase
    {
        public Guid ConnectionId { get; set; }

        public bool AuthTokenValid { get; set; }
    }
}