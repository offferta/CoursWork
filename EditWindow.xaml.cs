using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualBasic;

namespace Coursework;

public partial class EditWindow : Window, INotifyPropertyChanged
{
    public object _dataC0ontext;
    
    public EditWindow(object dataContext)
    {
        InitializeComponent();
        _dataC0ontext = dataContext;

        DataContext = _dataC0ontext;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        
    }
    public async Task UpdateDataAsync(int calculationId, string name, int wallId, double length, double width, int count)
    {
        
    }
}