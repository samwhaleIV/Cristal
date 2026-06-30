namespace Cristal.Pipeline.Filters {
    public readonly struct Noise(long seed,float scale):IFilter<Point,float> {
        public float Process(Point coordinate) {
            float value = OpenSimplex2S.Noise2_ImproveX(seed,coordinate.X * scale,coordinate.Y * scale); //might need half pixel offset

            // Value is in range '-1.0' to '1.0', apply basic formula to align to '0.0' to '1.0.
            value = MathF.FusedMultiplyAdd(value,0.5f,0.5f);

            return value;
        }
    }
}
