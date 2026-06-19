using System.Buffers;

namespace Cristal.Spectral {
    public sealed class Importer {

        //private readonly ArrayPool<byte> _arrayPool;

        //public Importer() {
        //    _arrayPool = new ArrayPool<byte>();
        //}

        public Distribution CreateSpectralDistribution(Bandwidth bandwidth,ReadOnlySpan<Point> points) {
            throw new NotImplementedException();
        }
    }
}
