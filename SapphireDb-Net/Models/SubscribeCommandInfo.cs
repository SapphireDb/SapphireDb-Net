using SapphireDb_Net.Command;

namespace SapphireDb_Net.Models
{
    class SubscribeCommandInfo
    {
        public CommandBase Command { get; set; }

        public bool SendWithAuthToken { get; set; }
    }
}