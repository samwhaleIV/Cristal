namespace Cristal.Pipeline.Filters {
    public readonly struct FloatToByte:IFilter<float,byte> {
        public byte Process(float value) {
            return (byte)(value * 255.0f + 0.5f);
        }
    }
}
