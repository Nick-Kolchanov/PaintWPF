﻿using System;
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
    /// Логика взаимодействия для FileNameInput.xaml
    /// </summary>
    public partial class FileNameInput : Window
    {
        public string fileName;

        public FileNameInput()
        {
            InitializeComponent();
        }

        private void okBnt_Click(object sender, RoutedEventArgs e)
        {
            fileName = fileNameTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void cancelBnt_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
