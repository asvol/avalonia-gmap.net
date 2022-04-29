using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using DynamicData.Binding;
using JetBrains.Annotations;
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
        private bool _isDrugged;

        public MapViewItem()
        {
            this.WhenActivated(disp =>
            {
                DisposableMixins.DisposeWith(this.WhenAnyValue(_ => _.IsSelected).Subscribe(UpdateSelectableZindex), disp);
                DisposableMixins.DisposeWith(this.WhenAnyValue(_ => _.Bounds).Subscribe(_ => UpdateLocalPosition()), disp);

                DisposableMixins.DisposeWith(this.Events().PointerPressed.Where(_ => IsEditable).Subscribe(DragPointerPressed),disp);
                DisposableMixins.DisposeWith(this.Events().PointerReleased.Where(_ => IsEditable).Subscribe(DragPointerReleased), disp);
                DisposableMixins.DisposeWith(this.Events().PointerMoved.Where(_ => IsEditable && _isDrugged).Subscribe(DragPointerMoved), disp);
                
            });

        }

        public bool IsEditable
        {
            get => _isEditable;
            set => _isEditable = value;
        }

        private void DragPointerMoved(PointerEventArgs args)
        {

            if (_isDrugged)
            {
                if (_map == null) return;

                var child = LogicalChildren.FirstOrDefault() as Visual;
                if (child == null) return;

                var point = args.GetCurrentPoint(_map.MapCanvas);
                var offsetX = 0;
                var offsetY = 0;
               
                var location = _map.FromLocalToLatLng((int)(point.Position.X  + _map.MapTranslateTransform.X + offsetX), (int)(point.Position.Y + _map.MapTranslateTransform.Y + offsetY));
                MapView.SetLocation(child, location); 
            }
        }
        private void DragPointerPressed(PointerPressedEventArgs args)
        {
            if ((args.KeyModifiers & KeyModifiers.Control) != 0)
            {
                _isDrugged = true;
                args.Handled = true;
            }
        }
        private void DragPointerReleased(PointerReleasedEventArgs args)
        {
            _isDrugged = false;
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

        protected override void LogicalChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.LogicalChildrenCollectionChanged(sender, e);
            if (LogicalChildren.FirstOrDefault() is not Visual child) return;
            ZIndex = MapView.GetZOrder(child);
            IsEditable = MapView.GetIsEditable(child);
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


        private IDisposable _collectionSubscribe;
        private bool _firstCall = true;

        public void UpdatePathCollection()
        {
            _collectionSubscribe?.Dispose();
            if (LogicalChildren.FirstOrDefault() is not Visual child) return;
            var pathPoints = MapView.GetPath(child);
            if (pathPoints is INotifyCollectionChanged coll)
            {
                _collectionSubscribe = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    _ => coll.CollectionChanged += _, _ => coll.CollectionChanged -= _).ObserveOn(RxApp.MainThreadScheduler).Subscribe(_=>UpdateLocalPosition());
            }
        }

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnAttachedToLogicalTree(e);
            UpdateLocalPosition();
        }

        public void UpdateLocalPosition()
        {
            if (_map == null) return;

            var child = LogicalChildren.FirstOrDefault() as Visual;
            if (child == null) return;

            var pathPoints = MapView.GetPath(child);
            if (pathPoints is { Count: > 1 })
            {
                
                IsShapeNotAvailable = false;// this is for hide content and draw only path
                if (_firstCall)
                {
                    _firstCall = false;
                    UpdatePathCollection();
                }
                var localPath = new List<GPoint>(pathPoints.Count);
                foreach (var p in pathPoints.ToArray())
                {
                    var itemPoint = _map.FromLatLngToLocal(p);
                    itemPoint.Offset(-(long)(_map.MapTranslateTransform.X), -(long)(_map.MapTranslateTransform.Y));
                    localPath.Add(itemPoint);
                }

                var minX = localPath.Min(_ => _.X);
                var minY = localPath.Min(_ => _.Y);
                Canvas.SetLeft(this, minX);
                Canvas.SetTop(this, minY);
                var truePath = new List<Point>(pathPoints.Count);
                foreach (var p in localPath)
                {
                    p.Offset(-minX,-minY);
                    truePath.Add(new Point(p.X, p.Y));
                }
                
                Shape = CreatePath(truePath, MapView.GetStroke(child), MapView.GetFill(child),MapView.GetStrokeThickness(child),MapView.GetStrokeDashArray(child),MapView.GetPathOpacity(child));
            }
            else
            {
                IsShapeNotAvailable = true;
                var location = MapView.GetLocation(child);
                var point = _map.FromLatLngToLocal(location);
                var offsetX = MapView.GetOffsetX(child);
                var offsetY = MapView.GetOffsetY(child);
                if (double.IsNaN(offsetX))
                {
                    offsetX = Bounds.Width / 2.0;
                }
                if (double.IsNaN(offsetY))
                {
                    offsetY = Bounds.Height / 2.0;
                }
                point.Offset(-(long)(_map.MapTranslateTransform.X + offsetX),
                    -(long)(_map.MapTranslateTransform.Y+ offsetY));
                Canvas.SetLeft(this, point.X);
                Canvas.SetTop(this, point.Y);
            }
        }

        public static readonly DirectProperty<MapViewItem, bool> IsShapeNotAvailableProperty =
            AvaloniaProperty.RegisterDirect<MapViewItem, bool>(nameof(IsShapeNotAvailable), o => o.IsShapeNotAvailable);
        private bool _isShapeNotAvailable = true;
        private bool _isEditable;

        public bool IsShapeNotAvailable
        {
            get => _isShapeNotAvailable;
            private set => SetAndRaise(IsShapeNotAvailableProperty, ref _isShapeNotAvailable, value);
        }

        public static readonly StyledProperty<Path> ShapeProperty = AvaloniaProperty.Register<MapViewItem, Path>(nameof(Shape));
        public Path Shape
        {
            get => GetValue(ShapeProperty);
            set => SetValue(ShapeProperty, value);
        }

        public static Path CreatePath(List<Point> localPath, IBrush stroke, IBrush fill,double thickness, AvaloniaList<double> dash, double opacity )
        {
            // Create a StreamGeometry to use to specify myPath.
            var geometry = new StreamGeometry();
            geometry.BeginBatchUpdate();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(localPath[0], false);
                // Draw a line to the next specified point.
                foreach (var path in localPath)
                {
                    ctx.LineTo(path);
                }
                //ctx.PolyLineTo(localPath, true, true);
            }

            // Freeze the geometry (make it unmodifiable)
            // for additional performance benefits.
            //geometry.Freeze();
            geometry.EndBatchUpdate();
            // Create a path to draw a geometry with.
            var myPath = new Path();
            {
                // Specify the shape of the Path using the StreamGeometry.
                myPath.Data = geometry;
                myPath.Stroke = stroke;
                myPath.StrokeThickness = thickness;
                myPath.StrokeDashArray = dash;
                myPath.StrokeJoin = PenLineJoin.Round;
                myPath.StrokeLineCap = PenLineCap.Square;
                myPath.Fill = fill;
                myPath.Opacity = opacity;
                myPath.IsHitTestVisible = false;
                
            }
            return myPath;
        }


        public static readonly StyledProperty<bool> IsSelectedProperty =
            AvaloniaProperty.Register<MapViewItem, bool>(nameof(IsSelected));

        

        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        
    }
}
