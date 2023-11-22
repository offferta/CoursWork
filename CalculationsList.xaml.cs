using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Coursework.Context;
using Coursework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coursework;

public partial class CalculationsList : Page
{
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

    private async void   LoadListCalculation()
    {
        MyDbContext context = new MyDbContext();
        try
        {
            var listCalculation =  context.Calculations
                .Join(
                    context.Walls,
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
        catch (Exception e)
        {
            MessageBox.Show(""+e);
        }
    }

    private void SerchTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        MyDbContext context = new MyDbContext();
        try
        {
            var listCalculation =  context.Calculations
                .Join(
                    context.Walls,
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
            
            var searchText = SerchTextBox.Text;
            
            var filteredData = listCalculation.Where(item =>
                (string.IsNullOrWhiteSpace(searchText) ||
                 item.name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
            )).ToList();
            Grid.ItemsSource = filteredData;
        }
        catch (Exception exception)
        {
            MessageBox.Show(""+exception);
        }
    }
    
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var dataContext = button?.DataContext;
        if (Grid.SelectedItem != null)
        {
            EditWindow editWindow = new EditWindow(dataContext);
            editWindow.ShowDialog();
        }
        else
        {
            MessageBox.Show("Выберите запись для редактирования.");
        }
    }
}