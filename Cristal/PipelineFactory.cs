using Cristal.Pipeline;

namespace Cristal {
    public static class PipelineFactory {
        public static EntryNode<TIn,TOut,TFilter> CreatePipeline<TIn,TOut,TFilter>(TFilter filter)
            where TFilter : IFunction<TIn,TOut>
        {
            return new EntryNode<TIn,TOut,TFilter>(filter);
        }

        public static EntryNode<TIn,TOut,TFilter> CreatePipeline<TIn,TOut,TFilter>()
            where TFilter : IFunction<TIn,TOut>, new()
        {
            return new EntryNode<TIn,TOut,TFilter>(new TFilter());
        }
    }
}
