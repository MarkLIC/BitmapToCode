/*
Copyright 2011 Mark Rushakoff, Lafayette Instrument Company. All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are
permitted provided that the following conditions are met:

   1. Redistributions of source code must retain the above copyright notice, this list of
      conditions and the following disclaimer.

   2. Redistributions in binary form must reproduce the above copyright notice, this list
      of conditions and the following disclaimer in the documentation and/or other materials
      provided with the distribution.

THIS SOFTWARE IS PROVIDED BY Mark Rushakoff, Lafayette Instrument Company ''AS IS'' AND ANY EXPRESS OR IMPLIED
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those of the
authors and should not be interpreted as representing official policies, either expressed
or implied, of Mark Rushakoff, Lafayette Instrument Company.
 */

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
