using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cristal {

    public ref struct Texture<T>:IDisposable where T:struct {

        public readonly TextureSize Size { get; }
        private readonly int ArrayLength { get; }
        private ArrayPool<byte>? ArrayPool { get; set; }
        private byte[]? Array { get; set; }

        internal Texture(TextureSize size,ArrayPool<byte> arrayPool) {
            Size = size;
            ArrayLength = size.Area * Unsafe.SizeOf<T>();
            ArrayPool = arrayPool;
            Array = ArrayPool.Rent(ArrayLength);
        }

        public readonly Span<T> Data {
            get {
                if(Array is null) {
                    return [];
                }
                return MemoryMarshal.Cast<byte,T>(new Span<byte>(Array,0,ArrayLength));
            }
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
