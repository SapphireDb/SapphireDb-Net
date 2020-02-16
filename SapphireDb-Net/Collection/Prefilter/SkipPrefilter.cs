using System.Collections.Generic;

namespace SapphireDb_Net.Collection.Prefilter
{
    public class SkipPrefilter<T> : TakePrefilter<T>
    {
        public SkipPrefilter(int number) : base(number)
        {
        }
        
        public override List<T> Execute(List<T> values)
        {
            return values;
        }

        public override string Hash()
        {
            return $"{PrefilterType},{Number}";
        }
    }
}