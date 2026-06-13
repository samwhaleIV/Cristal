namespace Cristal.Spectral {

    /// <summary>
    /// <para>
    /// Defines a range for frequencies, i.e., a high and low cutoff point. Used primarily as metadata for spectral power distributions.
    /// </para>
    /// <para>
    /// All instances are guaranteed to meet two conditions: <c>Low &lt; High &amp;&amp; Range &gt;= <see cref="MIN_RANGE"/></c>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// (In order to make the struct <c>default</c> meet these conditions, shenanigans have been employed. The goal is to put the validation upfront rather than in handlers/methods.)
    /// </remarks>
    public readonly struct Bandwidth {

        /// <summary>
        /// <para>
        /// The default 'low' value for a spectral power distribution, i.e., the infrared cutoff frequency.
        /// </para>
        /// <para>
        /// Should be less than <see cref="CUTOFF_UV"/> and <i>at least</i> <see cref="Bandwidth.MIN_RANGE"/> distance apart.
        /// </para>
        /// </summary>
        /// <remarks>
        /// IR: Short for "infrared."
        /// </remarks>
        public const float CUTOFF_IR = 380;

        /// <summary>
        /// <para>
        /// The default 'high' value for a spectral power distribution, i.e., the ultraviolet cutoff frequency.
        /// </para>
        /// <para>
        /// Should be greater than <see cref="CUTOFF_IR"/> and <i>at least</i> <see cref="Bandwidth.MIN_RANGE"/> distance apart.
        /// </para>
        /// </summary>
        /// <remarks>
        /// UV: Short for "ultraviolet."
        /// </remarks>
        public const float CUTOFF_UV = 740;

        /// <summary>
        /// <para>
        /// The minimum bandwidth that a <see cref="Bandwidth"/> must span. That is, a bandwidth must meet this criteria: <c>HIGH - LOW &gt;= <see cref="MIN_SPECTRAL_BANDWIDTH_RANGE"/></c>.</para>
        /// </summary>
        /// <remarks>
        /// Hypothetically, this prevents extreme aliasing for ranges that are abnormally short.
        /// </remarks>
        public const float MIN_RANGE = 1.0f;

        /// <summary>
        /// The low frequency start/cutoff of this bandwidth description. Combined with <see cref="_range"/> to calculate the high frequency cutoff.
        /// </summary>
        private readonly float _low;

        /// <summary>
        /// <para>
        /// The range/distance of this bandwidth. I.e., the absolute distance between the high and low cutoff frequencies.
        /// </para>
        /// <para>
        /// (Note: a <c>_high</c> field does not exist because length encoding is used to ensure struct <c>default</c> correctness guarantees.)
        /// </para>
        /// </summary>
        private readonly float _range;

        /// <summary>
        /// The lower bound (aka, the start) of this <see cref="Bandwidth"/>. For example, when working with visible light, this is usually infrared (the low frequency cutoff).
        /// </summary>
        public float Low => _low;

        /// <summary>
        /// The upper bound (aka, the end) of this <see cref="Bandwidth"/>. For example, when working visible light, this is usually ultraviolet (the high frequency cutoff).
        /// </summary>
        public float High => _low + MIN_RANGE + _range;

        /// <summary>
        /// The absolute distance between the high and low cutoff frequencies. I.e, <c>High - Low</c>. This can never be less than <see cref="MIN_RANGE"/>.
        /// </summary>
        public float Range => MIN_RANGE + _range;

        /// <summary>
        /// Creates a <see cref="Bandwidth"/> with the desired <paramref name="high"/> and <paramref name="low"/> bounds.
        /// If the desired bounds cannot satisfy the type constraints (described in <see cref="Bandwidth"/>), <paramref name="valid"/> will be set to <c>false</c>.
        /// Regardless of the validation feedback, all <see cref="Bandwidth"/> values meet the aforementioned conditions.
        /// </summary>
        /// <param name="low">
        /// Lower bound of a bandwidth (i.e., range start).
        /// </param>
        /// <param name="high">
        /// Upper bound of a bandwidth (i.e., range end).
        /// </param>
        /// <param name="valid">
        /// When <c>true</c>: <paramref name="high"/> and <paramref name="low"/> values values were honored.
        /// When <c>false</c>: <paramref name="high"/> and <paramref name="low"/> values failed validation (i.e, the output range was extended or <paramref name="high"/> was dropped altogether).
        /// </param>
        public Bandwidth(float low,float high,out bool valid) {
            _low = low;
            _range = 0.0f;
            valid = false;
            if(low > high) {
                return;
            }
            float size = high - low - MIN_RANGE;
            if(size < 0.0f) {
                return;
            }
            _range = size;
            valid = true;
        }

        /// <summary>
        /// A minorly subjective definition of the visible light spectrum, optimized to improve per-band resolution.
        /// </summary>
        /// <remarks>
        /// (If modifying the built-in constants, range validation should be performed by the developer. Standard validation feedback will be discarded.
        /// All variants of bandwidth are guaranteed to be valid, but bad constants could lead to unexpected ranges.
        /// This will not happen with realistic values. Nonetheless, ensure <see cref="MIN_RANGE"/> is reached and that <c>low &lt; high</c>).
        /// </remarks>
        public static Bandwidth VisibleLight => new(CUTOFF_IR,CUTOFF_UV,out _);

    }
}
