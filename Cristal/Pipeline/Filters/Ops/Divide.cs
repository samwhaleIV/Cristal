using System.Numerics;

namespace Cristal.Pipeline.Filters.Ops {
    public readonly struct Divide<T>(T operand):IFilter<T,T> where T : INumber<T> {
        T IFilter<T,T>.Process(T input) => input / operand;
    }
}
