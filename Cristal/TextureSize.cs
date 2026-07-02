namespace Cristal {
    /// <summary>
    /// Used as a description or request for a texture of a specific size.
    /// The default value state guarantees a minimum size of <c>1x1</c>.
    /// </summary>
    public readonly record struct TextureSize {

        /// <summary>
        /// Horizontal component (X) with a fixed lower bound of <c>1</c>.
        /// Not recommended for hot paths.
        /// </summary>
        public int Width => Math.Max(1,field);

        /// <summary>
        /// Vertical component (Y) with a fixed lower bound of <c>1</c>.
        /// Not recommended for hot paths.
        /// </summary>
        public int Height => Math.Max(1,field);

        /// <summary>
        /// Total area provided as width times height with a fixed lower bound of <c>1</c>.
        /// Not recommended for hot paths.
        /// </summary>
        public int Area => Width * Height;

        /// <summary>
        /// Creates and validates a square size description with the provided dimensional length value, <paramref name="size"/>.
        /// </summary>
        /// <param name="size">Horizontal (X) and vertical component (Y). I.e., the square root.</param>
        /// <exception cref="ArgumentOutOfRangeException">Dimensions must be greater or equal to 1.</exception>
        public TextureSize(int size) {
            ArgumentOutOfRangeException.ThrowIfLessThan(size,1,nameof(size));
            Width = size;
            Height = size;
        }

        /// <summary>
        /// Creates and validates a size description for any aspect ratio using the provided dimensional length values.
        /// </summary>
        /// <param name="width">Horizontal component (X). Must be greater or equal to <c>1</c>.</param>
        /// <param name="height">Vertical component (Y). Must be greater or equal to <c>1</c>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Dimensions must be greater or equal to 1.</exception>
        public TextureSize(int width,int height) {
            ArgumentOutOfRangeException.ThrowIfLessThan(width,1,nameof(width));
            ArgumentOutOfRangeException.ThrowIfLessThan(height,1,nameof(height));
            Width = width;
            Height = height;
        }
    }
}
