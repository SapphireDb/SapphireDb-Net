using System;
using System.Collections.Generic;

namespace SapphireDbNetDemo
{
    public class User
    {
        public Guid Id { get; set; }

        public List<UserEntry> Entries { get; set; }

        public string Name { get; set; }
    }

    public class UserEntry
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        // public User User { get; set; }

        public string Content { get; set; }
    }
}