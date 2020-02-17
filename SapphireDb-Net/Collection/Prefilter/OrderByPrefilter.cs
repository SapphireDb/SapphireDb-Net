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

        private readonly Func<T, object> _filterSelector; 
        
        public OrderByPrefilter(string property, SortDirection direction = SortDirection.Ascending)
        {
            Property = property;
            Descending = direction == SortDirection.Descending;

            PropertyInfo propertyInfo = typeof(T).GetProperty(property,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            _filterSelector = (value) => propertyInfo?.GetValue(value);
        }
        
        public override IOrderedEnumerable<T> Execute(List<T> values)
        {
            if (Descending)
            {
                return values.OrderByDescending(_filterSelector);
            }

            return values.OrderBy(_filterSelector);
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