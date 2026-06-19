using Cristal;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CristalDesigner {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:Window {

        public const int TEXTURE_SIZE = 2048;

        public const float DEFAULT_SCALE = 10.0f;
        public const float SCALE_STEP = 0.5f;

        public MainWindow() {
            InitializeComponent();

            _bitmap = new WriteableBitmap(TEXTURE_SIZE,TEXTURE_SIZE,300,300,PixelFormats.Gray32Float,null);

            TextureGenerationTarget.Source = _bitmap;
            _factory = new(124);
        }

        private readonly WriteableBitmap _bitmap;

        private readonly Factory _factory;

        private float GetNoiseScale() {
            if(!float.TryParse(ScaleInput.Text,out float scale)) {
                scale = DEFAULT_SCALE;
            }
            ScaleInput.Text = scale.ToString();
            return scale;
        }

        private void Button_Click(object sender,RoutedEventArgs e) {
            using var texture = _factory.CreateNoiseTexture(new TextureSize(TEXTURE_SIZE),GetNoiseScale());

            byte[] bytes = MemoryMarshal.AsBytes(texture.Data).ToArray();
            int stride = texture.Size.Width * sizeof(float);
            _bitmap.WritePixels(new Int32Rect(0,0,texture.Size.Width,texture.Size.Height),bytes,stride,0);
        }

        private void ScalePlus_Click(object sender,RoutedEventArgs e) {
            if(float.TryParse(ScaleInput.Text,out float scale)) {
                scale += SCALE_STEP;
            } else {
                scale = DEFAULT_SCALE;
            }
            ScaleInput.Text = scale.ToString();
        }

        private void ScaleMinus_Click(object sender,RoutedEventArgs e) {
            if(float.TryParse(ScaleInput.Text,out float scale)) {
                scale -= SCALE_STEP;
            } else {
                scale = DEFAULT_SCALE;
            }
            ScaleInput.Text = scale.ToString();
        }
    }
}
