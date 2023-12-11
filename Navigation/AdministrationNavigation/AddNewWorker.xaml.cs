using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Coursework.Context;
using Coursework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coursework.Navigation.AdministrationNavigation;

public partial class AddNewWorker : Page
{
    private Worker _worker;
    private readonly MyDbContext _context = new MyDbContext();
    private int selectedRoleId;

    public AddNewWorker(Worker worker)
    {
        InitializeComponent();
        _worker = worker;
        LoadRoles();
    }

    private async void LoadRoles()
    {
        try
        {
            var roles = await _context.Roles.ToListAsync();
            RoleComboBox.ItemsSource = roles;
            RoleComboBox.DisplayMemberPath = "Name";
            RoleComboBox.SelectedValuePath = "RoleId";
            RoleComboBox.SelectedIndex = 1;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке ролей: {ex.Message}");
        }
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        await AddNewUser();
    }

    private async Task AddNewUser()
    {
        if (string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
            string.IsNullOrWhiteSpace(PasswordTextBox.Password) ||
            string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
            string.IsNullOrWhiteSpace(LastNameTextBox.Text))
        {
            MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
            return;
        }

        var existingWorker = await _context.Workers.FirstOrDefaultAsync(w => w.Login == LoginTextBox.Text);
        var existingPhone = await _context.WorkerInformations.FirstOrDefaultAsync(w => w.Phone == PhoneTextBox.Text);
        var existingEmail = await _context.WorkerInformations.FirstOrDefaultAsync(w => w.Email == EmailTextBox.Text);

        if (existingWorker != null)
        {
            MessageBox.Show("Пользователь с таким логином уже существует.");
            return;
        }

        if (existingPhone != null)
        {
            MessageBox.Show("Пользователь с таким телефоном уже существует.");
            return;
        }

        if (existingEmail != null)
        {
            MessageBox.Show("Пользователь с такой почтой уже существует.");
            return;
        }

        bool isValidEmail = CheckEmail(EmailTextBox.Text);
        bool isValidPhone = CheckPhone(PhoneTextBox.Text);

        if (!isValidEmail || !isValidPhone)
        {
            return;
        }

        try
        {
            var newWorker = new Worker()
            {
                Login = LoginTextBox.Text,
                Password = PasswordTextBox.Password,
                RoleId = selectedRoleId
            };

            var updateWorkerInformation = new WorkerInformation()
            {
                FirstName = FirstNameTextBox.Text,
                SecondName = SecondNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Email = EmailTextBox.Text,
                Phone = PhoneTextBox.Text
            };

            _context.Workers.Add(newWorker);
            await _context.SaveChangesAsync();

            updateWorkerInformation.WorkerId = newWorker.WorkerId;

            _context.WorkerInformations.Add(updateWorkerInformation);
            await _context.SaveChangesAsync();

            MessageBox.Show("Пользователь успешно добавлен.");
        }
        catch (DbUpdateException ex)
        {
            MessageBox.Show($"Ошибка при добавлении пользователя: {ex.InnerException?.Message}");
        }
    }

    private bool CheckEmail(string email)
    {
        string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        bool isValidEmail = Regex.IsMatch(email, emailPattern);

        if (!isValidEmail)
        {
            MessageBox.Show("Некорректный адрес электронной почты.");
            return false;
        }

        return true;
    }
    
    private bool CheckPhone(string phone)
    {
        string phonePattern = @"^\d+$";
        bool isValidPhone = Regex.IsMatch(phone, phonePattern);

        if (!isValidPhone)
        {
            MessageBox.Show("Некорректный номер телефона.");
            return false;
        }
        return true;
    }
    
    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RoleComboBox.SelectedItem != null)
        {
            var selectedRole = (Role)RoleComboBox.SelectedItem;
            selectedRoleId = selectedRole.RoleId;
        }
    }
}