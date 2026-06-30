namespace Cristal.Pipeline.Filters {
    public readonly record struct IslandFilterConfig(float Center,float Range);

    public readonly struct IslandFilter(IslandFilterConfig config):IFilter<float,float> {

        private readonly record struct IslandRange(float Floor,float Ceiling) {
            public readonly float Reciprocal { get; init; } = 1.0f / (Ceiling - Floor);
        }

        private readonly IslandRange islandRange = new(
            Floor: MathF.Max(config.Center - Math.Abs(config.Range * 0.5f),0f),
            Ceiling: MathF.Min(config.Center + Math.Abs(config.Range * 0.5f),1f)
        );

        public float Process(float value) {
            // use floor and ceil instead of 0f and 1f. eliminate subtraction operation this way
            value = Math.Clamp((value - islandRange.Floor) * islandRange.Reciprocal,0f,1f);
            value = value * value * (3f - 2f * value);
            return value;
        }
    }
}
