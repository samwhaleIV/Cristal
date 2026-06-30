namespace Cristal.Pipeline.Filters {
    public readonly struct FloatToByte:IFilter<float,byte> {
        public byte Process(float value) {
            return (byte)MathF.Floor(value * byte.MaxValue);
        }
    }
}
