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
        _worker = worker;
    }

    private void CalculationsList_OnLoaded(object sender, RoutedEventArgs e)
    {
        /*calculationsList.ItemsSource = LoadListCalculation();*/
    }

    /*private List<Worker> LoadListCalculation()
    {
        MyDbContext context = new MyDbContext();
        try
        {
            var listCalculation = context.Calculations
                .Include(p => p.MaterialsCalculations)
                .Include(p => p.Walls)
                .Include(p => p.Windows)
                .Select(p => new
                {
                    p.CalculationId,
                    p.Title,
                    p.DateOrder
                }).ToListAsync();

        }
        catch (Exception e)
        {
            MessageBox.Show(""+e);
        }
    }*/
}