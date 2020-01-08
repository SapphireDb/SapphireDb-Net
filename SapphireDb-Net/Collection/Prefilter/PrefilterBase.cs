using System.Collections.Generic;

namespace SapphireDb_Net.Collection.Prefilter
{
    public abstract class PrefilterBase<TInput, TOutput> : IPrefilter
    {
        public string PrefilterType { get; set; }

        public PrefilterBase()
        {
            PrefilterType = GetType().Name;
        }

        public abstract TOutput Execute(List<TInput> values);

        public abstract string Hash();
    }
}