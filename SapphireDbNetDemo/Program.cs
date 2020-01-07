using System;
using SapphireDb_Net.Connection;
using SapphireDb_Net.Options;

namespace SapphireDbNetDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            SapphireDbOptions options = new SapphireDbOptions()
            {
                ServerBaseUrl = "localhost:5000",
                UseSsl = false,
                ApiKey = "net_client",
                ApiSecret = "pw1234"
            };
            ConnectionManager c = new ConnectionManager(options);
            c.SendCommand(null);

            Console.ReadKey();
        }
    }
}