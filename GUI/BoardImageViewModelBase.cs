using BelarusChess.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BelarusChess.UI
{
    public abstract class BoardImageViewModelBase : ViewModelBase
    {
        protected const int LeftMargin = 10;
        protected const int TopMargin = 10;
        protected const int CellEdge = 55;

        protected BoardImageViewModelBase()
        {
            Image.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => OnImageClicked?.Invoke(this, e);
        }

        public event EventHandler OnImageClicked;

        public Image Image { get; set; } = new Image
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Width = CellEdge,
            Height = CellEdge
        };
    }
}
