namespace Cristal.Pipeline {
    public interface IFilter<TIn,TOut> {
        public TOut Process(TIn input);
    }
}
