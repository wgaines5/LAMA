using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using LAMA.Auth;

namespace LAMA
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string senderId = value as string;
            string currentUserId = UserSession.CurrentUser.Uid;

            return senderId == currentUserId ? "#26c7b6" : "#01a4ff";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
