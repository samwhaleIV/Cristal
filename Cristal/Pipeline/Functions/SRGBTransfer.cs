namespace Cristal.Pipeline.Functions {
    public readonly struct SRGBTransfer:IFunction<float,float> {
        // Note: This is the full, standardized sRGB transfer function with a linear shadow component. Using a power operator with gamma '1.0 / 2.2' is usually sufficient.
        public float Process(float value) {
            if(value <= 0.0031308f) {
                return value * 12.92f;
            } else {
                const float GAMMA = (float)(1.0 / 2.4);
                return 1.055f * MathF.Pow(value,GAMMA) - 0.055f;
            }
        }
    }
}
