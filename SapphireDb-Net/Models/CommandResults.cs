using System.Collections.Generic;
using System.Linq;

namespace SapphireDb_Net.Models
{
    public class CommandResults<T>
    {
        public List<CommandResult<T>> Results { get; set; }

        public CommandResults(List<CommandResult<T>> results)
        {
            Results = results;
        }

        public bool HasSuccess()
        {
            return !HasErrors() && !HasValidationErrors();
        }

        public bool HasErrors()
        {
            return Results.Any(r => r.HasErrors());
        }

        public bool HasValidationErrors()
        {
            return Results.Any(r => r.HasValidationErrors());
        }
    }
}