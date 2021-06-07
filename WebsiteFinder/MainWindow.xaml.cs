using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace WebsiteFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private ActionsManager ActionsManager { get; set; } = new ActionsManager();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (keywordsTextBox.Text.Length == 0)
            {
                MessageBox.Show("Please fill out keywords to search with", "Missing Keywords", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ActionsManager.KeyWords = keywordsTextBox.Text;
            ActionsManager.MinDate = minDateTextBox.Text;
            ActionsManager.MaxDate = maxDateTextBox.Text;

            await Task.Run(() => ActionsManager.StartProcess());
            websitesDataGrid.ItemsSource = ActionsManager.Websites;

            websitesDataGrid.ItemsSource = new ContactInfoFinder().FindEmails(ActionsManager.Websites);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ActionsManager.EndProcess();
        }
    }
}
