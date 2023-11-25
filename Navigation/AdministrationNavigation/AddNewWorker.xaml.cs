using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        try
        {
            var newWorker = new Worker()
            {
                Login = LoginTextBox.Text,
                Password = PasswordTextBox.Text,
                RoleId = selectedRoleId
            };
            _context.Workers.Add(newWorker);
            await _context.SaveChangesAsync();
            
            var updateWorkerInformation = new WorkerInformation()
            {
                WorkerId = newWorker.WorkerId,
                FirstName = FirstNameTextBox.Text,
                SecondName = SecondNameTextBox.Text,
                LastName = LastNameTextBox.Text
            };
            
            _context.WorkerInformations.Add(updateWorkerInformation);
            await _context.SaveChangesAsync();
            MessageBox.Show("Пользователь успешно добавлен.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при добавлении пользователя: {ex.Message}");
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
}