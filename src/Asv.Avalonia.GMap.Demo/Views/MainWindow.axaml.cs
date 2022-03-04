using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GMap.NET;

namespace Asv.Avalonia.GMap.Demo.Views
{
    public partial class MainWindow : Window
    {
        public GMapControl MainMap { get; }

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            GoogleMapProvider.Instance.ApiKey = "AIzaSyAmO6pIPTz0Lt8lmYZEIAaixitKjq-4WlB";

            MainMap = this.Get<GMapControl>("GMap");
            MainMap.MapProvider = GMapProviders.YandexSatelliteMap;
            MainMap.Position = new PointLatLng(55.1644, 61.4368);
            MainMap.FillEmptyTiles = true;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.Get<GMapControl>("GMap").Dispose();
        }
    }
}
