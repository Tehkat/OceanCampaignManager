using System.IO;
using System.Windows;
using Microsoft.Win32;
using MapManager.App.Models;
using MapManager.App.Services;

namespace MapManager.App.Views
{
    public partial class IslandWindow : Window
    {
        private readonly DataService _dataService = new();
        private Campaign _campaign;

        public IslandWindow(Campaign campaign)
        {
            InitializeComponent();
            _campaign = campaign;
            CampaignTitle.Text = $"Campaign: {campaign.Name}";
            IslandList.ItemsSource = _campaign.Islands;
        }

        private void AddIsland_Click(object sender, RoutedEventArgs e)
        {
            var name = Microsoft.VisualBasic.Interaction.InputBox("Enter island name:", "New Island");
            if (string.IsNullOrWhiteSpace(name)) return;
            _campaign.Islands.Add(new Island { Name = name });
            _dataService.Save(new System.Collections.Generic.List<Campaign> { _campaign });
            IslandList.Items.Refresh();
        }

        private void UploadMap_Click(object sender, RoutedEventArgs e)
        {
            var island = IslandList.SelectedItem as Island;
            if (island == null)
            {
                MessageBox.Show("Please select an island first.");
                return;
            }

            var dlg = new OpenFileDialog { Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp" };
            if (dlg.ShowDialog() == true)
            {
                var mapsDir = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Maps");
                Directory.CreateDirectory(mapsDir);
                var dest = Path.Combine(mapsDir, Path.GetFileName(dlg.FileName));
                File.Copy(dlg.FileName, dest, true);
                island.MapPath = dest;
                _dataService.Save(new System.Collections.Generic.List<Campaign> { _campaign });
                IslandList.Items.Refresh();
            }
        }

        private void UpdateMap_Click(object sender, RoutedEventArgs e)
        {
            var island = IslandList.SelectedItem as Island;
            if (island == null)
            {
                MessageBox.Show("Please select an island first.");
                return;
            }

            var dlg = new OpenFileDialog { Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp" };
            if (dlg.ShowDialog() == true)
            {
                var mapsDir = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Maps");
                Directory.CreateDirectory(mapsDir);
                var dest = Path.Combine(mapsDir, Path.GetFileName(dlg.FileName));

                File.Copy(dlg.FileName, dest, true); // overwrite
                island.MapPath = dest;
                _dataService.Save(new System.Collections.Generic.List<Campaign> { _campaign });
                IslandList.Items.Refresh();

                MessageBox.Show($"Map for island '{island.Name}' updated successfully.");
            }
        }

        private void PreviewMap_Click(object sender, RoutedEventArgs e)
        {
            var island = IslandList.SelectedItem as Island;
            if (island == null)
            {
                MessageBox.Show("Please select an island first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(island.MapPath) || !File.Exists(island.MapPath))
            {
                MessageBox.Show($"Island '{island.Name}' does not have a map uploaded yet.");
                return;
            }

            var preview = new MapPreviewWindow(island.MapPath);
            preview.ShowDialog();
        }

        private void DeleteIsland_Click(object sender, RoutedEventArgs e)
        {
            var island = IslandList.SelectedItem as Island;
            if (island == null)
            {
                MessageBox.Show("Please select an island first.");
                return;
            }

            if (MessageBox.Show($"Delete island '{island.Name}'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _campaign.Islands.Remove(island);
                _dataService.Save(new System.Collections.Generic.List<Campaign> { _campaign });
                IslandList.Items.Refresh();
            }
        }

        private void BackToCampaigns_Click(object sender, RoutedEventArgs e)
        {
            var campaignWindow = new CampaignWindow();
            campaignWindow.Show();
            this.Close();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            var startWindow = new StartWindow();
            startWindow.Show();
            this.Close();
        }
    }
}
