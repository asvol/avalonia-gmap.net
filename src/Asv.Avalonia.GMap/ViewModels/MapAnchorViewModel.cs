using Material.Icons;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.GMap
{
    public class MapAnchorViewModel
    {
        [Reactive]
        public MaterialIconKind Icon { get; set; }
        [Reactive]
        public double RotateCenterX { get; set; }
        [Reactive]
        public double RotateCenterY { get; set; }
        [Reactive]
        public bool IsSelected { get; set; }
        [Reactive]
        public bool IsVisible { get; set; }
        [Reactive]
        public double RotateAngle { get; set; }
        [Reactive]
        public string Title { get; set; }
        [Reactive]
        public PointLatLng Location { get; set; }
        
    }
}