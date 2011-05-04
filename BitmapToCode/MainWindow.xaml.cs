﻿using System;
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
