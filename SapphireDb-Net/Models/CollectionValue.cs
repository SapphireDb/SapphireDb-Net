using System;
using System.Reactive.Subjects;

namespace SapphireDb_Net.Models
{
    public class CollectionValue
    {
        public Guid ReferenceId { get; set; }
        
        public ISubject<object> Subject { get; set; }

        public IDisposable SocketSubscription { get; set; }

        public CollectionValue(Guid referenceId)
        {
            ReferenceId = referenceId;
            Subject = new ReplaySubject<object>(1);
        }
    }
}