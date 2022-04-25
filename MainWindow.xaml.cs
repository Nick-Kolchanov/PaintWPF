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
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using Microsoft.Win32;
using System.Drawing.Imaging;
using System.IO;

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

        public enum BrushMode { Pen, Line, Ellipse, Eraser, Star }

        public static int CanvasWidth { get; set; }
        public static int CanvasHeight { get; set; }
        public static BrushMode CurBrushMode { get; set; }
        public static SolidColorBrush PenColor { get; set; }
        public static int PenWidth { get; set; }
        public static int StarApexNum { get; set; }
        public static double RadiusRelation { get; set; }
        public bool IsReadyToSave { get => isReadyToSave; set { isReadyToSave = value; saveButton.IsEnabled = value; saveAsButton.IsEnabled = value; } }

        public DrawWindow drawWindow;

        private static bool isReadyToSave;
        private string curFileName = "";

        private double zoomMin;
        private double zoomMax;
        public double zoom;

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
            CurBrushMode = BrushMode.Pen;
            zoom = 1;
            zoomMin = 0.1;
            zoomMax = 5;
            CanvasWidth = 800;
            CanvasHeight = 450;
        }

        private void NewCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            drawWindow = new DrawWindow(this);
            drawWindow.Show();
            curFileName = "";
            zoom = 1;
        }

        private void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Все файлы (*.*)|*.*| Windows Bitmap (*.bmp)|*.bmp| Файлы JPEG (*.jpeg, *.jpg)|*.jpeg;*.jpg| Файлы PNG (*.png)|*.png";
            if ((bool)dlg.ShowDialog())
            {
                drawWindow = new DrawWindow(this);
                BitmapImage bitmapBrush;
                var extFile = System.IO.Path.GetExtension(dlg.FileName);
                if (extFile != "jpg" && extFile != "jpeg" && extFile != "bmp" && extFile != "png")
                {
                    MessageBox.Show("Неверный тип файла");
                    return;
                }

                using (FileStream stream = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read))
                    
                using (BinaryReader reader = new BinaryReader(stream))
                {

                    var memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));

                    bitmapBrush = Convert(new Bitmap(memoryStream));
                }

                drawWindow.canvas.Background = new ImageBrush(bitmapBrush);
                drawWindow.Show();
                curFileName = dlg.FileName;
                zoom = 1;

            }

            IsReadyToSave = false;
        }

        public BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            src.Save(ms, ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
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

                using (var fs = File.OpenWrite(dlg.FileName))
                {
                    encoder.Save(fs);
                }

                IsReadyToSave = false;
                curFileName = dlg.FileName;
            } 
        }

        public void SaveWhileClosing(object sender)
        {
            if (sender is DrawWindow)
            {
                SaveCommandExecuted(sender, null); // todo fix this shit
            }
        }

        private void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!isReadyToSave)
                return;

            if (curFileName == "")
            {
                var fileNameWindow = new FileNameInput();
                if ((bool)fileNameWindow.ShowDialog())
                {
                    curFileName = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + fileNameWindow.fileName + ".jpg";
                }
                else
                {
                    return;
                }
            }

            Rect rect = new Rect(drawWindow.canvas.RenderSize);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Right, (int)rect.Bottom, 96d, 96d, PixelFormats.Default);
            rtb.Render(drawWindow.canvas);

            FileInfo fileInfo = new FileInfo(curFileName);

            BitmapEncoder encoder;
            if (fileInfo.Extension == "bmp")
            {
                encoder = new BmpBitmapEncoder();
            }
            else if (fileInfo.Extension == "png")
            {
                encoder = new PngBitmapEncoder();
            }
            else
            {
                encoder = new JpegBitmapEncoder();
            }

            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var fs = fileInfo.OpenWrite())
            {
                encoder.Save(fs);
            }

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
                CanvasWidth = resizeWindow.newWidth;
                CanvasHeight = resizeWindow.newHeight;

                if (drawWindow != null)
                {
                    drawWindow.Height = CanvasHeight;
                    drawWindow.Width = CanvasWidth;
                }
            }
        }

        private void WindowCascadeCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("todo");
        }

        private void WindowHorizontalCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("todo");
        }

        private void WindowVerticalCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("todo");
        }

        private void WindowSortCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("todo");
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
            ColorPickerWindow colorPicker = new ColorPickerWindow(PenColor);
            if ((bool)colorPicker.ShowDialog())
            {
                PenColor = new SolidColorBrush(colorPicker.SelectedColor);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isOk = false;

            if (!(sender is TextBox textBox))
            {
                return;
            }

            if (int.TryParse(textBox.Text, out int width))
            {
                if (width > 0)
                {
                    isOk = true;
                    PenWidth = width;
                }
            }

            if (!isOk)
            {
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else
            {
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            bool isOk = false;


            if (!(sender is TextBox textBox))
            {
                return;
            }

            if (int.TryParse(textBox.Text, out int apexNum))
            {
                if (apexNum > 0)
                {
                    isOk = true;
                    StarApexNum = apexNum;
                }
            }

            if (!isOk)
            {
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else
            {
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            bool isOk = false;


            if (!(sender is TextBox textBox))
            {
                return;
            }

            if (double.TryParse(textBox.Text, out double relation))
            {
                if (relation > 0)
                {
                    isOk = true;
                    RadiusRelation = relation;
                }
            }

            if (!isOk)
            {
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else
            {
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        private void CheckBrushmodeButton(int buttonNum)
        {
            penButton.Opacity = 1;
            lineButton.Opacity = 1;
            ellipseButton.Opacity = 1;
            eraserButton.Opacity = 1;
            starButton.Opacity = 1;

            if (buttonNum == 0)
                penButton.Opacity = 0.5;
            if (buttonNum == 1)
                lineButton.Opacity = 0.5;
            if (buttonNum == 2)
                ellipseButton.Opacity = 0.5;
            if (buttonNum == 3)
                eraserButton.Opacity = 0.5;
            if (buttonNum == 4)
                starButton.Opacity = 0.5;
        }

        private void penButton_Click(object sender, RoutedEventArgs e)
        {
            CurBrushMode = BrushMode.Pen;
            CheckBrushmodeButton(0);
        }

        private void lineButton_Click(object sender, RoutedEventArgs e)
        {
            CurBrushMode = BrushMode.Line;
            CheckBrushmodeButton(1);
        }

        private void ellipseButton_Click(object sender, RoutedEventArgs e)
        {
            CurBrushMode = BrushMode.Ellipse;
            CheckBrushmodeButton(2);
        }

        private void eraserButton_Click(object sender, RoutedEventArgs e)
        {
            CurBrushMode = BrushMode.Eraser;
            CheckBrushmodeButton(3);
        }

        private void starButton_Click(object sender, RoutedEventArgs e)
        {
            CurBrushMode = BrushMode.Star;
            CheckBrushmodeButton(4);
        }

        private void plusSizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (drawWindow == null)
                return;

            zoom += 0.2;
            if (zoom > zoomMax) { zoom = zoomMax; }

            drawWindow.canvas.RenderTransform = new ScaleTransform(zoom, zoom);
        }

        private void minusSizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (drawWindow == null)
                return;

            zoom -= 0.2;
            if (zoom < zoomMin) { zoom = zoomMin; }

            drawWindow.canvas.RenderTransform = new ScaleTransform(zoom, zoom);
        }
    }
}
