using Cristal;
using Cristal.Pipeline.Filters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using static Cristal.Pipeline.Filters.IslandFilter;

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
            if(_fullOutput is null) {
                Regenerate();
            }
        }

        private static void PushTextureToBitmap(Texture<byte> texture,WriteableBitmap bitmap) {
            using(var pixelStream = bitmap.PixelBuffer.AsStream()) {
                pixelStream.Seek(0,SeekOrigin.Begin);
                for(int i = 0;i<texture.Data.Length;i++) {
                    byte value = texture.Data[i];
                    pixelStream.WriteByte(value);
                    pixelStream.WriteByte(value);
                    pixelStream.WriteByte(value);
                    pixelStream.WriteByte(byte.MaxValue);
                }
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

            _tempOutput ??= new(TEMP_BITMAP_SIZE,TEMP_BITMAP_SIZE);

            var fullOutputSize = new TextureSize(FULL_RES_BITMAP_SIZE); //TODO: measure BitmapContainer, get next power of 2 up (can use 'System.Numerics.BitOperations.RoundUpToPowerOf2')
            if(_fullOutput is null || _fullOutput.PixelWidth != fullOutputSize.Width || _fullOutput.PixelHeight != fullOutputSize.Height) {
                _fullOutput = new WriteableBitmap(fullOutputSize.Width,fullOutputSize.Height);
            }

            var dispatcher = DispatcherQueue;
            var cristalFactory = Session.CristalFactory;

            var noiseScale = (float)ScaleSlider.Value;

            IslandFilterConfig? islandConfig = IslandToggleSwitch.IsOn ? new(
                (float)IslandPivotSlider.Value,
                (float)IslandRolloffSlider.Value
            ) : null;

            Task.Run(async () => {
                Texture<byte>? texture = null;
                try {
                    await Task.Delay(FULL_RES_DELAY,token);
                    var config = new NoiseTextureConfig(new TextureSize(FULL_RES_BITMAP_SIZE),noiseScale,FIXED_SEED,islandConfig);
                    texture = cristalFactory.CreateNoiseTexture(config,token);
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

            var config = new NoiseTextureConfig(new TextureSize(TEMP_BITMAP_SIZE),noiseScale,FIXED_SEED,islandConfig);
            using var texture = cristalFactory.CreateNoiseTexture(config);
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
    }
}
