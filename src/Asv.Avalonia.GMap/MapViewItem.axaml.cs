using System;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using ReactiveUI;

namespace Asv.Avalonia.GMap
{
    [PseudoClasses(":pressed", ":selected")]
    public class MapViewItem : ContentControl, ISelectable, IActivatableView
    {
        private MapView _map;

        static MapViewItem()
        {
            SelectableMixin.Attach<MapViewItem>(IsSelectedProperty);
            PressedMixin.Attach<MapViewItem>();
            FocusableProperty.OverrideDefaultValue<MapViewItem>(true);
        }

        public MapViewItem()
        {
            this.WhenActivated(disp =>
            {
                DisposableMixins.DisposeWith(this.WhenAnyValue(_ => _.IsSelected).Subscribe(UpdateSelectableZindex), disp);
                DisposableMixins.DisposeWith(this.WhenAnyValue(_ => _.Bounds).Subscribe(_ => UpdateLocalPosition()), disp);
            });
        }

        private void UpdateSelectableZindex(bool isSelected)
        {
            if (LogicalChildren.FirstOrDefault() is ISelectable item)
            {
                item.IsSelected = isSelected;
            }
            if (LogicalChildren.FirstOrDefault() is ISelectable item2)
            {
                item2.IsSelected = isSelected;
            }
            if (isSelected)
            {
                ZIndex += 10000;
            }
            else
            {
                ZIndex -= 10000;
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            IControl a = this;
            while (a != null)
            {
                a = a.Parent;
                if (a is MapView map)
                {
                    _map = map;
                    UpdateLocalPosition();
                    break;
                }
            }
        }

        public void UpdateLocalPosition()
        {
            if (_map == null) return;

            var item = LogicalChildren.FirstOrDefault() as Visual;
            if (item == null) return;

            var location = MapView.GetLocation(item);
            var point = _map.FromLatLngToLocal(location);
            var offsetXType = MapView.GetOffsetX(item);
            var offsetYType = MapView.GetOffsetY(item);
            var offsetX = offsetXType switch
            {
                OffsetXEnum.Left => 0,
                OffsetXEnum.Center => Bounds.Width / 2,
                OffsetXEnum.Right => Bounds.Width,
                _ => throw new ArgumentOutOfRangeException()
            };
            var offsetY = offsetYType switch
            {
                OffsetYEnum.Top => 0,
                OffsetYEnum.Center => Bounds.Height / 2,
                OffsetYEnum.Bottom => Bounds.Height,
                _ => throw new ArgumentOutOfRangeException()
            };
            point.Offset(-(long)(_map.MapTranslateTransform.X + offsetX), -(long)(_map.MapTranslateTransform.Y + offsetY));
            Canvas.SetLeft(this, point.X);
            Canvas.SetTop(this, point.Y);
        }

        public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<AvaloniaMapItem, bool>(nameof(IsSelected));
        

        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

    }
}
