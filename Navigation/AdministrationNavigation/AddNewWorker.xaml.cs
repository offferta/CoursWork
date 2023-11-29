using System;
using System.Linq;
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
        if (string.IsNullOrWhiteSpace(LoginTextBox.Text) || string.IsNullOrWhiteSpace(PasswordTextBox.Password) || string.IsNullOrWhiteSpace(FirstNameTextBox.Text) || string.IsNullOrWhiteSpace(LastNameTextBox.Text))
        {
            MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
        }
        else
        {
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
                    LastName = LastNameTextBox.Text
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

    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RoleComboBox.SelectedItem != null)
        {
            var selectedRole = (Role)RoleComboBox.SelectedItem;
            selectedRoleId = selectedRole.RoleId;
        }
    }

    private void PhoneTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        throw new NotImplementedException();
    }
}