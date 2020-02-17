using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SapphireDb_Net.Collection.Prefilter
{
    public class OrderByPrefilter<T> : PrefilterBase<T, IOrderedEnumerable<T>>
    {
        public string Property { get; set; }

        public bool Descending { get; set; }

        protected readonly Func<T, object> FilterSelector; 
        
        public OrderByPrefilter(string property, SortDirection direction = SortDirection.Ascending)
        {
            Property = property;
            Descending = direction == SortDirection.Descending;

            PropertyInfo propertyInfo = typeof(T).GetProperty(property,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            FilterSelector = (value) => propertyInfo?.GetValue(value);
        }
        
        public override IOrderedEnumerable<T> Execute(IEnumerable<T> values)
        {
            if (Descending)
            {
                return values.OrderByDescending(FilterSelector);
            }

            return values.OrderBy(FilterSelector);
        }

        public override string Hash()
        {
            return $"{PrefilterType},{Property},{Descending}";
        }
    }
    
    public enum SortDirection
    {
        Ascending, Descending
    }
}