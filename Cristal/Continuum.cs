using System.Buffers;
using Cristal.Pipeline.Filters;

namespace Cristal {
    /// <summary>
    /// TODO
    /// </summary>
    public sealed class Continuum {
        /// <summary>
        /// Source of pseudo-random number generation (PRNG) used in procedural generation routines.
        /// </summary>
        private readonly Random _random;

        private readonly ArrayPool<byte> _arrayPool = ArrayPool<byte>.Create();

        /// <summary>
        /// Create a <c>Continuum</c> with a <c>Random</c> object that may be shared with another system.
        /// Parameter <paramref name="externalRandom"/> will be preserved locally.
        /// </summary>
        /// <param name="externalRandom">External <c>Random</c> object. Reference will be retained by the new <c>Continuum</c>.</param>
        public Continuum(Random externalRandom) {
            _random = externalRandom;
        }

        /// <summary>
        /// Create a <c>Continuum</c> with a unique <c>Random</c> object.
        /// Starting seed is determined by the default <c>System.Random</c> implementation.
        /// See <see cref="Random()"/> for more information.
        /// </summary>
        public Continuum() {
            _random = new Random();
        }

        /// <summary>
        /// Create a <c>Continuum</c> with a provided starting seed.
        /// See <see cref="Random(int)"/> for more information on pseudo-random number generation and seeding.
        /// </summary>
        /// <param name="seed">
        /// Initial seed provided to a pseudo-random number generator (PRNG). Using the same seed across object instances or program lifetimes allows for deterministic behaviors.
        /// </param>
        public Continuum(int seed) {
            _random = new Random(seed);
        }

        public Texture<byte> CreateNoiseTexture(NoiseTextureConfig config,CancellationToken? token = null) {

            var scale = 1.0f / config.Size.Height * config.Scale;

            var pipeline = PipelineFactory.CreatePipeline<Point,float,Noise>(new Noise(config.Seed ?? _random.NextInt64(),scale))
                .AppendOptional(new IslandFilter(config.IslandFilterConfig.GetValueOrDefault()),config.IslandFilterConfig.HasValue)
                .Append<float,ToSRGB>()
                .Append<byte,FloatToByte>();

            TextureSize size = config.Size;
            Texture<byte> texture = new(size,_arrayPool);
            Span<byte> data = texture.Data;

            for(int y = 0;y < size.Height;y++) {

                for(int x = 0;x < size.Width;x++) {
                    byte value = pipeline.Process(new Point(x,y));
                    int index = y * size.Width + x;
                    data[index] = value;

                    token?.ThrowIfCancellationRequested();
                }
            }

            return texture;
        }
    }

    public readonly record struct NoiseTextureConfig(
        TextureSize Size,
        float Scale = 1.0f,
        long? Seed = null,
        IslandFilterConfig? IslandFilterConfig = null
    );
}
