using System.Windows;
using System.Windows.Controls;

namespace GUIForFTP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppViewModel viewModel;

        public MainWindow()
        {
            viewModel = new AppViewModel("localhost", 1234);
            DataContext = viewModel;
            InitializeComponent();
        }

        private async void HandleServerDoubleClick(object sender, RoutedEventArgs e) =>
            await viewModel.OpenOrDownloadServerItem((sender as ListBoxItem).Content.ToString());

        private void HandleClientDoubleClick(object sender, RoutedEventArgs e) =>
            viewModel.OpenClientFolder((sender as ListBoxItem).Content.ToString());

        private void SelectedItemChanged(object sender, SelectionChangedEventArgs e) => viewModel.SelectedItem = serverList.SelectedItem?.ToString();
    }
}