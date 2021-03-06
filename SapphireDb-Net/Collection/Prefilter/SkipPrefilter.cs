﻿using System.Collections.Generic;

namespace SapphireDb_Net.Collection.Prefilter
{
    public class SkipPrefilter<T> : TakePrefilter<T>
    {
        public SkipPrefilter(int number) : base(number)
        {
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