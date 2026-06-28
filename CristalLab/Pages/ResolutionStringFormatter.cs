using Microsoft.UI.Xaml.Data;
using System;

namespace CristalLab.Pages {
    public sealed partial class ResolutionStringFormatter:IValueConverter {
        public object Convert(object value,Type targetType,object parameter,string language) {
            double resolution = Math.Pow(2,(double)value);
            return $"{resolution:F0}x{resolution:F0}";
        }
        public object ConvertBack(object value,Type targetType,object parameter,string language) {
            throw new NotImplementedException();
        }
    }
}
