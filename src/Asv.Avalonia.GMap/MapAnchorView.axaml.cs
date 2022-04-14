using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Material.Icons;

namespace Asv.Avalonia.GMap
{
    [PseudoClasses(":pressed", ":selected")]
    public class MapAnchorView : TemplatedControl,ISelectable
    {
        public MapAnchorView()
        {
            // SelectableMixin.Attach<MapAnchorView>(IsSelectedProperty);
            // PressedMixin.Attach<MapAnchorView>();
        }

        public static readonly StyledProperty<IBrush?> IconBrushProperty = AvaloniaProperty.Register<MapAnchorView, IBrush?>(nameof(IconBrush));
        public IBrush? IconBrush
        {
            get => GetValue(IconBrushProperty);
            set => SetValue(IconBrushProperty, value);
        }

        public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<MapAnchorView, string>(nameof(Title));
        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<MaterialIconKind> IconProperty = AvaloniaProperty.Register<MapAnchorView, MaterialIconKind>(nameof(Icon));
        public MaterialIconKind Icon
        {
            get => (MaterialIconKind)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<MapAnchorView, bool>(nameof(IsSelected));
        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly StyledProperty<double> RotateCenterXProperty = AvaloniaProperty.Register<MapAnchorView, double>(nameof(RotateCenterX));
        public double RotateCenterX
        {
            get => GetValue(RotateCenterXProperty);
            set => SetValue(RotateCenterXProperty, value);
        }

        public static readonly StyledProperty<double> RotateCenterYProperty = AvaloniaProperty.Register<MapAnchorView, double>(nameof(RotateCenterY));
        public double RotateCenterY
        {
            get => GetValue(RotateCenterYProperty);
            set => SetValue(RotateCenterYProperty, value);
        }

        public static readonly StyledProperty<double> RotateAngleProperty = AvaloniaProperty.Register<MapAnchorView, double>(nameof(RotateAngle), defaultValue:300);
        public double RotateAngle
        {
            get => GetValue(RotateAngleProperty);
            set => SetValue(RotateAngleProperty, value);
        }

        public static readonly StyledProperty<double> SizeProperty = AvaloniaProperty.Register<MapAnchorView, double>(nameof(Size), defaultValue: 30);
        public double Size
        {
            get => GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public static readonly DirectProperty<MapAnchorView, string> DescriptionProperty =
            AvaloniaProperty.RegisterDirect<MapAnchorView, string>(nameof(MapAnchorView.Description), o => o.Description, (o, v) => o.Description = v);

        private string _description;
        public string Description
        {
            get => _description;
            set => SetAndRaise(DescriptionProperty, ref _description, value);
        }

       
        public static readonly DirectProperty<MapAnchorView, IEnumerable> ActionsProperty =
            AvaloniaProperty.RegisterDirect<MapAnchorView, IEnumerable>(nameof(Actions), o => o.Actions, (o, v) => o.Actions = v);

        private IEnumerable _actions;
        public IEnumerable Actions
        {
            get => _actions;
            set => SetAndRaise(ActionsProperty, ref _actions, value);
        }
    }
}
