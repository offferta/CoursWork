using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Coursework.Context;
using Coursework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Utils.Windows;

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
    
    private bool AreAllRequiredFieldsFilled()
    {
        return !string.IsNullOrWhiteSpace(LoginTextBox.Text) &&
               !string.IsNullOrWhiteSpace(PasswordTextBox.Password) &&
               !string.IsNullOrWhiteSpace(FirstNameTextBox.Text) &&
               !string.IsNullOrWhiteSpace(LastNameTextBox.Text);
    }

    private async Task<bool> IsExistingWorker(string login)
    {
        return await _context.Workers.AnyAsync(w => w.Login == login);
    }


    private bool IsValidEmail(string email)
    {
        string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    private bool IsValidPhone(string phone)
    {
        string phonePattern = @"^\d+$";
        return Regex.IsMatch(phone, phonePattern);
    }
    
    private void ExitProfileButton_OnClick(object sender, RoutedEventArgs e)
    {
        new Authorization().Show();
        Application.Current.MainWindow.Close();
    }

    private string oldEmail;
    private string oldPhone;
    private async void UpdateWorkerInformation_OnLoaded(object sender, RoutedEventArgs e)
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

    private async void UpDateButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!AreAllRequiredFieldsFilled())
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                return;
            }

            bool isLoginUnchanged = IsLoginUnchanged(LoginTextBox.Text); // Проверяем, изменился ли логин

            if (!isLoginUnchanged && await IsExistingWorker(LoginTextBox.Text))
            {
                MessageBox.Show("Пользователь с таким логином уже существует.");
                return;
            }


            if (!IsValidEmail(EmailTextBox.Text) || !IsValidPhone(PhoneTextBox.Text))
            {
                MessageBox.Show("fdf");
                return;
            }

            var updatedWorkerInformation = new WorkerInformation()
            {
                WorkerId = _worker.WorkerId,
                FirstName = FirstNameTextBox.Text,
                SecondName = SecondNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Email = EmailTextBox.Text,
                Phone = PhoneTextBox.Text
            };

            var a = await _context.WorkerInformations.SingleOrDefaultAsync(p => p.WorkerId == _worker.WorkerId);
            
            a.FirstName = FirstNameTextBox.Text;
            a.SecondName = SecondNameTextBox.Text;
            a. LastName = LastNameTextBox.Text;
            a.Email = EmailTextBox.Text;
            a.Phone = PhoneTextBox.Text;
            var b = await _context.Workers.SingleOrDefaultAsync(p => p.WorkerId == _worker.WorkerId);
            b.Login = LoginTextBox.Text;
            b.Password = PasswordTextBox.Password;
            await _context.SaveChangesAsync();
            MessageBox.Show("Данные успешно обновлены!");
        }
        catch (DbUpdateConcurrencyException)
        {
            MessageBox.Show(
                "Ошибка при обновлении данных: другой пользователь мог изменить данные. Пожалуйста, обновите страницу и повторите попытку.");
        }
        catch (DbUpdateException)
        {
            MessageBox.Show("Ошибка при обновлении данных: возможно, нарушено ограничение уникальности.");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Произошла непредвиденная ошибка: " + ex.Message);
        }
    }
    private bool IsLoginUnchanged(string newLogin)
    {
        return newLogin == _worker.Login; // Сравниваем новый логин с текущим в базе данных
    }
}