namespace Cristal {
    public readonly record struct Coordinate(int X,int Y);

    public readonly record struct TextureProcessorConfig(
        TextureSize Size,
        bool ParallelProcessingEnabled = false,
        CancellationToken? CancellationToken = null
    );

    public readonly record struct NoiseTextureConfig(
        float Scale = 1.0f,
        float OriginX = 0.5f,
        float OriginY = 0.5f,
        bool HalfPixelOffsetEnabled = true,
        bool IslandFilterEnabled = false,
        float IslandCenter = 0.5f,
        float IslandRange = 0.05f,
        int? Seed = null
    );
}
