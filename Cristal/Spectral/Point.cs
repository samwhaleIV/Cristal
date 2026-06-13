namespace Cristal.Spectral {
    /// <summary>
    /// <para>
    /// Data point in a spectral power distribution table (SPD).
    /// Conventionally, cartesian SPD plots put wavelength along the X axis (measured in a physical distance, such as nanometers) and reflectance along the Y axis (measured in arbitrary units).
    /// </para>
    /// <para>
    /// For our purposes, the X axis is encoded as nanometers and the Y axis is encoded as a percentage. See <see cref="Wavelength"/> and <see cref="Reflectance"/> for more information.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <see cref="Point"/> is primarily intended for offline ingestion and translation of third party SPDs. See <see cref="Importer"/> for more information.
    /// </remarks>
    public struct Point {

        /// <summary>
        /// Electromagnetic frequency of this spectral point, measured in nanometers.
        /// </summary>
        /// <remarks>
        /// For reference, visible light is typically defined as ~380nm (deep violet) to ~750nm (deep red).
        /// </remarks>
        public float Wavelength;

        /// <summary>
        /// <para>
        /// Linear power response of this spectral point, measuring what percentage of photons that hit <see cref="Wavelength"/> are reflected.
        /// </para>
        /// <para>
        /// Percentage is defined as 0.0 to 1.0. (0.0 [0%] = total absorption, 1.0 [100%] = total reflection).
        /// </para>
        /// </summary>
        /// <remarks>
        /// Note: For materials, reflectance is <i>not</i> expected to be at 100%. This is uncommon for most organic, real-world spectroscopy samples.
        /// A data set with this characteristic may indicate the values underwent normalization. In later processing, this will lead to materials that are over-reflective.
        /// </remarks>
        public float Reflectance;
    }
}
