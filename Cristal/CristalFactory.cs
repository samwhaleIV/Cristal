using System;

namespace MonoCristal {
    public sealed class CristalFactory {

        /// <summary>
        /// Source of pseudo-random number generation (PRNG) used in procedural generation routines.
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// Create a <c>CristalFactory</c> with a <c>Random</c> object that may be shared with another system.
        /// Parameter <paramref name="externalRandom"/> will be preserved locally.
        /// </summary>
        /// <param name="externalRandom">External <c>Random</c> object. Reference will be retained by the new <c>CristalFactory</c>.</param>
        public CristalFactory(Random externalRandom) {
            _random = externalRandom;
        }

        /// <summary>
        /// Create a <c>CristalFactory</c> with a unique <c>Random</c> object.
        /// Starting seed is determined by the default <c>System.Random</c> implementation.
        /// See <see cref="Random()"/> for more information.
        /// </summary>
        public CristalFactory() {
            _random = new Random();
        }

        /// <summary>
        /// Create a <c>CristalFactory</c> with a provided starting seed.
        /// See <see cref="Random(int)"/> for more information on pseudo-random number generation and seeding.
        /// </summary>
        /// <param name="seed">Initial seed provided to a pseudo-random number generator (PRNG). Using the same seed across object instances or program lifetimes allows for deterministic behaviors.</param>
        public CristalFactory(int seed) {
            _random = new Random(seed);
        }



    }
}
