namespace Cristal.Pipeline.Stateless {
    public static class ColorNoise {

        public static float White(int x,int y,int seed) {
            int n = x + y * 57 + seed * 131;
            n = (n << 13) ^ n;
            n = n * (n * n * 15731 + 789221) + 1376312589;
            return (n & 0x7fffffff) / 2147483648f;
        }

        // Interpolates between the rigid integer grid points
        private static float SmoothNoise(float x,float y,int seed) {
            // Get the integer boundaries
            int intX = (int)MathF.Floor(x);
            int intY = (int)MathF.Floor(y);

            // Get the fractional part for interpolation
            float fractX = x - intX;
            float fractY = y - intY;

            float u = fractX * fractX * (3f - 2f * fractX);
            float v = fractY * fractY * (3f - 2f * fractY);

            // Fetch the 4 corners of the white noise grid
            float v1 = White(intX,intY,seed);
            float v2 = White(intX + 1,intY,seed);
            float v3 = White(intX,intY + 1,seed);
            float v4 = White(intX + 1,intY + 1,seed);

            // Bilinear interpolation
            float i1 = MathF.Round(v1 + u * (v2 - v1),6);
            float i1_lerp = v1 + u * (v2 - v1);
            float i2_lerp = v3 + u * (v4 - v3);

            return i1_lerp + v * (i2_lerp - i1_lerp);
        }

        // Fractal Brownian motion. Layers noise to create specific frequency spectrums
        private static float GenerateFractalNoise(float x,float y,int seed,int octaves,float persistence,float lacunarity) {
            float total = 0f;
            float frequency = 1f;
            float amplitude = 1f;
            float maxValue = 0f;

            for(int i = 0;i < octaves;i++) {
                total += SmoothNoise(x * frequency,y * frequency,seed) * amplitude;

                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return total / maxValue;
        }

        // Pink Noise (~1/f spectrum)
        // Amplitudes halve with each octave. Great for standard procedural generation.
        public static float Pink(float x,float y,int seed,int octaves = 5) {
            return GenerateFractalNoise(x,y,seed,octaves,persistence: 0.5f,lacunarity: 2.0f);
        }

        // Brown Noise (~1/f^2 spectrum)
        // Amplitudes drop off much faster. Results in deep, rolling, low-frequency shapes.
        public static float Brown(float x,float y,int seed,int octaves = 5) {
            return GenerateFractalNoise(x,y,seed,octaves,persistence: 0.25f,lacunarity: 2.0f);
        }
    }
}
