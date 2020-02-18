using System;
using System.Collections.Generic;
using System.Linq;
using SapphireDb_Net.Command;
using SapphireDb_Net.Command.Create;
using SapphireDb_Net.Command.Update;

namespace SapphireDb_Net.Models
{
    public class CommandResult<T>
    {
        public object Error { get; set; }
        
        private Dictionary<string, string[]> ValidationResults { get; set; }

        public T Value { get; set; }

        public CommandResult(ValidatedResponseBase response)
        {
            Error = response.Error;
            ValidationResults = response.ValidationResults;

            if (response is CreateResponse createResponse)
            {
                Value = createResponse.NewObject.ToObject<T>();
            }
            else if (response is UpdateResponse updateResponse)
            {
                Value = updateResponse.UpdatedObject.ToObject<T>();
            }
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