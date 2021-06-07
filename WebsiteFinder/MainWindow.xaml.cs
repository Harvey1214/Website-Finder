﻿using System;
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

            await Task.Run(() => ActionsManager.StartProcess());
            websitesDataGrid.ItemsSource = ActionsManager.Websites;

            // progress
            progressBar.Value = 70;

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
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ActionsManager.EndProcess();
        }
    }
}
