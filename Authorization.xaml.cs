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
        await Authentication();
    }

    private async Task Authentication() { var username = LoginTextBox.Text; var password = PasswordTextBox.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Пожалуйста, введите имя пользователя и пароль.", "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (username.Trim() != username || password.Trim() != password)
        {
            MessageBox.Show("Имя пользователя и пароль не должны содержать начальных или конечных пробелов.", "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            await using var context = new MyDbContext();
            var worker = await context.Workers
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e =>
                    e.Login == username &&
                    e.Password == password);
    
            if (worker is null)
                throw null!;

            if (worker.RoleId == 1)
            {
                new AdministrationWindow(worker).Show();
                Close();
            }

            if (worker.RoleId == 2)
            {
                new MenedgerWindow(worker).Show();
                Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Пожалуйста, проверьте свое имя пользователя и пароль.", "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}