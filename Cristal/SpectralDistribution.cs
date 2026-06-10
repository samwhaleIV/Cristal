namespace Cristal {

    /// <summary>
    /// <para>Data point in a spectral power distribution table (SPD).
    /// Conventionally, cartesian SPD plots put wavelength along the X axis (measured in a physical distance, such as nanometers) and reflectance along the Y axis (measured in arbitrary units).</para>
    /// <para>For our purposes, the X axis is encoded as nanometers and the Y axis is encoded as a percentage. See <see cref="Wavelength"/> and <see cref="Reflectance"/> for more information.</para>
    /// <para>This data type is primarily intended for offline ingestion and translation of third party SPDs.</para>
    /// </summary>
    public readonly record struct SpectralPoint {
        /// <summary>
        /// Electromagnetic wavelength of this spectral point, measured in nanometers. Visible light is typically defined as ~380nm (deep violet) to ~750nm (deep red).
        /// </summary>
        public float Wavelength { get; init; }
        /// <summary>
        /// <para>Linear power response of this spectral point, measuring what percentage of photons that hit <see cref="Wavelength"/> are reflected.</para>
        /// <para>Percentage is defined as 0.0 to 1.0. (0.0 [0%] = total absorption, 1.0 [100%] = total reflection).</para>
        /// <para>Note: Reflectance is NOT expected to be at 100%. This is uncommon for most organic, real-world spectroscopy samples.
        /// A data set with this characteristic may indicate the data underwent normalization. In later processing, this will lead to materials that are overly bright.</para>
        /// </summary>
        public float Reflectance { get; init; }
    }

    [System.Runtime.CompilerServices.InlineArray(10)]
    public struct Byte255InlineArray {
        private byte _value;
    }

    public readonly record struct SpectralDistribution(float Scale,Byte255InlineArray Data) {
        /// <summary>
        /// <para>Creates a spectral distribution table from a set of wavelength and reflectance points.
        /// Through quantization, we form the optimized (yet lossy) structure needed for improved real-time performance.</para>
        /// <para>Typically, a set of unbound, spectral points is used "offline" for ingestion of external tables.
        /// (Offline meaning outside of your high performance runtime, such as a graphics heavy or packaged app.)
        /// If you are creating your own spectral distributions "online", you should do so using other methods and ignore this method (along with the <see cref="SpectralPoint"/> type).</para>
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static SpectralDistribution FromPoints(ReadOnlySpan<SpectralPoint> points) {

        }
    }
}
