using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Coursework.Context;
using Coursework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coursework;

public partial class MaterialsListPage : Page
{
    private Worker _worker;
    public MaterialsListPage(Worker worker)
    {
        InitializeComponent();
        _worker = worker;
        UpdateList();
    }
    private void UpdateList()
    {
        using var context = new MyDbContext();
        var b = new BitmapImage(new Uri($"{Environment.CurrentDirectory}\\picture.png"));
        var project = context.Sidings
            .Select(p => new
            {
                p.Title,
                p.Description,
                p.Price,
                Image = p.Image ?? b
            })
            .ToList();
        
        lvDataBinding.ItemsSource = project;
    }
    private async void UpdateListViewData()
    {
        using var context = new MyDbContext();
        var b = new BitmapImage(new Uri($"{Environment.CurrentDirectory}\\picture.png"));
        var project = context.Sidings
            .Select(p => new
            {
                p.Title,
                p.Description,
                p.Price,
                Image = p.Image ?? b
            })
            .ToList();
        
        lvDataBinding.ItemsSource = project;
        
        string searchText = SerchTextBox.Text;
        string selectedType = ComboType.SelectedItem as string;

        var filteredData = project.Where(item =>
            (string.IsNullOrWhiteSpace(searchText) || item.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) || item.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)) &&
            (selectedType == null || item.Title == selectedType || item.Description == selectedType)
        ).ToList();

        lvDataBinding.ItemsSource = filteredData;
    }
    
    

    private void SerchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateListViewData();
    }

    private void ComboType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
    }
}