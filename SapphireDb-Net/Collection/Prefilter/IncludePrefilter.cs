using System.Collections.Generic;

namespace SapphireDb_Net.Collection.Prefilter
{
    public class IncludePrefilter<T> : PrefilterBase<T, IEnumerable<T>>
    {
        public string Include { get; set; }

        public IncludePrefilter(string include)
        {
            Include = include;
        }
        
        public override IEnumerable<T> Execute(IEnumerable<T> values)
        {
            return values;
        }

        public override string Hash()
        {
            return $"{PrefilterType},{Include}";
        }
    }
}