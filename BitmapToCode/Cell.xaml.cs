using System;
using System.Collections.Generic;
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
    partial class Cell
    {
        public Cell()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty isFilledProperty = DependencyProperty.Register("IsFilled", typeof(bool), typeof(Cell));

        public bool IsFilled
        {
            get { return (bool)this.GetValue(isFilledProperty); }
            set { this.SetValue(isFilledProperty, value); }
        }

        private void MouseOver(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.IsFilled = true;
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                this.IsFilled = false;
            }
        }

        private void MouseEntered(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                this.IsFilled = !this.IsFilled;
            }
        }

        private void LeftDown(object sender, MouseButtonEventArgs e)
        {
            this.IsFilled = true;
        }

        private void RightDown(object sender, MouseButtonEventArgs e)
        {
            this.IsFilled = false;
        }

        private void MouseClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    this.IsFilled = true;
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    this.IsFilled = false;
                }
                else if (e.ChangedButton == MouseButton.Middle)
                {
                    this.IsFilled = !this.IsFilled;
                }
            }
        }
    }
}
