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

            var lookup = this.mainGrid.Children.Cast<UIElement>().ToLookup(x => new Tuple<int, int>(Grid.GetColumn(x), Grid.GetRow(x)));

            for (int r = 0; r < maxRow; r++)
            {
                for (int c = 0; c < maxCol; c++)
                {
                    if (lookup.Contains(new Tuple<int, int>(c, r)))
                    {
                        continue;
                    }

                    var cell = new Cell();
                    Grid.SetRow(cell, r);
                    Grid.SetColumn(cell, c);
                    this.mainGrid.Children.Add(cell);
                }
            }

            foreach (var item in lookup.Where(x => x.Key.Item1 >=maxCol || x.Key.Item2 >= maxRow).SelectMany(x => x.AsEnumerable()))
            {
                this.mainGrid.Children.Remove(item);
            }

            foreach (var item in lookup)
            {
                System.Diagnostics.Debug.Assert(item.Count() == 1, "More than one element in cell");
            }
        }
    }
}
