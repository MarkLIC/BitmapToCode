﻿/*
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
using Jurassic;

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

        private void ExecuteScript(object sender, RoutedEventArgs e)
        {
            this.textBoxConsole.Clear();

            var engine = new ScriptEngine();

            engine.SetGlobalFunction("print", new Action<string>(s => this.textBoxConsole.AppendText(s)));
            engine.SetGlobalFunction("println", new Action<string>(s => this.textBoxConsole.AppendText(s + Environment.NewLine)));
            engine.SetGlobalFunction("getRows", new Func<int>(() => this.nudRows.Value ?? 1));
            engine.SetGlobalFunction("getColumns", new Func<int>(() => this.nudColumns.Value ?? 1));
            engine.SetGlobalFunction("setRows", new Action<int>(i => this.nudRows.Value = i));
            engine.SetGlobalFunction("setColumns", new Action<int>(i => this.nudColumns.Value = i));
            engine.SetGlobalFunction("invertAll", new Action(() => this.cellCollection.InvertCommand.Execute(null)));
            engine.SetGlobalFunction("clearAll", new Action(() => this.cellCollection.ClearCommand.Execute(null)));
            engine.SetGlobalFunction("isFilled", new Func<int, int, bool>((x, y) => (this.cellCollection.CellAt(x, y) ?? new Cell()).IsFilled));
            engine.SetGlobalFunction("setFilled", new Action<int, int, bool>((x, y, fill) => (this.cellCollection.CellAt(x, y) ?? new Cell()).IsFilled = fill));
            engine.SetGlobalFunction("setBorderColor", new Action<string>(s =>
                                                                              {
                                                                                  try
                                                                                  {
                                                                                      this.colorPickerBorder.SelectedColor = (Color)ColorConverter.ConvertFromString(s);
                                                                                  }
                                                                                  catch (FormatException ex)
                                                                                  {
                                                                                      throw new JavaScriptException(engine, "Error", "Cannot parse color: " + s, ex);
                                                                                  }
                                                                              }));

            try
            {
                engine.Execute(this.textBoxCode.Text);
            }
            catch (JavaScriptException ex)
            {
                this.textBoxConsole.Text += "-------------------" + Environment.NewLine + "Error: " + ex.Message;
            }
        }

        private void CodeKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                this.ExecuteScript(this, e);
            }
        }

        private void Dump(object sender, RoutedEventArgs e)
        {
            this.textBoxConsole.Clear();

            var builder = new StringBuilder();
            builder.AppendLine("// Dump saved at " + DateTime.UtcNow.ToString("u"));
            builder.AppendLine();
            builder.AppendLine("setColumns(" + (this.nudColumns.Value ?? 1).ToString(CultureInfo.InvariantCulture) + ")");
            builder.AppendLine("setRows(" + (this.nudRows.Value ?? 1).ToString(CultureInfo.InvariantCulture) + ")");
            builder.AppendLine("setBorderColor('" + this.colorPickerBorder.SelectedColor + "')");
            builder.AppendLine();
            builder.AppendLine("clearAll()");

            builder.AppendLine("var pairs = [");
            for (int col = 0; col < (this.nudColumns.Value ?? 0); col++)
            {
                for (int row = 0; row < (this.nudRows.Value ?? 0); row++)
                {
                    var cell = this.cellCollection.CellAt(col, row);
                    if (cell != null && cell.IsFilled)
                    {
                        builder.AppendLine("  [" + col.ToString(CultureInfo.InvariantCulture) + ", " + row.ToString(CultureInfo.InvariantCulture) + "],");
                    }
                }
            }

            builder.AppendLine("]");
            builder.AppendLine();

            builder.AppendLine("for (var i = 0; i < pairs.length; i++)");
            builder.AppendLine("{");
            builder.AppendLine("  setFilled(pairs[i][0], pairs[i][1], true)");
            builder.AppendLine("}");

            this.textBoxConsole.Text = builder.ToString();
        }

        private void ShowHelp(object sender, RoutedEventArgs e)
        {
            var builder = new StringBuilder();
            builder.AppendLine("BitmapToCode Help");
            builder.AppendLine();
            builder.AppendLine("Pre-defined JavaScript functions:");
            builder.AppendLine("  print(string): Outputs text to the console on the right.");
            builder.AppendLine("  println(string): Outputs text to the console on the right, automatically append newline.");
            builder.AppendLine("  getRows(): Returns the configured number of rows on the grid.");
            builder.AppendLine("  getColumns(): Returns the configured number of columns on the grid.");
            builder.AppendLine("  setRows(int): Sets the number of rows on the grid.");
            builder.AppendLine("  setColumns(int): Sets the number of columns on the grid.");
            builder.AppendLine("  invertAll(): Inverts all of the pixels on the grid.");
            builder.AppendLine("  clearAll(): Clears all of the pixels on the grid.");
            builder.AppendLine("  isFilled(int, int): Returns true or false indicating whether the cell at the given x, y coordinate is filled.");
            builder.AppendLine("  setFilled(int, int, bool): Sets the fill of the the cell at the given x, y coordinate to the given state.");
            builder.AppendLine("  setBorderColor(string): Sets the cell border color to the given string in hex format.");
            builder.AppendLine();
            builder.AppendLine("Quickly execute code by pressing ctrl-enter in the code text area.");

            MessageBox.Show(builder.ToString(), "Help");
        }

        private void BorderColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            CellDesigner.Instance.Border = new SolidColorBrush(this.colorPickerBorder.SelectedColor);
        }

        private void ComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selection = e.AddedItems.Count > 0 ? (string)e.AddedItems[0] : null;
            string preset = CodePresets.GetPreset(selection);
            if (preset != null)
            {
                this.textBoxCode.Text = preset;
            }
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
