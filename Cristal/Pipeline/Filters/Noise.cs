namespace Cristal.Pipeline.Filters {
    public readonly struct Noise(long seed,float scale,float offsetX,float offsetY):IFilter<Point,float> {
        public float Process(Point coordinate) {
            float x = (coordinate.X + offsetX) * scale;
            float y = (coordinate.Y + offsetY) * scale;

            float value = OpenSimplex2.Noise2(seed,x,y);

            //Normalize '-1.0' to '1.0' to '0.0' to '1.0'
            value = MathF.FusedMultiplyAdd(value,0.5f,0.5f);

            return value;
        }
    }
}
