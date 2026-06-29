using System.Buffers;

namespace Cristal {
    /// <summary>
    /// TODO
    /// </summary>
    public sealed class Factory {
        /// <summary>
        /// Source of pseudo-random number generation (PRNG) used in procedural generation routines.
        /// </summary>
        private readonly Random _random;

        private readonly ArrayPool<byte> _arrayPool = ArrayPool<byte>.Create();

        /// <summary>
        /// Create a <c>CristalFactory</c> with a <c>Random</c> object that may be shared with another system.
        /// Parameter <paramref name="externalRandom"/> will be preserved locally.
        /// </summary>
        /// <param name="externalRandom">External <c>Random</c> object. Reference will be retained by the new <c>CristalFactory</c>.</param>
        public Factory(Random externalRandom) {
            _random = externalRandom;
        }

        /// <summary>
        /// Create a <c>CristalFactory</c> with a unique <c>Random</c> object.
        /// Starting seed is determined by the default <c>System.Random</c> implementation.
        /// See <see cref="Random()"/> for more information.
        /// </summary>
        public Factory() {
            _random = new Random();
        }

        /// <summary>
        /// Create a <c>CristalFactory</c> with a provided starting seed.
        /// See <see cref="Random(int)"/> for more information on pseudo-random number generation and seeding.
        /// </summary>
        /// <param name="seed">
        /// Initial seed provided to a pseudo-random number generator (PRNG). Using the same seed across object instances or program lifetimes allows for deterministic behaviors.
        /// </param>
        public Factory(int seed) {
            _random = new Random(seed);
        }

        public Texture<float> CreateNoiseTexture(TextureSize textureSize,float scale,long? seed = null) {

            Texture<float> texture = new(textureSize,_arrayPool);
            Span<float> data = texture.Data;

            long noiseSeed = seed ?? _random.NextInt64();

            // Noise scales vertically with 'size.Height'
            double pixelScale = 1.0 / (textureSize.Height - 1) * scale;

            for(int y = 0; y < textureSize.Height; y++) {
                for(int x = 0; x < textureSize.Width; x++) {
                    float value = OpenSimplex2S.Noise2_ImproveX(noiseSeed,x * pixelScale,y * pixelScale);

                    // Value is in range '-1.0' to '1.0', apply basic formula to align to '0.0' to '1.0.
                    value = (value + 1f) * 0.5f;

                    int index = y * textureSize.Width + x;
                    data[index] = value;
                }
            }

            return texture;
        }

        public async Task<Texture<float>> CreateNoiseTextureAsync(TextureSize textureSize,float scale,CancellationToken token,long? seed = null) {

            Texture<float> texture = new(textureSize,_arrayPool);
            Span<float> data = texture.Data;

            long noiseSeed = seed ?? _random.NextInt64();

            // Noise scales vertically with 'size.Height'
            double pixelScale = 1.0 / (textureSize.Height - 1) * scale;

            for(int y = 0;y < textureSize.Height;y++) {

                token.ThrowIfCancellationRequested();

                for(int x = 0;x < textureSize.Width;x++) {
                    float value = OpenSimplex2S.Noise2_ImproveX(noiseSeed,x * pixelScale,y * pixelScale);

                    // Value is in range '-1.0' to '1.0', apply basic formula to align to '0.0' to '1.0.
                    value = (value + 1f) * 0.5f;

                    int index = y * textureSize.Width + x;
                    data[index] = value;
                }
            }

            return texture;
        }
    }
}
