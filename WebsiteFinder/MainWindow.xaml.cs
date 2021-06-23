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
            ActionsManager.MinPages = Convert.ToInt32(minPagesNumericUpDown.Value);
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
            ContactInfoFinder contactFinder = new() 
            { 
                FilterByFooterDate = filterByFooterDateCheckBox.IsChecked.Value,
                MinDate = minDateTextBox.Text,
                MaxDate = maxDateTextBox.Text
            };
            ActionsManager.Websites = await Task.Run(() => contactFinder.GetEmails(ActionsManager.Websites));

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

            lines.Add("website link, contact email, page, footer date"); // header line

            foreach (Website website in ActionsManager.Websites)
            {
                string line = $"{website.Link},{website.GetEmailsDividedBySemicolons()},{website.Page},{website.FooterDate}";
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


/*
 * <Grid ShowGridLines="True">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="auto" MaxWidth="350px" />
            <ColumnDefinition Width="auto" MaxWidth="100px"/>
            <ColumnDefinition Width="auto" />
            <ColumnDefinition Width="auto" />
            <ColumnDefinition Width="auto" />
            <ColumnDefinition Width="auto" />
        </Grid.ColumnDefinitions>
        <Grid.RowDefinitions>
            <RowDefinition Height="auto" />
            <RowDefinition Height="auto" />
            <RowDefinition Height="auto" />
            <RowDefinition Height="auto" />
            <RowDefinition Height="auto" />
            <RowDefinition Height="auto" />
        </Grid.RowDefinitions>

        <TextBox Grid.Row="0" Grid.Column="0" mah:TextBoxHelper.Watermark="Keywords (space separated)" x:Name="keywordsTextBox" HorizontalAlignment="Left" Margin="10,10,0,0" TextWrapping="Wrap" VerticalAlignment="Top" Width="289" Height="26"/>
        <mah:NumericUpDown Grid.Row="0" Grid.Column="1" TextAlignment="Left" mah:TextBoxHelper.Watermark="From Page" x:Name="minPagesNumericUpDown" HorizontalAlignment="Left" Margin="304,10,0,0" VerticalAlignment="Top" Width="118" Height="26"/>
        <mah:NumericUpDown Grid.Row="0" Grid.Column="2" TextAlignment="Left" mah:TextBoxHelper.Watermark="Pages" x:Name="pagesNumericUpDown" HorizontalAlignment="Left" Margin="427,10,0,0" VerticalAlignment="Top" Width="90" Height="26"/>

        <Button Grid.Row="1" Grid.Column="0" x:Name="startButton" Style="{StaticResource MahApps.Styles.Button.Flat}" Content="Search" HorizontalAlignment="Left" Margin="10,41,0,0" VerticalAlignment="Top" Click="startButton_Click" Width="158" Height="29"/>
        <Button Grid.Row="1" Grid.Column="1" x:Name="startFromMailingList" Style="{StaticResource MahApps.Styles.Button.Flat}" Content="Mailing List" HorizontalAlignment="Left" Margin="173,41,0,0" VerticalAlignment="Top" Width="168" Click="startFromMailingList_Click" Height="29"/>
        <TextBox Grid.Row="1" Grid.Column="2" mah:TextBoxHelper.Watermark="Min Age" x:Name="minDateTextBox" HorizontalAlignment="Left" Margin="346,44,0,0" TextWrapping="Wrap" VerticalAlignment="Top" Width="82" Height="26"/>
        <TextBox Grid.Row="1" Grid.Column="3" mah:TextBoxHelper.Watermark="Max Age" x:Name="maxDateTextBox" HorizontalAlignment="Left" Margin="434,44,0,0" TextWrapping="Wrap" VerticalAlignment="Top" Width="83" Height="26"/>

        <mah:MetroProgressBar Grid.Row="2" Grid.Column="0" x:Name="progressBar" HorizontalAlignment="Center" Height="27" Margin="0,75,0,0" VerticalAlignment="Top" Width="507"/>
        <mah:ProgressRing Grid.Row="2" Grid.Column="0" x:Name="progressRing" IsActive="False" HorizontalAlignment="Left" Margin="485,72,0,0" VerticalAlignment="Top" Height="32" Width="32"/>
        
        <DataGrid Grid.Row="3" Grid.Column="0" IsReadOnly="True" x:Name="websitesDataGrid" Margin="10,107,10,182"/>

        <CheckBox Grid.Row="4" Grid.Column="0" x:Name="showWindowCheckBox" Content="Show Browser Window" HorizontalAlignment="Left" Margin="10,361,0,0" VerticalAlignment="Top" Height="18" Width="145"/>
        <CheckBox Grid.Row="4" Grid.Column="0" x:Name="filterByFooterDateCheckBox" Content="Filter by Footer Date (may eradicate a lot of results)" HorizontalAlignment="Left" Margin="11,384,0,0" VerticalAlignment="Top" Width="313" Height="18"/>
        <Button Grid.Row="4" Grid.Column="1" Style="{StaticResource MahApps.Styles.Button.Flat}" x:Name="exportToCSV" Content="Export to CSV" HorizontalAlignment="Left" Margin="414,356,0,0" VerticalAlignment="Top" Click="exportToCSV_Click" Grid.ColumnSpan="3" Height="29" Width="105"/>

        <TextBox Grid.Row="5" Grid.Column="0" mah:TextBoxHelper.Watermark="Gmail Address" x:Name="emailTextBox" HorizontalAlignment="Left" Margin="4,407,0,0" TextWrapping="Wrap" VerticalAlignment="Top" Width="254" Height="26"/>
        <PasswordBox Grid.Row="5" Grid.Column="1" mah:TextBoxHelper.Watermark="Password" x:Name="passwordBox" HorizontalAlignment="Left" Margin="264,407,0,0" VerticalAlignment="Top" Width="246" Height="26"/>

        <TextBox Grid.Row="6" Grid.Column="0" mah:TextBoxHelper.Watermark="Subject" x:Name="subjectTextBox" HorizontalAlignment="Left" Margin="4,438,0,0" TextWrapping="Wrap" VerticalAlignment="Top" Width="126" Height="26"/>
        <TextBox Grid.Row="6" Grid.Column="1" mah:TextBoxHelper.Watermark="Body" x:Name="bodyTextBox" HorizontalAlignment="Left" Margin="138,438,0,0" TextWrapping="Wrap" VerticalAlignment="Top" Width="372" Height="65"/>
        <Button Grid.Row="6" Grid.Column="0" x:Name="sendEmailsButton" Style="{StaticResource MahApps.Styles.Button.Flat}" Content="Email All" HorizontalAlignment="Left" Margin="4,469,0,0" VerticalAlignment="Top" Click="sendEmailsButton_Click" Width="126" Height="34"/>
        
        
    </Grid>
*/