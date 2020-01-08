using System;
using System.Collections.Generic;
using SapphireDb_Net;
using SapphireDb_Net.Collection.Prefilter;
using SapphireDb_Net.Command.Subscribe;
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
            SapphireDb db = new SapphireDb(options);

            db.Collection<object>("demo.entries").Snapshot().Subscribe((value) =>
            {
                
            });

            Console.ReadKey();
        }
    }
}