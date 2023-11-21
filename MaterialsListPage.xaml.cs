using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Coursework.Context;
using Coursework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coursework;

public partial class MaterialsListPage : Page, INotifyPropertyChanged
{
    private Worker _worker;
    private Siding _selectedSiding;
    private Visibility _isVisibleSelected = Visibility.Collapsed;
    
    public Siding SelectedSiding
    {
        get => _selectedSiding;
        set
        {
            _selectedSiding = value;
            IsVisibleSelected = Visibility.Visible;
            OnPropertyChanged();
        }
    }
    public Visibility IsVisibleSelected
    {
        get => _isVisibleSelected;
        set
        {
            _isVisibleSelected = value;
            OnPropertyChanged();
        }
    }
    public MaterialsListPage(Worker worker)
    {
        InitializeComponent();
        _worker = worker;
        UpdateList();
        DataContext = this;
    }
    
    private void UpdateList()
    {
        using var context = new MyDbContext();
        var b = new BitmapImage(new Uri($"{Environment.CurrentDirectory}\\picture.png"));
        var project = context.Sidings
            .Select(p => new Siding
            {
                Title = p.Title,
                Description = p.Description,
                Price = p.Price,
                Image = p.Image ?? b
            })
            .ToList();
        
        lvDataBinding2.ItemsSource = project;
    }
    private void UpdateListViewData()
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
        
        lvDataBinding2.ItemsSource = project;
        
        string searchText = SerchTextBox.Text;
        string selectedType = ComboType.SelectedItem as string;

        var filteredData = project.Where(item =>
            (string.IsNullOrWhiteSpace(searchText) || item.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) || item.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)) &&
            (selectedType == null || item.Title == selectedType || item.Description == selectedType)
        ).ToList();

        lvDataBinding2.ItemsSource = filteredData;
    }
    
    

    private void SerchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateListViewData();
    }

    private void ComboType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public ICommand ShowPopupCommand { get; }   
    
}