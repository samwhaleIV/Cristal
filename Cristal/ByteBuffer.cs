using System.Buffers;

namespace Cristal {

    public struct ByteBuffer:IDisposable {

        public readonly int Length { get; }
        private ArrayPool<byte>? ArrayPool { get; set; }

        private byte[]? Array { get; set; }

        internal ByteBuffer(int length,ArrayPool<byte> arrayPool) {
            Length = length;
            ArrayPool = arrayPool;
            Array = ArrayPool.Rent(Length);
        }

        public void Dispose() {
            if(ArrayPool is not null && Array is not null) {
                ArrayPool.Return(Array);
            }
            Array = null;
            ArrayPool = null;
        }

        internal readonly Span<byte> AsSpan() {
            if(Array is null) {
                return [];
            }
            return new Span<byte>(Array,0,Length);
        }

        public readonly void WriteTo(Stream stream) {

            // When desired, seeking to the stream's start is the caller's responsibility.

            if(Array is null) {
                return;
            }

            // Limit of the stream (also limited to 32-bits)
            int stride = (int)(stream.Length - stream.Position);

            // FULL length of the array provided by the pool
            if(Array.Length < stride) { 
                stride = Array.Length;
            }

            // SHORT length of the array (our slice within the array - remember, the pool provides an array that may be larger than our target)
            if(Length < stride) { 
                stride = Length;
            }

            stream.Write(Array,0,stride);
        }
    }
}
