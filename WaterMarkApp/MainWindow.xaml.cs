using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WaterMarkApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Bitmap image;
        Bitmap waterMark;
        Bitmap newImage;
        OpenFileDialog openDialog = new OpenFileDialog();
        SaveFileDialog saveDialog = new SaveFileDialog();
      
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RenderImage()
        {

            if (image == null)
                throw new Exception("Картинка не выбрана");

            if (waterMark == null)
                throw new Exception("Watermark не выбран");

            int top = int.Parse(topBox.Text);
            int left = int.Parse(leftBox.Text);
            int right = int.Parse(rightBox.Text);
            int bot = int.Parse(botBox.Text);
            float scale = float.Parse(scaleBox.Text);

            newImage = new Bitmap(image);

            using (Graphics imageGraphics = Graphics.FromImage(newImage))
            {
                waterMark.SetResolution(imageGraphics.DpiX, imageGraphics.DpiY);

                int x = 0 , y = 0 ;

                switch (cmbHorizontal.SelectedItem)
                {
                    case "Center":
                        x = (int)(image.Width - waterMark.Width * scale) / 2;
                        break;
                    case "Left":
                        x = 0;
                        break;
                    case "Right":
                        x = (int)(image.Width - waterMark.Width * scale);
                        break;
                }

                switch (cmbVertical.SelectedItem)
                {
                    case "Center":
                        y = (int)(image.Height - waterMark.Height * scale) / 2;
                        break;
                    case "Top":
                        y = 0 ;
                        break;
                    case "Bottom":
                        y = (int)(image.Height - waterMark.Height * scale);
                        break;
                }

                x = x + left - right;
                y = y + top - bot;

                imageGraphics.DrawImage(waterMark, x, y, waterMark.Width * scale, waterMark.Height * scale);
            }

        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void btnMainImageSelect_Click(object sender, RoutedEventArgs e)
        {
            if (openDialog.ShowDialog() == true)
            {
                txtMainPath.Text = openDialog.FileName;
                image = (Bitmap)Bitmap.FromFile(openDialog.FileName);
            }
        }

        private void btnWatermarkSelect_Click(object sender, RoutedEventArgs e)
        {
            if (openDialog.ShowDialog() == true)
            {
                txtWatermarkPath.Text = openDialog.FileName;
                waterMark = (Bitmap)Bitmap.FromFile(openDialog.FileName);
            }
        }

        private void btnRebder_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                RenderImage();
                imageBox.Source = BitmapToImageSource(newImage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            openDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            openDialog.Title = "Please select an image.";

            saveDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            saveDialog.Title = "Please select path to save image";

            cmbHorizontal.ItemsSource = new string[] { "Left", "Center", "Right" } ;
            cmbVertical.ItemsSource = new string[] { "Top", "Center", "Bottom" };

            cmbHorizontal.SelectedIndex = 1;
            cmbVertical.SelectedIndex = 1;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RenderImage();

                if (saveDialog.ShowDialog() == true)
                    newImage.Save(saveDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

