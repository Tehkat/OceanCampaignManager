using System.Collections.Generic;
using System.Windows;
using MapManager.App.Models;
using MapManager.App.Services;

namespace MapManager.App.Views
{
    public partial class CampaignWindow : Window
    {
        private readonly DataService _dataService = new();
        private List<Campaign> _campaigns = new();

        public CampaignWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            _campaigns = _dataService.Load();
            CampaignList.ItemsSource = _campaigns;
        }

        private void NewCampaign_Click(object sender, RoutedEventArgs e)
        {
            var name = Microsoft.VisualBasic.Interaction.InputBox("Enter campaign name:", "New Campaign");
            if (string.IsNullOrWhiteSpace(name)) return;
            _campaigns.Add(new Campaign { Name = name });
            _dataService.Save(_campaigns);
            CampaignList.Items.Refresh();
        }

        private void DeleteCampaign_Click(object sender, RoutedEventArgs e)
        {
            var selected = CampaignList.SelectedItem as Campaign;
            if (selected == null) return;
            _campaigns.Remove(selected);
            _dataService.Save(_campaigns);
            CampaignList.Items.Refresh();
        }

        private void OpenCampaign_Click(object sender, RoutedEventArgs e)
        {
            var selected = CampaignList.SelectedItem as Campaign;
            if (selected == null) return;

            var islandWindow = new IslandWindow(selected);
            islandWindow.Show();
            this.Close();
        }
    }
}
