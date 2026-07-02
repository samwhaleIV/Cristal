using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cristal {

    public struct Texture<T>:IDisposable where T:struct {

        public readonly TextureSize Size { get; }
        private readonly int ArraySliceLength { get; }

        private ArrayPool<byte>? ArrayPool { get; set; }
        private byte[]? Array { get; set; }

        internal Texture(TextureSize size,ArrayPool<byte> arrayPool) {
            Size = size;
            ArraySliceLength = size.Area * Unsafe.SizeOf<T>();
            ArrayPool = arrayPool;
            Array = ArrayPool.Rent(ArraySliceLength);
        }

        internal readonly Span<T> AsSpan() {
            if(Array is null) {
                return [];
            }
            return MemoryMarshal.Cast<byte,T>(new Span<byte>(Array,0,ArraySliceLength));
        }

        internal readonly Memory<byte> AsMemory() {
            if(Array is null) {
                return Memory<byte>.Empty;
            }
            return Array.AsMemory(0,ArraySliceLength);
        }

        public void Dispose() {
            if(ArrayPool is not null && Array is not null) {
                ArrayPool.Return(Array);
            }
            Array = null;
            ArrayPool = null;
        }
    }
}
