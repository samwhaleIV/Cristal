namespace Cristal.Pipeline.Filters.Ops {
    public readonly struct Power(float operand):IFilter<float,float> {
        float IFilter<float,float>.Process(float input) => MathF.Pow(input,operand);
    }
}
