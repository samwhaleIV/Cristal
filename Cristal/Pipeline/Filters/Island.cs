namespace Cristal.Pipeline.Filters {
    public readonly struct Island(float center,float range):IFilter<float,float> {

        private readonly record struct IslandRange(float Floor,float Ceiling) {
            public readonly float Reciprocal { get; init; } = 1.0f / (Ceiling - Floor);
        }

        private readonly IslandRange islandRange = new(
            Floor: MathF.Max(center - Math.Abs(range * 0.5f),0f),
            Ceiling: MathF.Min(center + Math.Abs(range * 0.5f),1f)
        );

        public float Process(float value) {
            value = (value - islandRange.Floor) * islandRange.Reciprocal;
            if(value <= 0f) {
                return 0f;
            } else if(value >= 1f) {
                return 1f;
            } else {
                return value * value * MathF.FusedMultiplyAdd(value,-2f,3);
            }
        }
    }
}
