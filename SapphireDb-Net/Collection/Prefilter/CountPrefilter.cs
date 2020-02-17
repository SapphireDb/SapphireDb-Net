using System.Collections.Generic;
using System.Linq;

namespace SapphireDb_Net.Collection.Prefilter
{
    public class CountPrefilter<T> : PrefilterBase<T, int>, IAfterQueryPrefilter
    {
        public override int Execute(IEnumerable<T> values)
        {
            return values.Count();
        }

        public override string Hash()
        {
            return $"{PrefilterType}";
        }
    }
}