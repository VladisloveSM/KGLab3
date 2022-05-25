using ImageWrapper;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using Color = System.Drawing.Color;

namespace KG3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _img;

        List<string> options = new List<string>()
        {
            "Red",
            "Green",
            "Blue",
            "Value",
        };

        List<string> methods = new List<string>()
        {
            "Equalization",
            "Linear Contrast",
            "Median Filter",
        };

        public MainWindow()
        {
            InitializeComponent();
            Option.ItemsSource = (options);
            Method.ItemsSource = (methods);
            lblRadius.Visibility = Visibility.Hidden;
            Radius.Visibility = Visibility.Hidden;
        }

        private void Method_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Method.SelectedIndex == 2)
            {
                lblRadius.Visibility = Visibility.Visible;
                Radius.Visibility = Visibility.Visible;
            }
            else
            {
                lblRadius.Visibility = Visibility.Hidden;
                Radius.Visibility = Visibility.Hidden;
            }
        }

        private void Option_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_img) && Method.SelectedItem != null && Option.SelectedItem != null)
            {
                ImageWrapper.ImageWrapper iw = new ImageWrapper.ImageWrapper(_img);

                Func<Color, byte> get = null;
                Func<Color, byte, Color> set = null;

                switch (Option.SelectedIndex)
                {
                    case 0:
                        get = c => c.R;
                        set = (c, b) => Color.FromArgb(c.A, b, c.G, c.B);
                        break;
                    case 1:
                        get = c => c.G;
                        set = (c, b) => Color.FromArgb(c.A, c.R, b, c.B);
                        break;
                    case 2:
                        get = c => c.B;
                        set = (c, b) => Color.FromArgb(c.A, c.R, c.G, b);
                        break;
                    case 3:
                        get = c => (byte)Math.Round(ImageWrapper.ColorConverter.ToHsv(c).Value * 255);
                        set = (c, b) =>
                        {
                            var t = ImageWrapper.ColorConverter.ToHsv(c);
                            t.Value = b / 255f;
                            return ImageWrapper.ColorConverter.ToRgb(t);
                        };
                        break;
                }

                Bitmap bm = null;

                if (!Int32.TryParse(Radius.Text, out var radius) && radius > 0)
                {
                    MessageBox.Show("Radius must be a positive integer number");
                }

                switch (Method.SelectedIndex)
                {
                    case 0:
                        bm = iw.Equalization(get, set);
                        break;
                    case 1:
                        bm = iw.LinearContrast(get, set);
                        break;
                    case 2:
                        bm = iw.MedianFilter(radius, get, set);
                        break;
                }

                IntPtr hBitmap = bm.GetHbitmap();

                using (var memory = new MemoryStream())
                {
                    bm.Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    Second.Source = bitmapImage;
                }
                
            }
        }

        private void Choose_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                _img = openFileDialog.FileName;
                First.Source = new BitmapImage(new Uri(_img));
            }
        }
    }
}
