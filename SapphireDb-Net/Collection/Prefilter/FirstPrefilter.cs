using System.Collections.Generic;
using System.Linq;

namespace SapphireDb_Net.Collection.Prefilter
{
    public class FirstPrefilter<T> : PrefilterBase<T, T>, IAfterQueryPrefilter
    {
        public override T Execute(IEnumerable<T> values)
        {
            return values.FirstOrDefault();
        }

        public override string Hash()
        {
            return $"{PrefilterType}";
        }
    }
}