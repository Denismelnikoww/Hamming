using Avalonia.Controls;
using HammingApp.ViewModels;

namespace HammingApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }
}