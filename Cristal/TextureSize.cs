namespace Cristal {
    public readonly struct TextureSize {

        public readonly int Width => int.Max(field,1);
        public readonly int Height => int.Max(field,1);
        public int Area => Width * Height;

        public TextureSize(int size) {
            Width = size;
            Height = size;
        }

        public TextureSize(int width,int height) {
            Width = width;
            Height = height;
        }

        public static TextureSize Square2 => new(2);
        public static TextureSize Square4 => new(4);
        public static TextureSize Square8 => new(8);
        public static TextureSize Square16 => new(16);
        public static TextureSize Square32 => new(32);
        public static TextureSize Square64 => new(64);
        public static TextureSize Square128 => new(128);
        public static TextureSize Square256 => new(256);
        public static TextureSize Square512 => new(512);
        public static TextureSize Square1024 => new(1024);
        public static TextureSize Square2048 => new(2048);
        public static TextureSize Square4096 => new(4096);
        public static TextureSize Square8192 => new(8192);
    }
}
