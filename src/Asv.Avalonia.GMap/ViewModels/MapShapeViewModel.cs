using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.GMap
{
    public enum MapShapeType
    {
        Anchor,
        Polygon
    }

    public enum OffsetXEnum
    {
        Left,
        Center,
        Right
    }

    public enum OffsetYEnum
    {
        Top,
        Center,
        Bottom
    }

    public abstract class MapShapeViewModel : ReactiveObject
    {
        public abstract MapShapeType ShapeType { get; }
        [Reactive]
        public int ZIndex { get; set; }
        [Reactive]
        public OffsetXEnum OffsetX { get; set; }
        [Reactive]
        public OffsetYEnum OffsetY { get; set; }
        [Reactive]
        public bool IsSelected { get; set; }
        [Reactive]
        public bool IsVisible { get; set; }
        [Reactive]
        public PointLatLng Location { get; set; }
    }
}