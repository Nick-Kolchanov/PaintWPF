using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFColorPickerLib;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using Microsoft.Win32;
using System.Drawing.Imaging;

namespace Paint
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand ResizeCommand = new RoutedCommand();
        public static RoutedCommand WindowCascadeCommand = new RoutedCommand();
        public static RoutedCommand WindowHorizontalCommand = new RoutedCommand();
        public static RoutedCommand WindowVerticalCommand = new RoutedCommand();
        public static RoutedCommand WindowSortCommand = new RoutedCommand();

        public static Brush PenColor { get; set; }
        public static int PenWidth { get; set; }
        public bool IsReadyToSave { get => isReadyToSave; set { isReadyToSave = value; saveButton.IsEnabled = value; saveAsButton.IsEnabled = value; } }

        public DrawWindow drawWindow;
        private static bool isReadyToSave;

        public MainWindow()
        {
            InitializeComponent();
            ResizeCommand.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));
            WindowCascadeCommand.InputGestures.Add(new KeyGesture(Key.H, ModifierKeys.Control));
            WindowHorizontalCommand.InputGestures.Add(new KeyGesture(Key.J, ModifierKeys.Control));
            WindowVerticalCommand.InputGestures.Add(new KeyGesture(Key.K, ModifierKeys.Control));
            WindowSortCommand.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Control));

            PenColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            PenWidth = 3;
            IsReadyToSave = false;
        }

        private void NewCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            drawWindow = new DrawWindow(this);
            drawWindow.Show();
        }

        private void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpeg, *.jpg)|*.jpeg;*.jpg| Файлы PNG (*.png)|*.png";
            if ((bool)dlg.ShowDialog())
            {
                drawWindow = new DrawWindow(this);
                drawWindow.canvas.Background = new ImageBrush(new BitmapImage(new Uri(dlg.FileName)));
            }
            drawWindow.Show();
        }

        private void SaveAsCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpeg, *.jpg)|*.jpeg;*.jpg | Файлы PNG (*.png)|*.png";
            if ((bool)dlg.ShowDialog())
            {
                Rect rect = new Rect(drawWindow.canvas.RenderSize);
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Right,
                  (int)rect.Bottom, 96d, 96d, PixelFormats.Default);
                rtb.Render(drawWindow.canvas);

                BitmapEncoder encoder;
                if (dlg.FilterIndex == 1)
                {
                    encoder = new BmpBitmapEncoder();
                }
                else if (dlg.FilterIndex == 2)
                {
                    encoder = new JpegBitmapEncoder();
                }
                else
                {
                    encoder = new PngBitmapEncoder();
                }

                encoder.Frames.Add(BitmapFrame.Create(rtb));

                using (var fs = System.IO.File.OpenWrite(dlg.FileName))
                {
                    encoder.Save(fs);
                }
            }

            IsReadyToSave = false;
        }

        private void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("ddd");
            IsReadyToSave = false;
        }

        private void ExitCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void ResizeCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var resizeWindow = new ResizeWindow();
            if ((bool)resizeWindow.ShowDialog())
            {
                drawWindow.Height = resizeWindow.newHeight;
                drawWindow.Width = resizeWindow.newWidth;
            }
        }

        private void WindowCascadeCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("ddd");
        }

        private void WindowHorizontalCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("ddd");
        }

        private void WindowVerticalCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("ddd");
        }

        private void WindowSortCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("ddd");
        }

        private void HelpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Программа для рисования 'Пэинт'. Автор: Колчанов Никита, ПИ-20-2. Copyright © 2022");
        }

        private void ChangeColorButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement) != null)
            {
                (sender as FrameworkElement).ContextMenu.IsOpen = true;
            }
        }

        private void ChangeRedColor_Click(object sender, RoutedEventArgs e)
        {
            PenColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }

        private void ChangeGreenColor_Click(object sender, RoutedEventArgs e)
        {
            PenColor = new SolidColorBrush(Color.FromRgb(0, 255, 0));
        }

        private void ChangeBlueColor_Click(object sender, RoutedEventArgs e)
        {
            PenColor = new SolidColorBrush(Color.FromRgb(0, 0, 255));
        }

        private void ChangeOtherColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Owner = this;
            if ((bool)colorDialog.ShowDialog())
            {
                PenColor = new SolidColorBrush(colorDialog.SelectedColor);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse((sender as TextBox)?.Text, out int width))
                PenWidth = width;
        }
    }
}
