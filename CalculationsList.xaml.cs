using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Coursework.Context;
using Coursework.Entities;

namespace Coursework;

public partial class CalculationsList : Page
{
    private readonly MyDbContext _context = new();
    private readonly Worker _worker;

    public CalculationsList(Worker worker)
    {
        InitializeComponent();
        LoadListCalculation();
        _worker = worker;
    }

    private void CalculationsList_OnLoaded(object sender, RoutedEventArgs e)
    {
    }

    private async void LoadListCalculation()
    {
        try
        {
            var listCalculation = _context.Calculations
                .Join(
                    _context.Walls,
                    calculation => calculation.CalculationId,
                    wall => wall.CalculationId,
                    (calculation, wall) => new
                    {
                        calculationId = calculation.CalculationId,
                        name = calculation.Title,
                        wall = wall.WallId,
                        count = wall.Count,
                        lenght = wall.Length,
                        wight = wall.Width
                    })
                .ToList();

            Grid.ItemsSource = listCalculation;
        }
        catch (Exception e)
        {
            MessageBox.Show("" + e);
        }
    }

    private void SerchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            var listCalculation = _context.Calculations
                .Join(
                    _context.Walls,
                    calculation => calculation.CalculationId,
                    wall => wall.CalculationId,
                    (calculation, wall) => new
                    {
                        calculationId = calculation.CalculationId,
                        name = calculation.Title,
                        wallId = wall.WallId,
                        lenght = wall.Length,
                        wight = wall.Width,
                        count = wall.Count
                    }
                )
                .ToList();
            Grid.ItemsSource = listCalculation;
            var searchText = SerchTextBox.Text;
            var filteredData = listCalculation.Where(item =>
                string.IsNullOrWhiteSpace(searchText) ||
                item.name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
            Grid.ItemsSource = filteredData;
        }
        catch (Exception exception)
        {
            MessageBox.Show("" + exception);
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var dataContext = button?.DataContext;
        if (Grid.SelectedItem != null)
        {
            var editWindow = new EditWindow(dataContext);
            editWindow.Show();
            editWindow.Closing += EditWindowOnClosing;
        }
        else
        {
            MessageBox.Show("Выберите запись для редактирования.");
        }
    }

    private void EditWindowOnClosing(object? sender, CancelEventArgs e)
    {
        var listCalculation = _context.Calculations
            .Join(
                _context.Walls,
                calculation => calculation.CalculationId,
                wall => wall.CalculationId,
                (calculation, wall) => new
                {
                    calculationId = calculation.CalculationId,
                    name = calculation.Title,
                    wall = wall.WallId,
                    count = wall.Count,
                    lenght = wall.Length,
                    wight = wall.Width
                }
            )
            .ToList();
        Grid.ItemsSource = listCalculation;
    }
}