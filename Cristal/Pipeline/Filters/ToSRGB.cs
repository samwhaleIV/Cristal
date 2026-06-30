namespace Cristal.Pipeline.Filters {
    public readonly struct ToSRGB:IFilter<float,float> {
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
