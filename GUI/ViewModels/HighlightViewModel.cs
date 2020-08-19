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

namespace BelarusChess.UI.ViewModels
{
    public class HighlightViewModel : BoardImageViewModelBase
    {
        public HighlightViewModel(HighlightType highlight, Cell cell) : base()
        {
            Highlight = highlight;
            Cell = cell;

            Image.Source = new BitmapImage(new Uri(GetImageUri(Highlight), UriKind.Relative));
            Image.Margin = new Thickness(LeftMargin + cell.Col * CellEdge, TopMargin + cell.Row * CellEdge, 0, 0);
            Panel.SetZIndex(Image, 1);
            if (Highlight == HighlightType.ValidMoveOnPiece)
                Panel.SetZIndex(Image, 3);

            Image.MouseEnter += Image_MouseEnter;
            Image.MouseLeave += Image_MouseLeave;
        }

        public HighlightType Highlight { get; set; }
        public Cell Cell { get; set; }

        private string GetImageUri(HighlightType highlight)
        {
            string imageUri = "";

            switch (highlight)
            {
                case HighlightType.ChoosedPiece:
                    imageUri = "..\\Resources\\Highlight choosed piece.png";
                    Panel.SetZIndex(Image, 1);
                    break;
                case HighlightType.ValidMove:
                    imageUri = "..\\Resources\\Highlight valid move.png";
                    Panel.SetZIndex(Image, 1);
                    break;
                case HighlightType.ValidMoveOnPiece:
                    imageUri = "..\\Resources\\Highlight valid move on piece.png";
                    Panel.SetZIndex(Image, 3);
                    break;
                case HighlightType.MouseMove:
                    imageUri = "..\\Resources\\Highlight choosed piece.png";
                    break;
                case HighlightType.Check:
                    imageUri = "..\\Resources\\Highlight check.png";
                    Panel.SetZIndex(Image, 1);
                    break;
                case HighlightType.MovedPiece:
                    imageUri = "..\\Resources\\Highlight mouse move cell.png";
                    Panel.SetZIndex(Image, 1);
                    break;
            }

            return imageUri;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Highlight == HighlightType.ValidMove || Highlight == HighlightType.ValidMoveOnPiece)
                Image.Source = new BitmapImage(new Uri(GetImageUri(Highlight), UriKind.Relative));
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Highlight == HighlightType.ValidMove || Highlight == HighlightType.ValidMoveOnPiece)
                Image.Source = new BitmapImage(new Uri(GetImageUri(HighlightType.MouseMove), UriKind.Relative));
        }
    }
}
