using System.Collections.ObjectModel;
using Avalonia;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.GMap
{
    public class MapPolygonItem:ReactiveObject
    {
        [Reactive]
        public PointLatLng Location { get; set; }
    }


    public class MapPolygonViewModel : MapShapeViewModel
    {
        private ReadOnlyObservableCollection<MapPolygonItem> _items;
        private readonly ObservableCollection<MapPolygonItem> _editableItems;

        public MapPolygonViewModel()
        {
            ZIndex = -1;
            _editableItems = new ObservableCollection<MapPolygonItem>();
            _items = new ReadOnlyObservableCollection<MapPolygonItem>(_editableItems);
        }

        public override MapShapeType ShapeType => MapShapeType.Polygon;

        public ReadOnlyObservableCollection<MapPolygonItem> Items => _items;


        


    }
}