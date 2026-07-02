namespace Cristal.Pipeline {
    public interface IFunction<TIn,TOut> {
        public TOut Process(TIn input);
    }
}
