using System.Numerics;

namespace Cristal.Pipeline.Functions {
    public readonly struct Add<T>(T operand):IFunction<T,T> where T : INumber<T> {
        T IFunction<T,T>.Process(T input) {
            return input + operand;
        }
    }
    public readonly struct Subtract<T>(T operand):IFunction<T,T> where T : INumber<T> {
        T IFunction<T,T>.Process(T input) {
            return input - operand;
        }
    }
    public readonly struct Divide<T>(T operand):IFunction<T,T> where T : INumber<T> {
        T IFunction<T,T>.Process(T input) {
            return input / operand;
        }
    }
    public readonly struct Multiply<T>(T operand):IFunction<T,T> where T : INumber<T> {
        T IFunction<T,T>.Process(T input) {
            return input * operand;
        }
    }
    public readonly struct Power(float operand):IFunction<float,float> {
        float IFunction<float,float>.Process(float input) {
            return MathF.Pow(input,operand);
        }
    }
}
