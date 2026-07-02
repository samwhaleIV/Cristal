using Cristal.Pipeline;
using Cristal.Pipeline.Functions;
using System.Buffers;
using System.Runtime.InteropServices;

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

        public ByteBuffer MonochromeToRGBA(Texture<byte> texture) {
            ByteBuffer buffer = new(texture.Size.Area * 4,_arrayPool);

            Span<byte> rgba = buffer.AsSpan();
            ReadOnlySpan<byte> mc = texture.AsSpan();

            for(int i = 0;i<mc.Length;i++) {
                byte value = mc[i];
                rgba[(i*4) + 0] = value;
                rgba[(i*4) + 1] = value;
                rgba[(i*4) + 2] = value;
                rgba[(i*4) + 3] = byte.MaxValue;
            }
            return buffer;
        }

        private Texture<TTextureData> ProcessTexturePipeline<TTextureData,TParentOut,TParent,TFilter>(
            Node<Coordinate,TParentOut,TTextureData,TParent,TFilter> pipeline,
            TextureProcessorConfig config
        )
            where TParent : IFunction<Coordinate,TParentOut>
            where TFilter : IFunction<TParentOut,TTextureData>
            where TTextureData : struct
        {
            Texture<TTextureData> texture = new(config.Size,_arrayPool);

            int width = config.Size.Width, height = config.Size.Height;

            if(config.ParallelProcessingEnabled) {
                Memory<byte> textureData = texture.AsMemory();

                ParallelOptions options = new();
                if(config.CancellationToken.HasValue) {
                    options.CancellationToken = config.CancellationToken.Value;
                }

                Parallel.For(0,height,options,y => {
                    Span<TTextureData> typedData = MemoryMarshal.Cast<byte,TTextureData>(textureData.Span);
                    int i = y * width;
                    for(int x = 0;x < width;x++) {
                        TTextureData value = pipeline.Process(new Coordinate(x,y));
                        typedData[i + x] = value;
                    }
                });
            } else {
                Span<TTextureData> textureData = texture.AsSpan();
                int i = 0;
                if(config.CancellationToken.HasValue) {
                    var cancellationToken = config.CancellationToken.Value;
                    for(int y = 0;y < height;y++) {
                        cancellationToken.ThrowIfCancellationRequested();
                        for(int x = 0;x < width;x++) {
                            TTextureData value = pipeline.Process(new Coordinate(x,y));
                            textureData[i++] = value;
                        }
                    }
                } else {
                    for(int y = 0;y < height;y++) {
                        for(int x = 0;x < width;x++) {
                            TTextureData value = pipeline.Process(new Coordinate(x,y));
                            textureData[i++] = value;
                        }
                    }
                }
            }
            return texture;
        }

        public Texture<byte> CreateNoiseTexture(NoiseTextureConfig noiseConfig,TextureProcessorConfig processorConfig) {

            float baseOffset = noiseConfig.HalfPixelOffsetEnabled ? 0.5f : 0.0f;
            float xOffset = processorConfig.Size.Width * -noiseConfig.OriginX + baseOffset;
            float yOffset = processorConfig.Size.Height * -noiseConfig.OriginY + baseOffset;

            float scale = 1.0f / processorConfig.Size.Height * noiseConfig.Scale;
            OpenSimplexNoise noise = new(noiseConfig.Seed ?? _random.NextInt64(),scale,xOffset,yOffset);

            if(noiseConfig.IslandFilterEnabled) {
                IslandFilter island = new(noiseConfig.IslandCenter,noiseConfig.IslandRange);
                var pipeline = PipelineFactory.CreatePipeline<Coordinate,float,OpenSimplexNoise>(noise).Append<float,IslandFilter>(island).Append<byte,FloatToByte>();
                return ProcessTexturePipeline(pipeline,processorConfig);
            } else {
                var pipeline = PipelineFactory.CreatePipeline<Coordinate,float,OpenSimplexNoise>(noise).Append<byte,FloatToByte>();
                return ProcessTexturePipeline(pipeline,processorConfig);
            }
        }
    }
}
