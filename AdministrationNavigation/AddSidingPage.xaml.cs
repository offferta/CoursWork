using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Coursework.Context;
using Coursework.Entities;
using static Coursework.Context.MyDbContext;
using Microsoft.Win32;

namespace Coursework.AdministrationNavigation;

public partial class AddSidingPage : Page
{
    private byte[] imageData;
    private Worker _worker;
    private byte[] selectedImageBytes;
    private Siding siningId;
    private List<Siding> siningList = new List<Siding>();
    
    public AddSidingPage(Worker worker)
    {
        InitializeComponent();
        _worker = worker;
    }

    private async void AddButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            await using var context = new MyDbContext();
            
            if (imageData == null)
            {
                MessageBox.Show("Please select an image.");
                return;
            }
            siningId = 
            context.Sidings.Update(new Siding()
            {
                Title = TitleTextBox.Text,
                Description = DescriptionTextBox.Text,
                Price = decimal.Parse(PriceTextBox.Text),
                Image = ConvertToBitmapImage(imageData)
            }).Entity;
            
            siningList.Add(siningId);

            MessageBox.Show("Добавление произведено");
            await context.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            MessageBox.Show("Непредвиденная ошибка" + exception);
        }
    }

    private void BrowseImage_OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.DefaultExt = ".jpg";
        dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

        if (dlg.ShowDialog() == true)
        {
            string imagePath = dlg.FileName;
            BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
            SelectedImage.Source = bitmap;

            imageData = File.ReadAllBytes(imagePath);
        }
    }
}