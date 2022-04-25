using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Paint
{
    /// <summary>
    /// Логика взаимодействия для ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        public Color Color { get; set; }

        public event Action OnColorChanged;

        public ColorPicker()
        {
            InitializeComponent();
            Init(Color);
        }

        public void Init(Color newColor)
        {
            Color = newColor;

            redColorTextBox.ColorValue = Color.R;
            greenColorTextBox.ColorValue = Color.G;
            blueColorTextBox.ColorValue = Color.B;
            redColorTextBox.CurrentMode = ColorNumberTextBox.Mode.Decimal;
            greenColorTextBox.CurrentMode = ColorNumberTextBox.Mode.Decimal;
            blueColorTextBox.CurrentMode = ColorNumberTextBox.Mode.Decimal;

            OnColorChanged += ColorChanged;
            OnColorChanged?.Invoke();
        }

        private void ColorChanged()
        {
            Color = Color.FromRgb(redColorTextBox.ColorValue, greenColorTextBox.ColorValue, blueColorTextBox.ColorValue);
            colorPreviewRectangle.Fill = new SolidColorBrush(Color);
        }

        private void decRadio_Checked(object sender, RoutedEventArgs e)
        {
            redColorTextBox.CurrentMode = ColorNumberTextBox.Mode.Decimal;
            greenColorTextBox.CurrentMode = ColorNumberTextBox.Mode.Decimal;
            blueColorTextBox.CurrentMode = ColorNumberTextBox.Mode.Decimal;
        }

        private void hexRadio_Checked(object sender, RoutedEventArgs e)
        {
            redColorTextBox.CurrentMode = ColorNumberTextBox.Mode.Hex;
            greenColorTextBox.CurrentMode = ColorNumberTextBox.Mode.Hex;
            blueColorTextBox.CurrentMode = ColorNumberTextBox.Mode.Hex;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            OnColorChanged?.Invoke();
        }
    }
}
