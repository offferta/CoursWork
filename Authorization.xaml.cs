using System;
using System.Threading.Tasks;
using System.Windows;
using Coursework.Context;
using Microsoft.EntityFrameworkCore;

namespace Coursework;

public partial class Authorization : Window
{
    public Authorization()
    {
        InitializeComponent();
    }

   private async void AuthorizatiorButton_OnClick(object sender, RoutedEventArgs e)
    {
        string username = LoginTextBox.Text;
        string password = PasswordTextBox.Password;
            
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Пожалуйста, введите имя пользователя и пароль.", "Ошибка Авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (username != LoginTextBox.Text || password != PasswordTextBox.Password)
        {
            MessageBox.Show("Имя пользователя и пароль не содержат пробелы в начале или конце.", "Ошибка Авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        try
        {
            await using var context = new MyDbContext();
            var worker = await context.Workers
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e =>
                    e.Login == LoginTextBox.Text &&
                    e.Password == PasswordTextBox.Password);
            if (worker is null)
                throw null!;

            if (worker.RoleId == 1)
            {
               // MessageBox.Show("Авториазция прошла успешно!", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                //new MenedgerWindow().Show();
                new AdministrationWindow(worker).Show();
                Close();
            }

            if (worker.RoleId == 2)
            {
               // MessageBox.Show("Авториазция прошла успешно!", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                new MenedgerWindow(worker).Show();
                Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Провертье логин и пароль" + ex, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}