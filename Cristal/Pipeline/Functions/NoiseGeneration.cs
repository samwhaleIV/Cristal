using Cristal.Pipeline.Stateless;

namespace Cristal.Pipeline.Functions {
    public readonly struct OpenSimplexNoise(long seed,float scale,float offsetX,float offsetY):IFunction<Coordinate,float> {
        public float Process(Coordinate coordinate) {
            float x = (coordinate.X + offsetX) * scale;
            float y = (coordinate.Y + offsetY) * scale;

            float value = OpenSimplex2.Noise2(seed,x,y);

            //Normalize '-1.0' to '1.0' to '0.0' to '1.0'
            value = value * 0.5f + 0.5f;

            return value;
        }
    }

    public readonly struct WhiteNoise(int seed):IFunction<Coordinate,float> {
        public float Process(Coordinate coordinate) {
            return ColorNoise.White(coordinate.X,coordinate.Y,seed);
        }
    }

    public readonly struct PinkNoise(int seed):IFunction<Coordinate,float> {
        public float Process(Coordinate coordinate) {
            return ColorNoise.Pink(coordinate.X,coordinate.Y,seed);
        }
    }

    public readonly struct BrownNoise(int seed):IFunction<Coordinate,float> {
        public float Process(Coordinate coordinate) {
            return ColorNoise.Brown(coordinate.X,coordinate.Y,seed);
        }
    }
}
