
using LAMA.Auth;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAMA
{
    public class SenderIdIsOtherUser : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string senderId = value as string;
            string currentUserId = UserSession.CurrentUser.Uid;

            return senderId != currentUserId;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
