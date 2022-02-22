using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using Pen = System.Drawing.Pen;
using Point = System.Windows.Point;

namespace Paint
{
    /// <summary>
    /// Логика взаимодействия для DrawWindow.xaml
    /// </summary>
    public partial class DrawWindow : Window
    {
        Point currentPoint = new Point();
        readonly MainWindow parent;

        Point elemStartingPoint;
        int elemIndex = -1;

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (MainWindow.CurBrushMode == MainWindow.BrushMode.Pen)
                {
                    Line line = new Line
                    {
                        Stroke = MainWindow.PenColor,
                        StrokeThickness = MainWindow.PenWidth,
                        X1 = currentPoint.X * (1 / parent.zoom),
                        Y1 = currentPoint.Y * (1 / parent.zoom),
                        X2 = e.GetPosition(this).X * (1 / parent.zoom),
                        Y2 = e.GetPosition(this).Y * (1 / parent.zoom)
                    };

                    currentPoint = e.GetPosition(this);

                    canvas.Children.Add(line);
                }

                if (MainWindow.CurBrushMode == MainWindow.BrushMode.Line)
                {
                    if (elemIndex != -1)
                        canvas.Children.RemoveAt(elemIndex);

                    Line line = new Line
                    {
                        Stroke = MainWindow.PenColor,
                        StrokeThickness = MainWindow.PenWidth,
                        X1 = elemStartingPoint.X * (1 / parent.zoom),
                        Y1 = elemStartingPoint.Y * (1 / parent.zoom),
                        X2 = e.GetPosition(this).X * (1 / parent.zoom),
                        Y2 = e.GetPosition(this).Y * (1 / parent.zoom)
                    };

                    elemIndex = canvas.Children.Add(line);
                }

                if (MainWindow.CurBrushMode == MainWindow.BrushMode.Ellipse)
                {
                    if (elemIndex != -1)
                        canvas.Children.RemoveAt(elemIndex);

                    Ellipse ellipse = new Ellipse
                    {
                        Stroke = MainWindow.PenColor,
                        StrokeThickness = MainWindow.PenWidth,
                        Width = Math.Abs(e.GetPosition(this).X - elemStartingPoint.X) * (1 / parent.zoom),
                        Height = Math.Abs(e.GetPosition(this).Y - elemStartingPoint.Y) * (1 / parent.zoom),
                    };

                    Canvas.SetLeft(ellipse, Math.Min(elemStartingPoint.X, e.GetPosition(this).X) * (1 / parent.zoom));
                    Canvas.SetTop(ellipse, Math.Min(elemStartingPoint.Y, e.GetPosition(this).Y) * (1 / parent.zoom));

                    elemIndex = canvas.Children.Add(ellipse);
                }

                if (MainWindow.CurBrushMode == MainWindow.BrushMode.Eraser)
                {
                    Line line = new Line
                    {
                        Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        StrokeThickness = MainWindow.PenWidth,
                        X1 = currentPoint.X * (1 / parent.zoom),
                        Y1 = currentPoint.Y * (1 / parent.zoom),
                        X2 = e.GetPosition(this).X * (1 / parent.zoom),
                        Y2 = e.GetPosition(this).Y * (1 / parent.zoom)
                    };

                    currentPoint = e.GetPosition(this);

                    canvas.Children.Add(line);
                }

                if (MainWindow.CurBrushMode == MainWindow.BrushMode.Star)
                {
                    if (elemIndex != -1)
                        canvas.Children.RemoveAt(elemIndex);

                    if (Math.Abs(e.GetPosition(this).X - elemStartingPoint.X) < 5 || Math.Abs(e.GetPosition(this).Y - elemStartingPoint.Y) < 5)
                    {
                        elemIndex = -1;
                        return;
                    }
                        

                    int n = MainWindow.StarApexNum;
                    double R = 25, r = MainWindow.RadiusRelation * R;
                    double alpha = 1;
                    double x0 = 0, y0 = 0;
                    List<Point> listPoints = new List<Point>();
                    double a = alpha, da = Math.PI / n, l;
                    double size = Math.Min(Math.Abs(e.GetPosition(this).X - elemStartingPoint.X) * (1 / parent.zoom), Math.Abs(e.GetPosition(this).Y - elemStartingPoint.Y) * (1 / parent.zoom)) / 50;
                    for (int k = 0; k < 2 * n + 1; k++)
                    {
                        l = k % 2 == 0 ? r : R;
                        listPoints.Add(new Point((int)((x0 + l * Math.Cos(a)) * size), (int)((y0 + l * Math.Sin(a)) * size)));
                        a += da;
                    }

                    Polyline pl = new Polyline()
                    {
                        Stroke = MainWindow.PenColor,
                        StrokeThickness = MainWindow.PenWidth,
                        Points = new PointCollection(listPoints)
                    };

                    Canvas.SetLeft(pl, (elemStartingPoint.X + e.GetPosition(this).X) / 2 * (1 / parent.zoom));
                    Canvas.SetTop(pl, (elemStartingPoint.Y + e.GetPosition(this).Y) / 2 * (1 / parent.zoom));

                    elemIndex = canvas.Children.Add(pl);
                }

                parent.IsReadyToSave = true;
            }
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                currentPoint = e.GetPosition(this);

            elemStartingPoint = currentPoint;
            elemIndex = -1;

            
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        public DrawWindow(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!parent.IsReadyToSave)
            {
                return;
            }


            e.Cancel = true;

            var saveOrCloseWindow = new SaveOrCloseWindow();
            if ((bool)saveOrCloseWindow.ShowDialog())
            {
                if (saveOrCloseWindow.isCancel)
                    return;
                parent.SaveWhileClosing(this);
                e.Cancel = false;
            }
            else
            {
                if (saveOrCloseWindow.isCancel)
                    return;
                e.Cancel = false;
            }

            parent.IsReadyToSave = false;
        }
    }
}
