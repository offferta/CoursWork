using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Coursework.Context;
using Coursework.Entities;
using Coursework.Navigation.AdministrationNavigation;
using Microsoft.EntityFrameworkCore;
using Window = System.Windows.Window;

namespace Coursework;

public partial class MenedgerWindow : Window, INotifyPropertyChanged
{
    private readonly Worker _worker;

    public MenedgerWindow(Worker worker)
    {
        InitializeComponent();
        _worker = worker;
        GetFullName(_worker.WorkerId);
        MenedgerFrame.Navigate(new CalculatePage(_worker));
        Application.Current.MainWindow = this;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async void GetFullName(int id)
    {
        try
        {
            await using var context = new MyDbContext();

            var workerInformations = await context.WorkerInformations
                .Include(p => p.Worker)
                .Where(p => p.WorkerId == _worker.WorkerId)
                .Select(p => new
                {
                    FIO = $"{p.FirstName} {p.LastName}"
                })
                .FirstOrDefaultAsync();

            if (FullNameLabel != null) FullNameLabel.Content = $"{workerInformations.FIO}";
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    private void CalculateButton_OnClick(object sender, RoutedEventArgs e)
    {
        MenedgerFrame.Navigate(new CalculatePage(_worker));
    }

    private void CalculationButton_OnClick(object sender, RoutedEventArgs e)
    {
        MenedgerFrame.Navigate(new CalculationsList(_worker));
    }

    private void ListMaterialsButton_OnClick(object sender, RoutedEventArgs e)
    {
        MenedgerFrame.Content = new MaterialsListPage(_worker);
    }

    private void ListCalculationButton_OnClick(object sender, RoutedEventArgs e)
    {
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void FullNameLabel_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        MenedgerFrame.Content = new UpdateWorkerInformation(_worker);
    }
}