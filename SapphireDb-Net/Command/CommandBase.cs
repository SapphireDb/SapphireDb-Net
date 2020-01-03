using System;

namespace SapphireDb_Net.Command
{
    public class CommandBase
    {
        public string CommandType { get; set; }

        public Guid ReferenceId { get; set; }

        public CommandBase(string commandType)
        {
            CommandType = commandType;
            ReferenceId = Guid.NewGuid();
        }
    }
}