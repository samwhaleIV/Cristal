namespace Cristal.Pipeline {
    public readonly struct Node<TParentIn,TParentOut,TSelfOut,TParent,TFunction>(TParent parent,TFunction function) : IFunction<TParentIn,TSelfOut>
        where TParent : IFunction<TParentIn,TParentOut>
        where TFunction : IFunction<TParentOut,TSelfOut>
    {
        public TSelfOut Process(TParentIn input) {
            return function.Process(parent.Process(input));
        }

        public Node<TParentIn,TSelfOut,TNextOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFunction>,TNext> Append<TNextOut,TNext>(TNext next)
            where TNext : IFunction<TSelfOut,TNextOut>
        {
            return new Node<TParentIn,TSelfOut,TNextOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFunction>,TNext>(this,next);
        }

        public Node<TParentIn,TSelfOut,TNextOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFunction>,TNext> Append<TNextOut,TNext>()
            where TNext : IFunction<TSelfOut,TNextOut>, new()
        {
            return new Node<TParentIn,TSelfOut,TNextOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFunction>,TNext>(this,new TNext());
        }

        public Node<TParentIn,TSelfOut,TSelfOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFunction>,OptionalNode<TSelfOut,TNext>> AppendOptional<TNext>(TNext next,bool enabled)
            where TNext : IFunction<TSelfOut,TSelfOut>
        {
            return new Node<TParentIn,TSelfOut,TSelfOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFunction>,OptionalNode<TSelfOut,TNext>>(this,new OptionalNode<TSelfOut,TNext>(next,enabled));
        }

        public Node<TParentIn,TSelfOut,TSelfOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFunction>,OptionalNode<TSelfOut,TNext>> AppendOptional<TNext>(bool enabled)
            where TNext : IFunction<TSelfOut,TSelfOut>, new()
        {
            return new Node<TParentIn,TSelfOut,TSelfOut,Node<TParentIn,TParentOut,TSelfOut,TParent,TFunction>,OptionalNode<TSelfOut,TNext>>(this,new OptionalNode<TSelfOut,TNext>(new TNext(),enabled));
        }
    }
}
