using System.Collections.Generic;
using System.Linq;

namespace SapphireDb_Net.Collection.Prefilter
{
    public class LastPrefilter<T> : PrefilterBase<T, T>, IAfterQueryPrefilter
    {
        public override T Execute(IEnumerable<T> values)
        {
            return values.LastOrDefault();
        }

        public override string Hash()
        {
            return $"{PrefilterType}";
        }
    }
}