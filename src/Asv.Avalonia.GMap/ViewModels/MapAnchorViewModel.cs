using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media;
using Material.Icons;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.GMap
{
    

    public class MapAnchorViewModel: MapShapeViewModel
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

        public override MapShapeType ShapeType => MapShapeType.Anchor;

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
        
    }
}