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
using System.Windows.Shapes;

namespace Paint
{
    /// <summary>
    /// Логика взаимодействия для ResizeWindow.xaml
    /// </summary>
    public partial class ResizeWindow : Window
    {
        public int newWidth;
        public int newHeight;

        public ResizeWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(widthTextBox.Text, out newWidth) && int.TryParse(heightTextBox.Text, out newHeight))
            {
                DialogResult = true;
                Close();
            }
            else
                MessageBox.Show("Некорректный ввод!");
        }
    }
}
