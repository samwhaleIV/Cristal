namespace Cristal.Pipeline.Nodes {
    public readonly struct Node<TParentIn,TParentOut,TSelfOut,TParent,TFilter>(TParent parent,TFilter filter) : IFilter<TParentIn,TSelfOut>
        where TParent : IFilter<TParentIn,TParentOut>
        where TFilter : IFilter<TParentOut,TSelfOut>
    {
        public TSelfOut Process(TParentIn input) {
            return filter.Process(parent.Process(input));
        }

        public Node<TParentIn,TSelfOut,TNextOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFilter>,TNextFilter> Append<TNextOut,TNextFilter>(TNextFilter nextFilter)
            where TNextFilter : IFilter<TSelfOut,TNextOut>
        {
            return new Node<TParentIn,TSelfOut,TNextOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFilter>,TNextFilter>(this,nextFilter);
        }

        public Node<TParentIn,TSelfOut,TNextOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFilter>,TNextProcessor> Append<TNextOut,TNextProcessor>()
            where TNextProcessor : IFilter<TSelfOut,TNextOut>, new()
        {
            return new Node<TParentIn,TSelfOut,TNextOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFilter>,TNextProcessor>(this,new TNextProcessor());
        }

        public Node<TParentIn,TSelfOut,TSelfOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFilter>,OptionalNode<TSelfOut,TNextFilter>> AppendOptional<TNextFilter>(TNextFilter nextFilter,bool enabled)
            where TNextFilter : IFilter<TSelfOut,TSelfOut>
        {
            return new Node<TParentIn,TSelfOut,TSelfOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFilter>,OptionalNode<TSelfOut,TNextFilter>>(this,new OptionalNode<TSelfOut,TNextFilter>(nextFilter,enabled));
        }

        public Node<TParentIn,TSelfOut,TSelfOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFilter>,OptionalNode<TSelfOut,TNextFilter>> AppendOptional<TNextFilter>(bool enabled)
            where TNextFilter : IFilter<TSelfOut,TSelfOut>, new()
        {
            return new Node<TParentIn,TSelfOut,TSelfOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFilter>,OptionalNode<TSelfOut,TNextFilter>>(this,new OptionalNode<TSelfOut,TNextFilter>(new TNextFilter(),enabled));
        }
    }
}
