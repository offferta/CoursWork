using System;
using System.Linq;
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
        UpdateWorkerInformation_OnLoaded();
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

    private async void UpdateWorkerInformation_OnLoaded()
    {
        try
        {
            var workerInformations = await _context.WorkerInformations
                .Include(p => p.Worker)
                .Where(p => p.WorkerId == _worker.WorkerId)
                .Select(p => new
                {
                    LoginTextBox = p.Worker.Login,
                    PasswordTextBox = p.Worker.Password,
                    FirstNameTextBox = p.FirstName,
                    SecondNameTextBox = p.SecondName,
                    LastNameTextBox = p.LastName,
                    EmailTextBox = p.Email,
                    PhoneTextBox = p.Phone
                })
                .FirstOrDefaultAsync();
            LoginTextBox.Text = workerInformations.LoginTextBox.ToString();
            PasswordTextBox.Password = workerInformations.PasswordTextBox.ToString();
            FirstNameTextBox.Text = workerInformations.FirstNameTextBox.ToString();
            SecondNameTextBox.Text = workerInformations.SecondNameTextBox.ToString();
            LastNameTextBox.Text = workerInformations.LastNameTextBox.ToString();
            EmailTextBox.Text = workerInformations.EmailTextBox.ToString();
            PhoneTextBox.Text = workerInformations.PhoneTextBox.ToString();
        }
        catch (Exception exception)
        {
            MessageBox.Show("Ошибка" + exception);
        }
    }
}