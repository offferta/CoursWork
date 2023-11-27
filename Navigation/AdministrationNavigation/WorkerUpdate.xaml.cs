using System;
using System.Linq;
using System.Windows;
using Coursework.Context;

namespace Coursework.Navigation.AdministrationNavigation;

public partial class WorkerUpdate : Window
{
    private readonly MyDbContext _context = new MyDbContext();
    public object _dataC0ontext;
    public WorkerUpdate(object dataContext)
    {
        InitializeComponent();
        _dataC0ontext = dataContext;

        DataContext = _dataC0ontext;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateInformationOnWorker();
    }

    void UpdateInformationOnWorker()
    {
        int workerId;
        if (!int.TryParse(txtWorkerId.Text, out workerId))
        {
            MessageBox.Show("Не корректный ID сотрудника.");
            return;
        }
        var worker = _context.WorkerInformations.FirstOrDefault(w => w.WorkerId == workerId);
        if (worker == null)
        {
            MessageBox.Show("Сотрудник с указанным ID не найден.");
            return;
        }
        worker.FirstName = txtFirstName.Text;
        worker.LastName = txtLastName.Text;
        worker.SecondName = txtSecondName.Text;
        worker.Email = txtEmail.Text;
        worker.Phone = txtPhone.Text;
        try
        {
            _context.SaveChanges();
            MessageBox.Show("Информация о сотруднике успешно обновлена.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
        }
        
    }
}