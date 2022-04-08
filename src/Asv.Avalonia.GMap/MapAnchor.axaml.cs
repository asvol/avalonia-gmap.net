using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.ReactiveUI;
using Material.Icons;

namespace Asv.Avalonia.GMap
{
    public class MapAnchor : TemplatedControl
    {
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



    }
}
