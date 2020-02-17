using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SapphireDb_Net.Collection.Prefilter
{
    public class ThenOrderByPrefilter<T> : OrderByPrefilter<T>
    {
        public ThenOrderByPrefilter(string property, SortDirection direction = SortDirection.Ascending) : base(property, direction) { }
        
        public override IOrderedEnumerable<T> Execute(IEnumerable<T> values)
        {
            if (Descending)
            {
                return ((IOrderedEnumerable<T>)values).ThenByDescending(FilterSelector);
            }

            return ((IOrderedEnumerable<T>)values).ThenBy(FilterSelector);
        }
    }
}