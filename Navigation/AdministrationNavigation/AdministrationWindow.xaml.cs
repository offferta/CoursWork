using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Coursework.AdministrationNavigation;
using Coursework.Context;
using Coursework.Entities;
using Coursework.Navigation.AdministrationNavigation;
using Microsoft.EntityFrameworkCore;
using Window = System.Windows.Window;

namespace Coursework;

public partial class AdministrationWindow : Window, INotifyPropertyChanged
{
    private readonly Worker _worker;
    private readonly MyDbContext _context = new MyDbContext();

    public AdministrationWindow(Worker worker)
    {
        InitializeComponent();
        _worker = worker;
        GetFullName(_worker.WorkerId);
        AdministrationFrame.Navigate(new AddSidingPage(_worker));
        Application.Current.MainWindow = this;
    }
    
    private void AddSiningButton_OnClick(object sender, RoutedEventArgs e)
    {
        AdministrationFrame.Navigate(new AddSidingPage(_worker));
    }

    private void AddNewWorkerButton_OnClick(object sender, RoutedEventArgs e)
    {
        AdministrationFrame.Navigate(new AddNewWorker(_worker));
    }

    public async void GetFullName(int id)
    {
        try
        {
            var workerInformations = await _context.WorkerInformations
                .Include(p => p.Worker)
                .Where(p => p.WorkerId == _worker.WorkerId)
                .Select(p => new
                {
                    FIO = $"{p.FirstName} {p.LastName} "
                })
                .FirstOrDefaultAsync();

            if (FullNameLabel != null)
            {
                FullNameLabel.Content = $"Администратор";
            }
            else
            {
                FullNameLabel.Content = $"";
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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
        AdministrationFrame.Navigate(new UpdateWorkerInformation(_worker));
        
    }

    private void CalculationsList_OnClick(object sender, RoutedEventArgs e)
    {
        AdministrationFrame.Navigate(new CalculationsList(_worker));
    }

    private void ListWorkerButton_OnClick(object sender, RoutedEventArgs e)
    {
        AdministrationFrame.Navigate(new ListWorker(_worker));
    }
}