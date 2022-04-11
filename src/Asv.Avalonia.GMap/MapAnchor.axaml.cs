using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.ReactiveUI;
using Material.Icons;
using ReactiveUI;

namespace Asv.Avalonia.GMap
{
    public class MapAnchor : TemplatedControl
    {
        public MapAnchor()
        {
            
        }


        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<MapAnchor, string>(nameof(Title));
        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<MaterialIconKind> IconProperty = AvaloniaProperty.Register<MapAnchor, MaterialIconKind>(nameof(Icon));
        public MaterialIconKind Icon
        {
            get => (MaterialIconKind)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<MapAnchor, bool>(nameof(IsSelected));
        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly StyledProperty<double> RotateCenterXProperty = AvaloniaProperty.Register<MapAnchor, double>(nameof(RotateCenterX));
        public double RotateCenterX
        {
            get => GetValue(RotateCenterXProperty);
            set => SetValue(RotateCenterXProperty, value);
        }

        public static readonly StyledProperty<double> RotateCenterYProperty = AvaloniaProperty.Register<MapAnchor, double>(nameof(RotateCenterY));
        public double RotateCenterY
        {
            get => GetValue(RotateCenterYProperty);
            set => SetValue(RotateCenterYProperty, value);
        }

        public static readonly StyledProperty<double> RotateAngleProperty = AvaloniaProperty.Register<MapAnchor, double>(nameof(RotateAngle), defaultValue:300);
        public double RotateAngle
        {
            get => GetValue(RotateAngleProperty);
            set => SetValue(RotateAngleProperty, value);
        }

        public static readonly StyledProperty<double> SizeProperty = AvaloniaProperty.Register<MapAnchor, double>(nameof(Size), defaultValue: 30);
        public double Size
        {
            get => GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public static readonly DirectProperty<MapAnchor, string> DescriptionProperty =
            AvaloniaProperty.RegisterDirect<MapAnchor, string>(nameof(MapAnchor.Description), o => o.Description, (o, v) => o.Description = v);

        private string _description;
        public string Description
        {
            get => _description;
            set => SetAndRaise(DescriptionProperty, ref _description, value);
        }

       
        public static readonly DirectProperty<MapAnchor, IEnumerable> ActionsProperty =
            AvaloniaProperty.RegisterDirect<MapAnchor, IEnumerable>(nameof(Actions), o => o.Actions, (o, v) => o.Actions = v);

        private IEnumerable _actions;
        public IEnumerable Actions
        {
            get => _actions;
            set => SetAndRaise(ActionsProperty, ref _actions, value);
        }
    }
}
