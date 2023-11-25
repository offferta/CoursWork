using System;
using System.Windows;
using System.Windows.Controls;
using Coursework.Context;
using Coursework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coursework.Navigation.AdministrationNavigation;

public partial class UpdateWorkerInformation : Page
{
    private readonly Worker _worker;
    private readonly MyDbContext _context = new MyDbContext();
    public UpdateWorkerInformation(Worker worker)
    {
        InitializeComponent();
        _worker = worker;
    }

    private async void UpDateButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            var updateInformationWorker = new WorkerInformation()
            {
                WorkerId = _worker.WorkerId,
                FirstName = FirstNameTextBox.Text,
                SecondName = SecondNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Email = EmailTextBox.Text,
                Phone = PhoneTextBox.Text
            };
            _context.WorkerInformations.Update(updateInformationWorker);
            await _context.SaveChangesAsync();
            MessageBox.Show("Данные успешно обновлены!");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            MessageBox.Show("Ошибка при обновлении данных: другой пользователь мог изменить данные. Пожалуйста, обновите страницу и повторите попытку.");
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show("Ошибка при обновлении данных: возможно, нарушено ограничение уникальности.");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Произошла непредвиденная ошибка: " + ex.Message);
        }
    }

}