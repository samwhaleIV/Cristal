namespace Cristal.Spectral {

    /// <summary>
    /// Typically, a spectral power distribution. This is a suitable type for representing both a material's reflectance or a light source's radiant flux.
    /// </summary>
    public readonly record struct Distribution {

        /// <summary>
        /// High and low frequency cutoffs, i.e., the margins of this spectral power distribution. See <see cref="Bandwidth"/> for more information.
        /// </summary>
        public readonly Bandwidth Range { get; init; }

        /// <summary>
        /// Since <see cref="Bands"/> are normalized, <see cref="Scale"/> must be applied to each sample.
        /// In other words, this is the Y axis' scale in a spectral power distribution.
        /// We take values that have been normalized and use <see cref="Scale"/> to get close to their original values.
        /// </summary>
        /// <remarks>
        /// Creating a <see cref="Distribution"/> generally necessitates compression.
        /// However, the existence of a scale factor allows <i>all</i> the bits of a band to be fully utilized, by normalizing the largest input value to <c>0b11111111</c>.
        /// </remarks>
        public readonly float Scale { get; init; }

        /// <summary>
        /// Normalized samples, defined by an externally defined bandwidth (e.g. <c>max - min</c>).
        /// Each sample covers frequency in this pattern: <c>(i * (bandwidth / <see cref="Constants.SPD_BAND_COUNT"/>))</c>.
        /// </summary>
        public readonly InlineArrayBands Bands { get; init; }
    }
}
