using System.Collections.ObjectModel;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.GMap
{

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


    public class MapAnchorViewModel: ReactiveObject
    {
        public MapAnchorViewModel()
        {
            if (Design.IsDesignMode)
            {
                Actions = new ReadOnlyObservableCollection<MapAnchorActionViewModel>(
                    new ObservableCollection<MapAnchorActionViewModel>
                    {
                        new() {Title = "Action1", Icon = MaterialIconKind.Run},
                        new() {Title = "Action2", Icon = MaterialIconKind.Run}
                    });
            }
        }

        [Reactive]
        public int ZOrder { get; set; }
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
        [Reactive]
        public MaterialIconKind Icon { get; set; }
        [Reactive]
        public double RotateCenterX { get; set; }
        [Reactive]
        public double RotateCenterY { get; set; }
        [Reactive] 
        public IBrush IconBrush { get; set; }
        [Reactive]
        public double RotateAngle { get; set; }
        [Reactive]
        public string Title { get; set; }
        [Reactive]
        public string Description { get; set; }
        [Reactive]
        public double Size { get; set; } = 32;
        
        public virtual ReadOnlyObservableCollection<MapAnchorActionViewModel> Actions { get; }
        
        public virtual ReadOnlyObservableCollection<PointLatLng> Path { get; }

        [Reactive]
        public IBrush Fill { get; set; }
        [Reactive]
        public IBrush Stroke { get; set; } = Brushes.Blue;
        [Reactive]
        public double StrokeThickness { get; set; } = 3;
        [Reactive]
        public AvaloniaList<double> StrokeDashArray { get; set; }
        [Reactive]
        public double PathOpacity { get; set; } = 0.6;
        
        
        



    }
}