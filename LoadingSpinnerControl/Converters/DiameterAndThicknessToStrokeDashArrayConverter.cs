using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace LoadingSpinnerControl.Converters
{
    public class DiameterAndThicknessToStrokeDashArrayConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            if (
                values.Length < 2
                || !double.TryParse(values[0].ToString(), out double diameter)
                || !double.TryParse(values[1].ToString(), out double thickness)
            )
            {
                return 0;
            }
            double circumference = Math.PI * diameter;

            double lineLenght = circumference * 0.75;
            double gabLenght = circumference - lineLenght;

            return new DoubleCollection(new[] { lineLenght / thickness, gabLenght / thickness });
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture
        )
        {
            throw new NotImplementedException();
        }
    }
}
