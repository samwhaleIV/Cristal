using System.Buffers;
using System.Runtime.InteropServices;

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

        public readonly void WriteToStream(Stream stream) {
            stream.Seek(0,SeekOrigin.Begin);
            var data = Array ?? [];
            stream.Write(data,0,Math.Min(data.Length,Length));
        }

        public readonly Span<byte> CreateSpan() {
            if(Array is null) {
                return [];
            }
            return new Span<byte>(Array,0,Length);
        }

        public readonly ReadOnlySpan<byte> CreateReadOnlySpan() {
            if(Array is null) {
                return [];
            }
            return new ReadOnlySpan<byte>(Array,0,Length);
        }
    }
}
