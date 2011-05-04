using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace BitmapToCode
{
    internal sealed class CellDesigner : DependencyObject
    {
        private static readonly CellDesigner instance = new CellDesigner();

        private CellDesigner()
        {
        }

        // singleton class to access dependency properties
        public static CellDesigner Instance
        {
            get { return instance; }
        }

        public static readonly DependencyProperty borderProperty = DependencyProperty.Register("Border", typeof(SolidColorBrush), typeof(CellDesigner));
        public SolidColorBrush Border
        {
            get { return (SolidColorBrush)this.GetValue(borderProperty); }
            set { this.SetValue(borderProperty, value); }
        }
    }
}
