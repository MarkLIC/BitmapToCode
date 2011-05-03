using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BitmapToCode
{
    partial class MainWindow
    {
        private static readonly NullableIntToUintConverterImpl nullableIntConverter = new NullableIntToUintConverterImpl();
        public MainWindow()
        {
            InitializeComponent();
        }

        public static IValueConverter NullableIntToUintConverter
        {
            get { return nullableIntConverter; }
        }

        private sealed class NullableIntToUintConverterImpl : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (uint)(value as int? ?? 1);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
