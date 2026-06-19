using System.Buffers;

namespace Cristal {
    public ref struct Texture<T>(TextureSize size):IDisposable where T : struct {
        private T[]? _array = ArrayPool<T>.Shared.Rent(size.Area);

        public readonly TextureSize Size => size;

        public readonly Span<T> Data {
            get {
                if(_array is null) {
                    return [];
                } else {
                    return new(_array,0,size.Area);
                }
            }
        }

        public void Dispose() {
            if(_array is null) {
                return;
            }
            ArrayPool<T>.Shared.Return(_array);
            _array = null;
        }
    }
}
