using System.Runtime.CompilerServices;

namespace Cristal.Spectral {
    /// <summary>
    /// <para>
    /// A byte array with an item capacity of <see cref="BAND_COUNT"/>.
    /// </para>
    /// <para>This is used for the data block in <see cref="Distribution"/>.
    /// An inline array allows structs to contain arrays locally, rather than pointing to one in the heap.
    /// </para>
    /// </summary>
    [InlineArray(BAND_COUNT)]
    public struct InlineArrayBands {
        /// <summary>
        /// The quantity of bands in a spectral power distribution. Simulation accuracy increases with band count but performance may suffer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Value is chosen to balance cache line efficiency with simulation resolution.
        /// With shorter bands, procedural generation will have more latitude (when working with color) and tristimulus approximations will be more accurate.
        /// </para>
        /// <para>
        /// However (it bears repeating), more bands equals worse performance.
        /// </para>
        /// </remarks>
        public const int BAND_COUNT = 256;

        /// <summary>
        /// This is a "magic" element that the inline array attribute binds to.
        /// </summary>
        private byte _element;
    }
}
