using System.Collections.Generic;

namespace SapphireDb_Net.Collection.Prefilter
{
    public class SelectPrefilter<T> : PrefilterBase<T, object[]>, IAfterQueryPrefilter
    {
        public string[] Properties { get; set; }

        public SelectPrefilter(string[] properties)
        {
            Properties = properties;
        }
        
        public override object[] Execute(IEnumerable<T> values)
        {
            return new object[] { };
        }

        public override string Hash()
        {
            return $"{PrefilterType}";
        }
    }
}