using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAMA
{
    public class BoolToBookmarkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isBookmarked)
            {
                return isBookmarked ? "\u2605" : "\u2606";
                // return isBookmarked ? "Saved" : "Save";
            }
            return "\u2606";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
