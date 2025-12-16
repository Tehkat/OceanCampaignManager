using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using MapManager.App.Models;
using MapManager.App.Services;
using MapManager.App.Views;

namespace MapManager.App
{
    public partial class MainWindow : Window
    {
        private readonly DataService _dataService = new();
        private List<Campaign> _campaigns = new();
        private Campaign? _selectedCampaign;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            _campaigns = _dataService.Load();
            CampaignList.ItemsSource = _campaigns;
        }

        private void RefreshUI()
        {
            CampaignList.Items.Refresh();

            if (_selectedCampaign != null)
            {
                SelectedCampaignTitle.Text = _selectedCampaign.Name;
                IslandList.ItemsSource = _selectedCampaign.Islands;
                IslandList.Items.Refresh();
            }
            else
            {
                SelectedCampaignTitle.Text = "No campaign selected";
                IslandList.ItemsSource = null;
            }
        }

        // Button handlers referenced in XAML

        private void NewCampaign_Click(object sender, RoutedEventArgs e)
        {
            var name = Prompt("Enter campaign name:");
            if (string.IsNullOrWhiteSpace(name)) return;

            var c = new Campaign { Name = name };
            _campaigns.Add(c);
            _selectedCampaign = c;
            _dataService.Save(_campaigns);
            RefreshUI();
        }

        private void DeleteCampaign_Click(object sender, RoutedEventArgs e)
        {
            var c = CampaignList.SelectedItem as Campaign;
            if (c == null) return;

            if (MessageBox.Show($"Delete '{c.Name}'?", "Confirm", MessageBoxButton.YesNo)
                == MessageBoxResult.Yes)
            {
                _campaigns.Remove(c);
                if (_selectedCampaign == c) _selectedCampaign = null;
                _dataService.Save(_campaigns);
                RefreshUI();
            }
        }

        private void OpenCampaign_Click(object sender, RoutedEventArgs e)
        {
            _selectedCampaign = CampaignList.SelectedItem as Campaign;
            RefreshUI();
        }

        private void AddIsland_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCampaign == null)
            {
                MessageBox.Show("Select a campaign first.");
                return;
            }

            var name = Prompt("Enter island name:");
            if (string.IsNullOrWhiteSpace(name)) return;

            _selectedCampaign.Islands.Add(new Island { Name = name });
            _dataService.Save(_campaigns);
            RefreshUI();
        }

        private void UploadMap_Click(object sender, RoutedEventArgs e)
        {
            var island = IslandList.SelectedItem as Island;
            if (_selectedCampaign == null || island == null)
            {
                MessageBox.Show("Select a campaign and an island.");
                return;
            }

            var dlg = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp",
                Title = "Select map image"
            };

            if (dlg.ShowDialog() == true)
            {
                var mapsDir = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Maps");
                Directory.CreateDirectory(mapsDir);

                var dest = Path.Combine(mapsDir, Path.GetFileName(dlg.FileName));
                File.Copy(dlg.FileName, dest, true);

                island.MapPath = dest;
                _dataService.Save(_campaigns);
                RefreshUI();
            }
        }

        private void PreviewMap_Click(object sender, RoutedEventArgs e)
        {
            var island = IslandList.SelectedItem as Island;
            if (island == null || string.IsNullOrWhiteSpace(island.MapPath) || !File.Exists(island.MapPath))
            {
                MessageBox.Show("Select an island with an uploaded map.");
                return;
            }

            var viewer = new MapPreviewWindow(island.MapPath);
            viewer.Owner = this;
            viewer.ShowDialog();
        }

        // Simple prompt dialog
        private string? Prompt(string message)
        {
            var dialog = new Window
            {
                Title = message,
                Height = 150,
                Width = 360,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize
            };

            var panel = new System.Windows.Controls.StackPanel { Margin = new Thickness(10) };
            var tb = new System.Windows.Controls.TextBox { Margin = new Thickness(0, 0, 0, 10) };
            var buttons = new System.Windows.Controls.StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var ok = new System.Windows.Controls.Button { Content = "OK", Width = 80, IsDefault = true, Margin = new Thickness(0,0,10,0) };
            var cancel = new System.Windows.Controls.Button { Content = "Cancel", Width = 80, IsCancel = true };

            ok.Click += (_, __) => dialog.DialogResult = true;

            buttons.Children.Add(ok);
            buttons.Children.Add(cancel);
            panel.Children.Add(tb);
            panel.Children.Add(buttons);
            dialog.Content = panel;

            return dialog.ShowDialog() == true ? tb.Text : null;
        }
    }
}
