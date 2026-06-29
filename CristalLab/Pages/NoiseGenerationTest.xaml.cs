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

        private static float TransferSRGB(float value) {
            if(value <= 0.0031308f) {
                return value * 12.92f;
            } else {
                const float GAMMA = (float)(1.0 / 2.4);
                return 1.055f * MathF.Pow(value,GAMMA) - 0.055f;
            }
        }

        private void Button_Click(object sender,RoutedEventArgs e) {
            int resolution = (int)Math.Pow(2,Math.Round(ResolutionSlider.Value));
            if(_output is null || (_output.PixelWidth != resolution || _output.PixelHeight != resolution)) {
                _output = new(resolution,resolution);
                BitmapContainer.Source = _output;
            }

            using var noiseTexture = Session.CristalFactory.CreateNoiseTexture(new Cristal.TextureSize(resolution),(float)ScaleSlider.Value,0);

            using var pixelStream = _output.PixelBuffer.AsStream();
            pixelStream.Seek(0,System.IO.SeekOrigin.Begin);

            bool srgbTransfer = true;

            bool useIslandMode = IslandToggleSwitch.IsOn;
            float islandPivotPoint = (float)IslandPivotSlider.Value;
            float islandRolloffRange = (float)IslandRolloffSlider.Value;

            float islandCeiling = MathF.Min(islandPivotPoint + islandRolloffRange * 0.5f,1f);
            float islandFloor = MathF.Max(islandPivotPoint - islandRolloffRange * 0.5f,0f);

            float rolloffRangeRecip = 1.0f / (islandCeiling - islandFloor);

            for(int i = 0;i<noiseTexture.Data.Length;i++) {
                float noiseValue = noiseTexture.Data[i];
                if(useIslandMode) {
                    float t = Math.Clamp((noiseValue - islandFloor) * rolloffRangeRecip,0f,1f);
                    noiseValue = t * t * (3f - 2f * t); // Smooth Step
                }
                byte value = (byte)MathF.Floor((srgbTransfer ? TransferSRGB(noiseValue) : noiseValue) * byte.MaxValue);
                pixelStream.WriteByte(value);
                pixelStream.WriteByte(value);
                pixelStream.WriteByte(value);
                pixelStream.WriteByte(byte.MaxValue);
            }

            _output.Invalidate();
        }
    }
}
