using System.Collections.Generic;
using SapphireDb_Net.Helper;

namespace SapphireDb_Net.Collection.Prefilter
{
    public class WherePrefilter<T> : PrefilterBase<T, List<T>>
    {
        public object Conditions { get; set; }
        
        public override List<T> Execute(List<T> values)
        {
            return values;
        }

        public override string Hash()
        {
            return $"{PrefilterType},{JsonHelper.Serialize(Conditions)}";
        }
    }
}