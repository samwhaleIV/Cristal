using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cristal {

    public struct Texture<T>:IDisposable where T:struct {

        public readonly TextureSize Size { get; }
        private readonly int Length { get; }

        private ArrayPool<byte>? ArrayPool { get; set; }
        private byte[]? Array { get; set; }

        internal Texture(TextureSize size,ArrayPool<byte> arrayPool) {
            Size = size;
            Length = size.Area * Unsafe.SizeOf<T>();
            ArrayPool = arrayPool;
            Array = ArrayPool.Rent(Length);
        }

        public readonly Span<T> CreateSpan() {
            if(Array is null) {
                return [];
            }
            return MemoryMarshal.Cast<byte,T>(new Span<byte>(Array,0,Length));
        }

        public readonly ReadOnlySpan<T> CreateReadOnlySpan() {
            if(Array is null) {
                return [];
            }
            return MemoryMarshal.Cast<byte,T>(new ReadOnlySpan<byte>(Array,0,Length));
        }

        internal readonly byte[] GetInternalArray() {
            return Array ?? [];
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
