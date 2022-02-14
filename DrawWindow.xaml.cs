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

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line
                {
                    Stroke = MainWindow.PenColor,
                    StrokeThickness = MainWindow.PenWidth,
                    X1 = currentPoint.X,
                    Y1 = currentPoint.Y,
                    X2 = e.GetPosition(this).X,
                    Y2 = e.GetPosition(this).Y
                };

                currentPoint = e.GetPosition(this);

                canvas.Children.Add(line);

                parent.IsReadyToSave = true;
            }
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                currentPoint = e.GetPosition(this);
        }

        public DrawWindow(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            parent.IsReadyToSave = false;
            //dialog
        }
    }
}
