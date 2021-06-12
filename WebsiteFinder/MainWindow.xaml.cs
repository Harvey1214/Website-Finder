using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Microsoft.Win32;

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
            websitesDataGrid.ItemsSource = null;

            if (keywordsTextBox.Text.Length == 0)
            {
                MessageBox.Show("Please fill out keywords to search with", "Missing Keywords", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ActionsManager.KeyWords = keywordsTextBox.Text;
            ActionsManager.MinDate = minDateTextBox.Text;
            ActionsManager.MaxDate = maxDateTextBox.Text;

            ActionsManager.Pages = Convert.ToInt32(pagesNumericUpDown.Value);
            if (ActionsManager.Pages == 0) ActionsManager.Pages = 1;

            // progress
            progressBar.Value = 15;
            progressRing.IsActive = true;

            bool headless = !showWindowCheckBox.IsChecked.Value;

            await Task.Run(() => ActionsManager.StartProcess(true, headless));
            websitesDataGrid.ItemsSource = ActionsManager.Websites;

            // progress
            progressBar.Value = 70;

            LoadContactInfo();
        }

        private void startFromMailingList_Click(object sender, RoutedEventArgs e)
        {
            websitesDataGrid.ItemsSource = null;

            // progress
            progressRing.IsActive = true;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ActionsManager.Websites = DomainsFromFile.Get(openFileDialog.FileName);
            }

            // progress
            progressBar.Value = 15;

            LoadContactInfo();
        }

        private async void LoadContactInfo()
        {
            await Task.Run(() => new ContactInfoFinder().GetEmails(ActionsManager.Websites));

            websitesDataGrid.ItemsSource = null;
            websitesDataGrid.ItemsSource = ActionsManager.Websites;

            // progress
            progressBar.Value = 100;
            progressRing.IsActive = false;

            // open a resizable window with results
            Global.Websites = ActionsManager.Websites;
            ResultsWindow resultsWindow = new ResultsWindow();
            resultsWindow.Show();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ActionsManager.EndProcess();
        }

        private void sendEmailsButton_Click(object sender, RoutedEventArgs e)
        {
            string subject = subjectTextBox.Text;
            string body = bodyTextBox.Text;

            string fromAddress = emailTextBox.Text;
            string password = passwordBox.Password;

            if (fromAddress.Length == 0)
            {
                MessageBox.Show("Gmail account has to be specified", "Email Address Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (password.Length == 0)
            {
                MessageBox.Show("A password to the Gmail account has to be specified", "Password Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (Website website in ActionsManager.Websites)
            {
                if (website.Emails.Count == 0) continue;

                Email email = new()
                {
                    Subject = subject,
                    Body = body,
                    FromAddress = fromAddress,
                    Password = password,
                    ToAddresses = website.Emails
                };

                email.Send();
            }
        }

        private void exportToCSV_Click(object sender, RoutedEventArgs e)
        {
            List<string> lines = new();

            lines.Add("website link, contact email"); // header line

            foreach (Website website in ActionsManager.Websites)
            {
                string line = $"{website.Link},{website.ConcatenatedEmails}";
                lines.Add(line);
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Filter = "CSV file(*.csv)| *.csv | All Files(*.*) | *.* ";
            saveFileDialog.Title = "Export Scraped Data";

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllLines(saveFileDialog.FileName, lines.ToArray());
            }
        }

        private void madeBy_Click(object sender, RoutedEventArgs e)
        {
            var uri = "https://www.fiverr.com/proprogrammer_";
            var psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = uri;
            Process.Start(psi);
        }
    }
}
