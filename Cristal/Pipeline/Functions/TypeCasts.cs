namespace Cristal.Pipeline.Functions {
    public readonly struct FloatToByte:IFunction<float,byte> {
        public byte Process(float value) {
            return (byte)(value * 255.0f + 0.5f);
        }
    }
}
