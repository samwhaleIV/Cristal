using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace CristalLab.Pages {

    public sealed partial class NoiseGenerationTest:WorkspacePage {

        private WriteableBitmap? _output = null;

        public NoiseGenerationTest() {
            InitializeComponent();
        }

        private void Button_Click(object sender,RoutedEventArgs e) {
            int resolution = (int)Math.Pow(2,Math.Round(ResolutionSlider.Value));
            if(_output is null || (_output.PixelWidth != resolution || _output.PixelHeight != resolution)) {
                _output = new(resolution,resolution);
                BitmapContainer.Source = _output;
            }

            using var noiseTexture = Session.CristalFactory.CreateNoiseTexture(new Cristal.TextureSize(resolution),(float)ScaleSlider.Value);

            using var pixelStream = _output.PixelBuffer.AsStream();
            pixelStream.Seek(0,System.IO.SeekOrigin.Begin);

            for(int i = 0;i<noiseTexture.Data.Length;i++) {
                byte value = (byte)MathF.Floor(noiseTexture.Data[i] * byte.MaxValue);
                pixelStream.WriteByte(value);
                pixelStream.WriteByte(value);
                pixelStream.WriteByte(value);
                pixelStream.WriteByte(byte.MaxValue);
            }

            _output.Invalidate();
        }
    }
}
