using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using SapphireDb_Net;
using SapphireDb_Net.Collection;
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

            DefaultCollection<object> collection = db.Collection<object>("demo.entries");

            IObservable<List<object>> values = collection.Where(new object[] { "content", "==", "xyc" }).Values();
            
            values.Subscribe((value) =>
            {
                
            });

            // collection.Add(new {content = "Das ist ein test"}, new { content = "Das ist test 2" }).Subscribe((r) =>
            // {
            //     
            // });

            
            
            Console.ReadKey();
        }
    }
}