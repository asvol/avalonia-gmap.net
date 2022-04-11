using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.GMap
{
    [PseudoClasses(":pressed", ":selected")]
    public class AvaloniaMapItem : ContentControl,ISelectable,IDisposable
    {
        private readonly CompositeDisposable _dispose = new();

        static AvaloniaMapItem()
        {
            SelectableMixin.Attach<AvaloniaMapItem>(IsSelectedProperty);
            PressedMixin.Attach<AvaloniaMapItem>();
            FocusableProperty.OverrideDefaultValue<AvaloniaMapItem>(true);
        }


        public AvaloniaMapItem()
        {
            _dispose.Add(this.Events().PointerEnter.Subscribe(_ => ZIndex += 10000));
            _dispose.Add(this.Events().PointerLeave.Subscribe(_ => ZIndex -= 10000));
        }

        #region Drag


        #endregion

        private AvaloniaMap _map;
        public AvaloniaMap Map => _map;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            IControl a = this;
            while (a!=null)
            {
                a = a.Parent;
                if (a is AvaloniaMap map)
                {
                    _map = map;
                    UpdateLocalPosition();
                    break;
                }
            }
        }

        #region LocalPosition X\Y

        public static readonly DirectProperty<AvaloniaMapItem, double> LocalPositionXProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMapItem, double>(nameof(LocalPositionX), o => o.LocalPositionX, (o, v) => o.LocalPositionX = v);
        private double _localPositionX;
        public double LocalPositionX
        {
            get => _localPositionX;
            private set => SetAndRaise(LocalPositionXProperty, ref _localPositionX, value);
        }

        public static readonly DirectProperty<AvaloniaMapItem, double> LocalPositionYProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMapItem, double>(nameof(LocalPositionY), o => o.LocalPositionY, (o, v) => o.LocalPositionY = v);
        private double _localPositionY;
        public double LocalPositionY
        {
            get => _localPositionY;
            private set => SetAndRaise(LocalPositionYProperty, ref _localPositionY, value);
        }

        public void UpdateLocalPosition()
        {
            if (Map == null) return;
            var point = Map.FromLatLngToLocal(Location);
            point.Offset(-(long)Map.MapTranslateTransform.X, -(long)Map.MapTranslateTransform.Y);
            LocalPositionX = (int)(point.X + (long)OffsetX);
            LocalPositionY = (int)(point.Y + (long)OffsetY);
        }

        #endregion

        #region Location

        public static readonly DirectProperty<AvaloniaMapItem, PointLatLng> LocationProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMapItem, PointLatLng>(nameof(Location), o => o.Location, (o, v) => o.Location = v);
        private PointLatLng _location;
        public PointLatLng Location
        {
            get => _location;
            set
            {
                if (SetAndRaise(LocationProperty, ref _location, value))
                {
                    UpdateLocalPosition();
                }
            }
        }

        #endregion

        #region Offset

        public static readonly DirectProperty<AvaloniaMapItem, double> OffsetXProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMapItem, double>(nameof(OffsetX), o => o.OffsetX, (o, v) => o.OffsetX = v);
        private double _offsetX;
        public double OffsetX
        {
            get => _offsetX;
            set
            {
                if (SetAndRaise(OffsetXProperty, ref _offsetX, value))
                {
                    UpdateLocalPosition();
                }
            }
        }

        public static readonly DirectProperty<AvaloniaMapItem, double> OffsetYProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMapItem, double>(nameof(OffsetY), o => o.OffsetY, (o, v) => o.OffsetY = v);
        private double _offsetY;
        public double OffsetY
        {
            get => _offsetY;
            set
            {
                if (SetAndRaise(OffsetYProperty, ref _offsetY, value))
                {
                    UpdateLocalPosition();
                }
            }
        }

        #endregion

        public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<AvaloniaMapItem, bool>(nameof(IsSelected));

        private string _description;

        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public string Description
        {
            get => _description;
            set => SetAndRaise(MapAnchor.DescriptionProperty, ref _description, value);
        }

        public void Dispose()
        {
            _dispose?.Dispose();
        }
    }
}
