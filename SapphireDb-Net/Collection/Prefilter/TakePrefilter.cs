using System.Collections.Generic;

namespace SapphireDb_Net.Collection.Prefilter
{
    public class TakePrefilter<T> : PrefilterBase<T, IEnumerable<T>>
    {
        public int Number { get; set; }

        public TakePrefilter(int number)
        {
            Number = number;
        }
        
        public override IEnumerable<T> Execute(IEnumerable<T> values)
        {
            return values;
        }

        public override string Hash()
        {
            return $"{PrefilterType},{Number}";
        }
    }
}