using System.Windows;
using Coursework.AdministrationNavigation;
using Coursework.Entities;
using Window = System.Windows.Window;

namespace Coursework;

public partial class AdministrationWindow : Window
{
    private readonly Worker _worker;

    public AdministrationWindow(Worker worker)
    {
        InitializeComponent();
        _worker = worker;
        AdministrationFrame.Navigate(new AddSidingPage(_worker));
    }

    private void AddSiningButton_OnClick(object sender, RoutedEventArgs e)
    {
        AdministrationFrame.Navigate(new AddSidingPage(_worker));
    }
}