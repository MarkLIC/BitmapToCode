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
    partial class CellCollection
    {
        private readonly Dictionary<Tuple<int, int>, Cell> cells = new Dictionary<Tuple<int, int>, Cell>();
        public CellCollection()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty numColumnsProperty = DependencyProperty.Register("NumColumns", typeof(uint), typeof(CellCollection));
        public uint NumColumns
        {
            get { return (uint)this.GetValue(numColumnsProperty); }
            set { this.SetValue(numColumnsProperty, value); }
        }

        public static readonly DependencyProperty numRowsProperty = DependencyProperty.Register("NumRows", typeof(uint), typeof(CellCollection));
        public uint NumRows
        {
            get { return (uint)this.GetValue(numRowsProperty); }
            set { this.SetValue(numRowsProperty, value); }
        }

        public static readonly DependencyProperty cellDimensionProperty = DependencyProperty.Register("CellDimension", typeof(int), typeof(CellCollection));
        public int CellDimension
        {
            get { return (int)this.GetValue(cellDimensionProperty); }
            set { this.SetValue(cellDimensionProperty, value); }
        }

        public ICommand ClearCommand
        {
            get
            {
                return new CellCollectionCommand(() =>
                                                     {
                                                         foreach (var cell in this.mainGrid.Children.OfType<Cell>())
                                                         {
                                                             cell.IsFilled = false;
                                                         }
                                                     });
            }
        }

        public ICommand InvertCommand
        {
            get
            {
                return new CellCollectionCommand(() =>
                                                     {
                                                         foreach (var cell in this.mainGrid.Children.OfType<Cell>())
                                                         {
                                                             cell.IsFilled = !cell.IsFilled;
                                                         }
                                                     });
            }
        }

        public Cell CellAt(int x, int y)
        {
            var key = new Tuple<int, int>(x, y);
            return this.cells.ContainsKey(key) ? this.cells[key] : null;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == numColumnsProperty)
            {
                var old = (uint)e.OldValue;
                var @new = (uint)e.NewValue;

                if (old < @new)
                {
                    for (uint c = old; c < @new; c++)
                    {
                        this.mainGrid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(this.CellDimension, GridUnitType.Pixel)});
                    }
                }
                else
                {
                    for (uint i = old; i > @new; i--)
                    {
                        this.mainGrid.ColumnDefinitions.RemoveAt(this.mainGrid.ColumnDefinitions.Count - 1);
                    }
                }

                this.FillChildren();
            }
            else if (e.Property == numRowsProperty)
            {
                var old = (uint)e.OldValue;
                var @new = (uint)e.NewValue;

                if (old < @new)
                {
                    for (uint r = old; r < @new; r++)
                    {
                        this.mainGrid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(this.CellDimension, GridUnitType.Pixel)});
                    }
                }
                else
                {
                    for (uint i = old; i > @new; i--)
                    {
                        this.mainGrid.RowDefinitions.RemoveAt(this.mainGrid.RowDefinitions.Count - 1);
                    }
                }

                this.FillChildren();
            }
            else if (e.Property == cellDimensionProperty)
            {
                foreach (var def in this.mainGrid.ColumnDefinitions)
                {
                    def.Width = new GridLength(this.CellDimension, GridUnitType.Pixel);
                }

                foreach (var def in this.mainGrid.RowDefinitions)
                {
                    def.Height = new GridLength(this.CellDimension, GridUnitType.Pixel);
                }
            }

            base.OnPropertyChanged(e);
        }

        private void FillChildren()
        {
            int maxRow = this.mainGrid.RowDefinitions.Count;
            int maxCol = this.mainGrid.ColumnDefinitions.Count;

            var existingChildren = this.mainGrid.Children.Cast<UIElement>().ToLookup(x => new Tuple<int, int>(Grid.GetColumn(x), Grid.GetRow(x)));

            for (int r = 0; r < maxRow; r++)
            {
                for (int c = 0; c < maxCol; c++)
                {
                    if (existingChildren.Contains(new Tuple<int, int>(c, r)))
                    {
                        continue;
                    }

                    var cell = new Cell();
                    Grid.SetRow(cell, r);
                    Grid.SetColumn(cell, c);
                    this.mainGrid.Children.Add(cell);
                }
            }

            foreach (var item in existingChildren.Where(x => x.Key.Item1 >= maxCol || x.Key.Item2 >= maxRow).SelectMany(x => x.AsEnumerable()))
            {
                this.mainGrid.Children.Remove(item);
            }

            var lookup = this.mainGrid.Children.Cast<UIElement>().ToLookup(x => new Tuple<int, int>(Grid.GetColumn(x), Grid.GetRow(x)));
            System.Diagnostics.Debug.Assert(lookup.All(item => item.Count() == 1), "More than one element in cell");

            this.cells.Clear();
            foreach (var item in lookup)
            {
                this.cells[item.Key] = (Cell)item.Single();
            }
        }

        private sealed class CellCollectionCommand : ICommand
        {
            private readonly Action action;

            public CellCollectionCommand(Action action)
            {
                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }

                this.action = action;
            }

            public void Execute(object parameter)
            {
                this.action();
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                add { }
                remove { }
            }
        }
    }
}
