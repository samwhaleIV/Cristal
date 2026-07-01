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

namespace CristalLab.Pages {

    public sealed partial class NoiseGenerationTest:WorkspacePage {

        private const long FIXED_SEED = 0;
        private const int TEMP_BITMAP_SIZE = 64;
        private const int FULL_RES_DELAY = 50;

        private WriteableBitmap? _fullOutput = null;
        private CancellationTokenSource? _cancellationTokenSource = null;

        private WriteableBitmap? _tempOutput = null;

        public int CurrentSize { get; private set; } = 0;

        public NoiseGenerationTest() {
            InitializeComponent();
        }

        private void WorkspacePage_Loaded(object sender,RoutedEventArgs e) {
            if(_fullOutput is null) {
                Regenerate(parametersChanged: true);
            }
        }

        private static void PushTextureToBitmap(ByteBuffer buffer,WriteableBitmap bitmap) {
            using(var stream = bitmap.PixelBuffer.AsStream()) {
                buffer.WriteToStream(stream);
            }
            bitmap.Invalidate();
        }

        private TextureSize CalculateBitmapSize() {
            var actualSize = BitmapContainer.ActualSize;
            int smallerDimension = Math.Min((int)actualSize.X,(int)actualSize.Y);
            int newSize = Math.Max(smallerDimension,TEMP_BITMAP_SIZE * 2);
            return new TextureSize(newSize);
        }

        private void Regenerate(bool parametersChanged = true) {
            if(!IsLoaded) {
                return;
            }

            var fullSize = CalculateBitmapSize();

            if(_fullOutput is null) {
                _fullOutput = new WriteableBitmap(fullSize.Width,fullSize.Height);
            } else {
                bool sizeChanged = _fullOutput.PixelWidth != fullSize.Width || _fullOutput.PixelHeight != fullSize.Height;
                if(sizeChanged) {
                    bool smallerOrEqual = fullSize.Width <= _fullOutput.PixelWidth || fullSize.Height <= _fullOutput.PixelHeight;
                    if(!parametersChanged && smallerOrEqual) {
                        return;
                    }
                    _fullOutput = new WriteableBitmap(fullSize.Width,fullSize.Height);
                } else if(!parametersChanged) {
                    return;
                }
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new();
            var token = _cancellationTokenSource.Token;

            _tempOutput ??= new(TEMP_BITMAP_SIZE,TEMP_BITMAP_SIZE);

            var dispatcher = DispatcherQueue;
            var cristal = Session.Cristal;

            var noiseScale = (float)ScaleSlider.Value;

            NoiseTextureConfig config = new(
                Scale: (float)ScaleSlider.Value,
                IslandFilterEnabled: IslandToggleSwitch.IsOn,
                IslandCenter: (float)IslandPivotSlider.Value,
                IslandRange: (float)IslandRolloffSlider.Value,
                Seed: FIXED_SEED
            );

            ResolutionText.Text = $"Rendering";
            RenderProgressBar.IsIndeterminate = true;

            Task.Run(async () => {
                Texture<byte>? texture = null;
                ByteBuffer? rgbaData = null;

                void cleanup() {
                    texture?.Dispose();
                    texture = null;
                    rgbaData?.Dispose();
                    rgbaData = null;
                }

                try {
                    await Task.Delay(FULL_RES_DELAY,token);

                    texture = cristal.CreateNoiseTextureParallel(fullSize,config,token);
                    rgbaData = cristal.MonochromeToRGBA(texture.Value);
                    texture.Value.Dispose();
                    texture = null;

                    token.ThrowIfCancellationRequested();

                    dispatcher.TryEnqueue(() => {
                        if(token.IsCancellationRequested || rgbaData is null) {
                            cleanup();
                            return;
                        }

                        PushTextureToBitmap(rgbaData.Value,_fullOutput);

                        BitmapContainer.Source = _fullOutput;
                        ResolutionText.Text = $"{_fullOutput.PixelWidth}x{_fullOutput.PixelHeight}";
                        RenderProgressBar.IsIndeterminate = false;

                        cleanup();
                    });
                } catch(OperationCanceledException exception) {
                    Debug.WriteLine($"Background texture generation canceled: {exception.Message}");
                    cleanup();
                } catch(Exception exception) {
                    Debug.WriteLine($"Background texture generation failed unexpectedly: {exception.Message}");
                    cleanup();
                }
            },token);

            if(parametersChanged) {
                TextureSize shortSize = new(TEMP_BITMAP_SIZE);
                var tmpTexture = cristal.CreateNoiseTexture(shortSize,config);
                var rgbaData = cristal.MonochromeToRGBA(tmpTexture);
                tmpTexture.Dispose();
                PushTextureToBitmap(rgbaData,_tempOutput);
                rgbaData.Dispose();
                BitmapContainer.Source = _tempOutput;
            }
        }

        private void IslandRolloffSlider_ValueChanged(object sender,Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e) {
            Regenerate(parametersChanged: true);
        }

        private void IslandPivotSlider_ValueChanged(object sender,Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e) {
            Regenerate(parametersChanged: true);
        }

        private void IslandToggleSwitch_Toggled(object sender,RoutedEventArgs e) {
            Regenerate(parametersChanged: true);
        }

        private void ScaleSlider_ValueChanged(object sender,Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e) {
            Regenerate(parametersChanged: true);
        }

        private void BitmapContainer_SizeChanged(object sender,SizeChangedEventArgs e) {
            if(e.NewSize == e.PreviousSize) {
                return;
            }
            Regenerate(parametersChanged: false);
        }
    }
}
