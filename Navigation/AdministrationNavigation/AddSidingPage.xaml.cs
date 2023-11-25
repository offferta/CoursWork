using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Coursework.Context;
using Coursework.Entities;
using Microsoft.Win32;
using static Coursework.Context.MyDbContext;

namespace Coursework.AdministrationNavigation;

public partial class
    AddSidingPage : Page
{
    private readonly List<Siding> _siningList = new();
    private readonly List<FeaturesMaterial> _featuresMaterials = new();
    private byte[] _imageData;
    private byte[] _selectedImageBytes;
    private Siding _siningId;
    private Worker _worker;
    private FeaturesMaterial _featurmMaterialWidth;
    private FeaturesMaterial _featurmMaterialLength;
    private FeaturesMaterial _featurmMaterialColor;
    private FeaturesMaterial _featurmMaterialDescription;

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
            if (_imageData == null)
            {
                MessageBox.Show("Please select an image.");
                return;
            }
            
            _siningId = context.Sidings.Update(new Siding
            {
                Title = TitleTextBox.Text,
                Description = DescriptionTextBox.Text,
                Price = decimal.Parse(PriceTextBox.Text),
                Image = ConvertToBitmapImage(_imageData)
            }).Entity;
            _siningList.Add(_siningId);
            await context.SaveChangesAsync();
            
            
            _featurmMaterialWidth = context.FeaturesMaterials.Add(new FeaturesMaterial
            {
                SidingId = _siningId.SidingId,
                FeaturesId = 1,
                Value = WidthTextBox.Text
                
            }).Entity;
            
            _featurmMaterialLength = context.FeaturesMaterials.Add(new FeaturesMaterial
            {
                SidingId = _siningId.SidingId,
                FeaturesId = 2,
                Value = LengthTextBox.Text
            }).Entity;
            
            _featurmMaterialColor = context.FeaturesMaterials.Add(new FeaturesMaterial
            {
                SidingId = _siningId.SidingId,
                FeaturesId = 4,
                Value = ColorTextBox.Text
                
            }).Entity;
            
            _featuresMaterials.Add(_featurmMaterialLength);
            _featuresMaterials.Add(_featurmMaterialWidth);
            _featuresMaterials.Add(_featurmMaterialColor);
            
            await context.SaveChangesAsync();
            MessageBox.Show("Добавление произведено");
        }
        catch (Exception exception)
        {
            MessageBox.Show("Непредвиденная ошибка" + exception);
        }
    }

    private void BrowseImage_OnClick(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog();
        dlg.DefaultExt = ".jpg";
        dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

        if (dlg.ShowDialog() == true)
        {
            var imagePath = dlg.FileName;
            var bitmap = new BitmapImage(new Uri(imagePath));
            SelectedImage.Source = bitmap;

            _imageData = File.ReadAllBytes(imagePath);
        }
    }
}