using System.Reactive.Subjects;
using SapphireDb_Net.Command;

namespace SapphireDb_Net.Models
{
    class CommandReference
    {
        public ISubject<ResponseBase> Subject { get; set; }

        public bool Keep { get; set; }
    }
}