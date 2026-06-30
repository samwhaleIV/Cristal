namespace Cristal.Pipeline.Nodes {
    public readonly struct OptionalNode<T,TFilter>(TFilter filter,bool enabled):IFilter<T,T>
        where TFilter : IFilter<T,T>
    {
        public T Process(T input) {
            return enabled ? filter.Process(input) : input;
        }
    }
}
