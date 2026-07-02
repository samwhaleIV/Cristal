namespace Cristal.Pipeline {
    public readonly struct OptionalNode<T,TFunction>(TFunction function,bool enabled):IFunction<T,T>
        where TFunction : IFunction<T,T>
    {
        public T Process(T input) {
            return enabled ? function.Process(input) : input;
        }
    }
}
