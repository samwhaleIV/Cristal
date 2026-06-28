using Microsoft.UI.Xaml.Data;
using System;

namespace CristalLab.Pages {
    public sealed partial class ScaleStringFormatter:IValueConverter {
        public object Convert(object value,Type targetType,object parameter,string language) {
            return $"{(double)value:F1}";
        }

        public object ConvertBack(object value,Type targetType,object parameter,string language) {
            throw new NotImplementedException();
        }
    }
}
