using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MapManager.App.Models;
using MapManager.App.Services;

namespace MapManager.App.Views
{
    public partial class CampaignWindow : Window
    {
        private readonly DataService _dataService = new();
        private List<Campaign> _campaigns;

        public CampaignWindow()
        {
            InitializeComponent();
            _campaigns = _dataService.Load();
            CampaignList.ItemsSource = _campaigns;
        }

        private void NewCampaign_Click(object sender, RoutedEventArgs e)
        {
            var name = Microsoft.VisualBasic.Interaction.InputBox("Enter campaign name:", "New Campaign");
            if (string.IsNullOrWhiteSpace(name)) return;

            // Reload campaigns to ensure latest data
            _campaigns = _dataService.Load();

            // Prevent duplicate campaign names (case-insensitive)
            if (_campaigns.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show($"A campaign named '{name}' already exists.");
                return;
            }

            _campaigns.Add(new Campaign { Name = name });
            _dataService.Save(_campaigns);
            CampaignList.ItemsSource = _campaigns;
            CampaignList.Items.Refresh();
        }

        private void OpenCampaign_Click(object sender, RoutedEventArgs e)
        {
            var campaign = CampaignList.SelectedItem as Campaign;
            if (campaign == null)
            {
                MessageBox.Show("Please select a campaign first.");
                return;
            }

            var islandWindow = new IslandWindow(campaign);
            islandWindow.Show();
            this.Close();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            var startWindow = new StartWindow();
            startWindow.Show();
            this.Close();
        }

        
        private void DeleteCampaign_Click(object sender, RoutedEventArgs e)
        {
            var campaign = CampaignList.SelectedItem as Campaign;
            if (campaign == null)
            {
                MessageBox.Show("Please select a campaign first.");
                return;
            }

            if (MessageBox.Show($"Delete campaign '{campaign.Name}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _campaigns.Remove(campaign);
                _dataService.Save(_campaigns);
                CampaignList.Items.Refresh();
            }
        }

        private void BackToStart_Click(object sender, RoutedEventArgs e)
        {
            var startWindow = new StartWindow();
            startWindow.Show();
            this.Close();
        }
    }
}
