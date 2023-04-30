using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Schach
{
    public partial class MainWindow : Window
    {
        ShessPiece[] pieces = new ShessPiece[8];
        TextBlock[,] textBlocks = new TextBlock[8, 8];
        ShessPiece movedPiece;

        // Rock rockW, rockB;

        public MainWindow()
        {
            string a = "\u2654, \u2655, \u2656, \u2657, \u2658, \u2659, \u265A, \u265B, \u265C, \u265D, \u265E, \u265F";
            InitializeComponent();

            pieces[0] = new Rock(new Point(0, 7), true);
            pieces[1] = new Rock(new Point(7, 7), true);
            pieces[2] = new Rock(new Point(0, 0), false);
            pieces[3] = new Rock(new Point(7, 0), false);
            pieces[4] = new Bishop(new Point(2, 7), true);
            pieces[5] = new Bishop(new Point(5, 7), true);
            pieces[6] = new Bishop(new Point(2, 0), false);
            pieces[7] = new Bishop(new Point(5, 0), false);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    TextBlock b = new TextBlock();
                    b.Width = 60; b.Height = 60;
                    b.Text = "*"; b.FontSize = 45;
                    b.VerticalAlignment = VerticalAlignment.Center;
                    b.TextAlignment = TextAlignment.Center;
                    b.Foreground = Brushes.Black;
                    b.Background = ((i + j)) % 2 != 0 ? Brushes.White : Brushes.LightGray;
                    textBlocks[i, j] = b;
                    myCanvas.Children.Add(b);
                    Canvas.SetLeft(b, 60 * i);
                    Canvas.SetBottom(b, 60 * j);
                    b.MouseDown += mousePushed;
                    b.MouseUp += mouseReleased;
                    b.MouseEnter += showWays;
                    b.MouseLeave += removeWays;
                }
            }
            showPieces();
        }

        void showWays(object sender, MouseEventArgs mouseEventArgs)
        {
            TextBlock tB = (TextBlock)sender;
            Point point = getBlockCoords(tB);

            ShessPiece pieceTip = searchPiece(point);

            for (int f = 0; f < pieces.Length; f++)
            {
                if (pieces[f].Position.X == point.X && pieces[f].Position.Y == point.Y)
                {
                    pieceTip = pieces[f];

                }

            }

            if (pieceTip != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (pieceTip.isMovePossible(new Point (i, j)))
                            {
                                 textBlocks[i, j].Background = Brushes.LightGreen;
                            }
                        }
                    }
                }
            }
        }


        void removeWays(object sender, MouseEventArgs mouseEventArgs)
        {

            for (int i = 0; i < 8; i++)
            {
                {
                    for (int j = 0; j < 8; j++)
                    {
                        textBlocks[i,j].Background = ((i + j)) % 2 != 0 ? Brushes.White : Brushes.LightGray;
                    }
                }
            }
        }

        ShessPiece searchPiece (Point point)
        {
            for (int f = 0; f < pieces.Length; f++)
            {
                if (pieces[f].Position.X == point.X && pieces[f].Position.Y == point.Y)
                {
                    return pieces[f];

                }
            }

            return null;
        }


        void mousePushed(object sender, MouseEventArgs mouseArgs)
        {
            TextBlock tB = (TextBlock)sender;
            Point point = getBlockCoords(tB);
          
            movedPiece = searchPiece(point);
        }

        void mouseReleased(object sender, MouseEventArgs mouseArgs)
        {
            TextBlock tB = (TextBlock)sender;
            Point point = getBlockCoords(tB);

            if (movedPiece != null)
            {
                if (movedPiece.moveTo(point))
                {
                    showPieces();
                }
                else
                {
                    MessageBox.Show("Nicht erlaubter Zug", Title = "Achtung!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        Point getBlockCoords(TextBlock tB)
        {
            Point point = new Point(-1, -1);
            
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (textBlocks[i, j] == tB)
                    {
                        point = new Point (i, j);
                    }
                }
            }

            return point;
        }

        void showPieces()
        {
            for (int i = 0; i< 8; i++)
            { 
                for (int j = 0; j< 8; j++)
                {
                    textBlocks[i, j].Text = "";
                    for (int k = 0; k<pieces.Length; k++)
                    {
                        if (pieces[k].Position.X == i && pieces[k].Position.Y == j)
                        {
                            textBlocks[i, j].Text = pieces[k].getForm.ToString();
                            break;
                        }
                    }
                }
            }
        }
    }
}
