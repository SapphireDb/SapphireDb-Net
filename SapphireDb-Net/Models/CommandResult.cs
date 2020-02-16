using System.Collections.Generic;
using System.Linq;

namespace SapphireDb_Net.Models
{
    public class CommandResult<T>
    {
        public object Error { get; set; }
        
        private Dictionary<string, string[]> ValidationResults { get; set; }

        public T Value { get; set; }

        public CommandResult(object error, Dictionary<string, string[]> validationResults, T value)
        {
            Error = error;
            ValidationResults = validationResults;
            Value = value;
        }

        public bool HasSuccess()
        {
            return !HasErrors() && !HasValidationErrors();
        }

        public bool HasErrors()
        {
            return Error != null;
        }

        public bool HasValidationErrors()
        {
            return ValidationResults.Any();
        }
    }
}