using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
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
using static Coursework.Context.MyDbContext;
using Coursework.Context;
using Coursework.Entities;
using Microsoft.Data.SqlClient;
using Window = System.Windows.Window;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte[] _imageData;
        
        public MainWindow()
        {
            InitializeComponent();
        }
        
        
        private async void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
            await using var context = new MyDbContext();
            
            if (_imageData == null)
            {
                MessageBox.Show("Please select an image.");
                return;
            }
                context.Sidings.Update(new Siding()
                {
                    Title = TitleTextBox.Text,
                    Description = DescriptionTextBox.Text,
                    Price = decimal.Parse(PriceTextBox.Text),
                    Image = ConvertToBitmapImage(_imageData)
                });
                await context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show("dfffsdf");
            }
        }

        private void BrowseImage_OnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            if (dlg.ShowDialog() == true)
            {
                string imagePath = dlg.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
                SelectedImage.Source = bitmap;

                // Read and store the image data as byte array
                _imageData = File.ReadAllBytes(imagePath);
            }
        }
    }
    
}