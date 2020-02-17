using System.Collections.Generic;
using SapphireDb_Net.Helper;

namespace SapphireDb_Net.Collection.Prefilter
{
    public class WherePrefilter<T> : PrefilterBase<T, IEnumerable<T>>
    {
        public object[] Conditions { get; set; }

        public WherePrefilter(object[] conditions)
        {
            Conditions = conditions;
        }
        
        public override IEnumerable<T> Execute(IEnumerable<T> values)
        {
            return values;
        }

        public override string Hash()
        {
            return $"{PrefilterType},{JsonHelper.Serialize(Conditions)}";
        }
    }
}