using System;
using System.Linq;
using System.Windows;
using Coursework.Context;
using Coursework.Entities;
using Window = System.Windows.Window;

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
        if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
            string.IsNullOrWhiteSpace(txtLastName.Text) ||
            string.IsNullOrWhiteSpace(txtSecondName.Text) ||
            string.IsNullOrWhiteSpace(txtPhone.Text) ||
            string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            MessageBox.Show("Заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

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

    private void DeleteButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            int workerId;
            if (!int.TryParse(txtWorkerId.Text, out workerId))
            {
                MessageBox.Show("Не корректный ID сотрудника.");
                return;
            }

            var worker2 = _context.WorkerInformations.FirstOrDefault(w => w.WorkerId == workerId);
            if (worker2 == null)
            {
                MessageBox.Show("Сотрудник с указанным ID не найден.");
                return;
            }
            var worker3 = _context.Calculations.FirstOrDefault(w => w.WorkerId == workerId);
            if (worker3 == null)
            {
                MessageBox.Show("Сотрудник с указанным ID не найден.");
                return;
            }

           
            Worker? worker = _context.Workers.FirstOrDefault(w => w.WorkerId == workerId);
            
            _context.WorkerInformations.Remove(worker2);
            _context.Calculations.Remove(worker3);
            _context.Workers.Remove(worker);
            _context.SaveChanges();
            
            MessageBox.Show("Удаление прошло успешно");
        }
        catch (Exception exception)
        {
            MessageBox.Show($"Ошибка" + exception);
        }
    }
}