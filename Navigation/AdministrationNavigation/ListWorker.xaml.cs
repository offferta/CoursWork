using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Coursework.Context;
using Coursework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coursework.Navigation.AdministrationNavigation;

public partial class ListWorker : Page
{
    private Worker _worker;
    public readonly MyDbContext _context = new MyDbContext();

    public ListWorker(Worker worker)
    {
        InitializeComponent();
        _worker = worker;
        LoadWorker();
    }

    void LoadWorker()
    {
        GetListWorkers();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var dataContext = button?.DataContext;
        
        if (WorkerDataGrid.SelectedItem != null)
        {
            var workerUpdate = new WorkerUpdate(dataContext);
            workerUpdate.Show();
            workerUpdate.Closing += WorkerUpdateOnClosing;
        }
    }

    private void WorkerUpdateOnClosing(object? sender, CancelEventArgs e)
    {
        GetListWorkers();
    }

    private void GetListWorkers()
    {
        var listWorker = _context.WorkerInformations
            .Include(p => p.Worker)
            .Select(p => new
            {
                id = p.WorkerId,
                firstName = p.FirstName,
                lastName = p.LastName,
                secondName = p.SecondName,
                phone = p.Phone,
                email = p.Email,
            }).ToList();
        WorkerDataGrid.ItemsSource = listWorker;
    }
}