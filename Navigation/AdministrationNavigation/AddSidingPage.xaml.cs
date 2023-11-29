using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            try
            {
                if (_imageData == null)
                {
                    MessageBox.Show("Загрузите фотографию.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TitleTextBox.Text) ||
                    string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                    string.IsNullOrWhiteSpace(PriceTextBox.Text) || 
                    string.IsNullOrWhiteSpace(ThicknessTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля.");
                    return;
                }

                if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
                {
                    MessageBox.Show("Пожалуйста, введите действительную цену.");
                    return;
                }

                _siningId = context.Sidings.Update(new Siding
                {
                    Title = TitleTextBox.Text,
                    Description = DescriptionTextBox.Text,
                    Price = price,
                    Image = ConvertToBitmapImage(_imageData)
                }).Entity;
    
                _siningList.Add(_siningId);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            
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
    public bool IsNumber(string text)
    {
        return Regex.IsMatch(text, @"^-?\d{0,1}(\.\d{0,2})?$");
    }
    
    private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (!IsNumber(e.Text))
        {
            e.Handled = true;
        }
    }
    private void WidthTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (!IsNumber(e.Text))
        {
            e.Handled = true;
        }
    }
    private void LengthTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (!IsNumber(e.Text))
        {
            e.Handled = true;
        }
    }
    private void ThicknessTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (!IsNumber(e.Text))
        {
            e.Handled = true;
        }
    }
}