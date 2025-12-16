using System.Windows;
using System.Windows.Media.Imaging;

namespace MapManager.App.Views
{
    public partial class MapPreviewWindow : Window
    {
        public MapPreviewWindow(string mapPath)
        {
            InitializeComponent();
            MapImage.Source = new BitmapImage(new System.Uri(mapPath));
        }
    }
}
