namespace Cristal.Pipeline {
    public readonly struct EntryNode<TIn,TOut,TFunction>(TFunction function) : IFunction<TIn,TOut>
        where TFunction : IFunction<TIn,TOut>
    {
        public TOut Process(TIn input) {
            return function.Process(input);
        }

        public Node<TIn,TOut,TNextOut,EntryNode<TIn,TOut,TFunction>,TNext> Append<TNextOut,TNext>(TNext next)
            where TNext : IFunction<TOut,TNextOut>
        {
            return new Node<TIn,TOut,TNextOut,EntryNode<TIn,TOut,TFunction>,TNext>(this,next);
        }

        public Node<TIn,TOut,TNextOut,EntryNode<TIn,TOut,TFunction>,TNext> Append<TNextOut,TNext>()
            where TNext : IFunction<TOut,TNextOut>, new()
        {
            return new Node<TIn,TOut,TNextOut,EntryNode<TIn,TOut,TFunction>,TNext>(this,new TNext());
        }

        public Node<TIn,TOut,TOut,EntryNode<TIn,TOut,TFunction>,OptionalNode<TOut,TNext>> AppendOptional<TNext>(TNext next,bool enabled)
            where TNext : IFunction<TOut,TOut>
        {
            return new Node<TIn,TOut,TOut,EntryNode<TIn,TOut,TFunction>,OptionalNode<TOut,TNext>>(this,new OptionalNode<TOut,TNext>(next,enabled));
        }

        public Node<TIn,TOut,TOut,EntryNode<TIn,TOut,TFunction>,OptionalNode<TOut,TNext>> AppendOptional<TNext>(bool enabled)
            where TNext : IFunction<TOut,TOut>, new()
        {
            return new Node<TIn,TOut,TOut,EntryNode<TIn,TOut,TFunction>,OptionalNode<TOut,TNext>>(this,new OptionalNode<TOut,TNext>(new TNext(),enabled));
        }
    }
}
