namespace Cristal.Pipeline.Nodes {
    public readonly struct EntryNode<TIn,TOut,TFilter>(TFilter filter) : IFilter<TIn,TOut>
        where TFilter : IFilter<TIn,TOut>
    {
        public TOut Process(TIn input) {
            return filter.Process(input);
        }

        public Node<TIn,TOut,TNextOut,EntryNode<TIn,TOut,TFilter>,TNextFilter> Append<TNextOut,TNextFilter>(TNextFilter nextFilter)
            where TNextFilter : IFilter<TOut,TNextOut>
        {
            return new Node<TIn,TOut,TNextOut,EntryNode<TIn,TOut,TFilter>,TNextFilter>(this,nextFilter);
        }

        public Node<TIn,TOut,TNextOut,EntryNode<TIn,TOut,TFilter>,TNextFilter> Append<TNextOut,TNextFilter>()
            where TNextFilter : IFilter<TOut,TNextOut>, new()
        {
            return new Node<TIn,TOut,TNextOut,EntryNode<TIn,TOut,TFilter>,TNextFilter>(this,new TNextFilter());
        }

        public Node<TIn,TOut,TOut,EntryNode<TIn,TOut,TFilter>,OptionalNode<TOut,TNextFilter>> AppendOptional<TNextFilter>(TNextFilter nextFilter,bool enabled)
            where TNextFilter : IFilter<TOut,TOut>
        {
            return new Node<TIn,TOut,TOut,EntryNode<TIn,TOut,TFilter>,OptionalNode<TOut,TNextFilter>>(this,new OptionalNode<TOut,TNextFilter>(nextFilter,enabled));
        }

        public Node<TIn,TOut,TOut,EntryNode<TIn,TOut,TFilter>,OptionalNode<TOut,TNextFilter>> AppendOptional<TNextFilter>(bool enabled)
            where TNextFilter : IFilter<TOut,TOut>, new()
        {
            return new Node<TIn,TOut,TOut,EntryNode<TIn,TOut,TFilter>,OptionalNode<TOut,TNextFilter>>(this,new OptionalNode<TOut,TNextFilter>(new TNextFilter(),enabled));
        }
    }
}
