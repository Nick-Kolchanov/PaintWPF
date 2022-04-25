using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Paint
{
    internal class ColorNumberTextBox : TextBox
    {
        public enum Mode { Decimal, Hex };
        private Mode currentMode;
        public Mode CurrentMode { get { return currentMode; } set { currentMode = value; ConvertText(); } }

        public byte ColorValue;

        private void ConvertText()
        {
            if (currentMode == Mode.Decimal)
            {
                Text = ColorValue.ToString();
            }
            else if (currentMode == Mode.Hex)
            {
                Text = ColorValue.ToString("X");
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {

            int tmp = 0;
            try
            {
                if (currentMode == Mode.Decimal)
                {
                    tmp = Convert.ToInt32(Text);
                }
                else if (currentMode == Mode.Hex)
                {
                    tmp = Convert.ToInt32(Text, 16);
                }
            }
            catch (Exception)
            {
            }

            if (tmp < 0)
                ColorValue = 0;
            else if (tmp > 255)
                ColorValue = 255;
            else
            {
                try
                {
                    ColorValue = Convert.ToByte(tmp);
                }
                catch (Exception)
                {
                    ColorValue = 0;
                }
            }

            if (Text == "")
                ColorValue = 0;

            if (Text.Length > 3)
                ColorValue = 255;


            if (currentMode == Mode.Decimal)
            {
                Text = ColorValue.ToString();
            }
            else if (currentMode == Mode.Hex)
            {
                Text = ColorValue.ToString("X");
            }

            base.OnTextChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                base.OnKeyDown(e);
                return;
            }

            if (currentMode == Mode.Hex && (e.Key >= Key.A && e.Key <= Key.F))
            {
                base.OnKeyDown(e);
                return;
            }

            e.Handled = true;
        }
    }
}
