using System;
using Cristal;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;

namespace CristalLab.Pages {

    public sealed partial class NoiseGenerationTest:WorkspacePage {

        private const long FIXED_SEED = 0;

        private const int TEMP_BITMAP_SIZE = 64;
        private const int FULL_RES_BITMAP_SIZE = 512;

        private const int FULL_RES_DELAY = 100;

        private WriteableBitmap? _fullOutput = null;
        private CancellationTokenSource? _cancellationTokenSource = null;

        private WriteableBitmap? _tempOutput = null;

        public NoiseGenerationTest() {
            InitializeComponent();
        }

        private void WorkspacePage_Loaded(object sender,RoutedEventArgs e) {
            Regenerate();
        }


        private void PushTextureToBitmap(Texture<float> texture,WriteableBitmap bitmap) {
            using var pixelStream = bitmap.PixelBuffer.AsStream();
            pixelStream.Seek(0,SeekOrigin.Begin);

            //TODO: move allllllll this extra math to Cristal. It is running poorly over the UI thread. Boo!

            const bool USE_SRGB_TRANSFER = false;

            bool useIslandMode = IslandToggleSwitch.IsOn;
            float islandPivotPoint = (float)IslandPivotSlider.Value;
            float islandRolloffRange = (float)IslandRolloffSlider.Value;

            float islandCeiling = MathF.Min(islandPivotPoint + islandRolloffRange * 0.5f,1f);
            float islandFloor = MathF.Max(islandPivotPoint - islandRolloffRange * 0.5f,0f);

            float rolloffRangeRecip = 1.0f / (islandCeiling - islandFloor);

            for(int i = 0;i<texture.Data.Length;i++) {
                float input = texture.Data[i];

                if(useIslandMode) {
                    float t = Math.Clamp((input - islandFloor) * rolloffRangeRecip,0f,1f);
                    input = t * t * (3f - 2f * t); // Smooth Step
                }

                byte output = (byte)MathF.Floor((USE_SRGB_TRANSFER ? TransferSRGB(input) : input) * byte.MaxValue);
                pixelStream.WriteByte(output);
                pixelStream.WriteByte(output);
                pixelStream.WriteByte(output);
                pixelStream.WriteByte(byte.MaxValue);
            }
            bitmap.Invalidate();
        }


        private void Regenerate() {
            if(!IsLoaded) {
                return;
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new();

            var token = _cancellationTokenSource.Token;

            if(_tempOutput is null) {
                _tempOutput = new(TEMP_BITMAP_SIZE,TEMP_BITMAP_SIZE);
            }

            var fullOutputSize = new TextureSize(FULL_RES_BITMAP_SIZE); //TODO: measure BitmapContainer, get next power of 2 up (can use 'System.Numerics.BitOperations.RoundUpToPowerOf2')
            if(_fullOutput is null || _fullOutput.PixelWidth != fullOutputSize.Width || _fullOutput.PixelHeight != fullOutputSize.Height) {
                _fullOutput = new WriteableBitmap(fullOutputSize.Width,fullOutputSize.Height);
            }

            var textureScale = (float)ScaleSlider.Value;
            var dispatcher = DispatcherQueue;
            var cristalFactory = Session.CristalFactory;

            Task.Run(async () => {
                Texture<float>? texture = null;
                try {
                    await Task.Delay(FULL_RES_DELAY,token);
                    texture = await cristalFactory.CreateNoiseTextureAsync(fullOutputSize,textureScale,token,FIXED_SEED);
                    token.ThrowIfCancellationRequested();
                    dispatcher.TryEnqueue(() => {
                        if(token.IsCancellationRequested || texture is null) {
                            return;
                        }
                        PushTextureToBitmap(texture.Value,_fullOutput);
                        BitmapContainer.Source = _fullOutput;
                        texture?.Dispose();
                        texture = null;
                    });
                } catch(OperationCanceledException exception) {
                    Debug.WriteLine($"Background texture generation canceled: {exception.Message}");
                    texture?.Dispose();
                    texture = null;
                } catch(Exception exception) {
                    Debug.WriteLine($"Background texture generation failed unexpectedly: {exception.Message}");
                    texture?.Dispose();
                    texture = null;
                }
            },token);
            using var texture = cristalFactory.CreateNoiseTexture(new TextureSize(TEMP_BITMAP_SIZE),textureScale,FIXED_SEED);
            PushTextureToBitmap(texture,_tempOutput);
            BitmapContainer.Source = _tempOutput;
        }

        private void IslandRolloffSlider_ValueChanged(object sender,Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e) {
            Regenerate();
        }

        private void IslandPivotSlider_ValueChanged(object sender,Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e) {
            Regenerate();
        }

        private void IslandToggleSwitch_Toggled(object sender,RoutedEventArgs e) {
            Regenerate();
        }

        private void ScaleSlider_ValueChanged(object sender,Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e) {
            Regenerate();
        }

        private static float TransferSRGB(float value) {
            if(value <= 0.0031308f) {
                return value * 12.92f;
            } else {
                const float GAMMA = (float)(1.0 / 2.4);
                return 1.055f * MathF.Pow(value,GAMMA) - 0.055f;
            }
        }
    }
}
