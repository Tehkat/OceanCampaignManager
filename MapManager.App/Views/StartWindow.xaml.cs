using System.Windows;

namespace MapManager.App.Views
{
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var campaignWindow = new CampaignWindow();
            campaignWindow.Show();
            this.Close();
        }
    }
}
