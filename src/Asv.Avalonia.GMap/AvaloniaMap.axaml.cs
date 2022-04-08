using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Selection;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using ReactiveUI;

namespace Asv.Avalonia.GMap
{
    public class AvaloniaMap : SelectingItemsControl, IDisposable
    {
        private const double MinimumHorizontalDragDistance = 3;
        private const double MinimumVerticalDragDistance = 3;
        private readonly CompositeDisposable _dispose = new();
        internal readonly TranslateTransform MapTranslateTransform = new();
        internal readonly TranslateTransform MapOverlayTranslateTransform = new();
        internal ScaleTransform MapScaleTransform = new();
        private readonly ScaleTransform _lastScaleTransform = new();
        private readonly MouseDevice _mouse = new();
        private readonly Core _core = new();

        static AvaloniaMap()
        {
            GMapImageProxy.Enable();
        }

        public AvaloniaMap()
        {
            _core.SystemType = "WindowsPresentation";
            _core.RenderMode = RenderMode.WPF;
            Position = new PointLatLng(55.1644, 61.4368);
            MapProvider = GMapProviders.BingHybridMap;
            Zoom = 1;
            MinZoom = _core.MinZoom;
            MaxZoom = _core.MaxZoom;
            _core.OnMapZoomChanged += ForceUpdateOverlays;
            _core.OnCurrentPositionChanged += point => Position = point;
            this.WhenAnyValue(_ => _.Bounds).Subscribe(_ => _core.OnMapSizeChanged((int)_.Width, (int)_.Height));
            
        }

        #region Render

        private void UpdateMarkersOffset()
        {
            if (MapCanvas != null)
            {
                if (MapScaleTransform != null)
                {
                    var (x, y) = MapScaleTransform.Transform(
                        new Point(_core.RenderOffset.X, _core.RenderOffset.Y));
                    MapOverlayTranslateTransform.X = x;
                    MapOverlayTranslateTransform.Y = y;

                    // map is scaled already
                    MapTranslateTransform.X = _core.RenderOffset.X;
                    MapTranslateTransform.Y = _core.RenderOffset.Y;
                }
                else
                {
                    MapTranslateTransform.X = _core.RenderOffset.X;
                    MapTranslateTransform.Y = _core.RenderOffset.Y;

                    MapOverlayTranslateTransform.X = MapTranslateTransform.X;
                    MapOverlayTranslateTransform.Y = MapTranslateTransform.Y;
                }
            }
        }

        private Canvas _mapCanvas;
        internal Canvas MapCanvas
        {
            get
            {
                if (_mapCanvas == null)
                {
                    if (VisualChildren.Count > 0)
                    {
                        _mapCanvas = this.GetVisualDescendants().FirstOrDefault(w => w is Canvas) as Canvas;
                        if (_mapCanvas != null) _mapCanvas.RenderTransform = MapTranslateTransform;
                    }
                }

                return _mapCanvas;
            }
        }

        public new void InvalidateVisual()
        {
            _core.Refresh?.Set();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (!_core.IsStarted)
            {
                _core.OnMapOpen().ProgressChanged += (_, _) => base.InvalidateVisual(); 
                ForceUpdateOverlays();
            }

        }

        public void InvalidateVisual(bool forced)
        {
            if (forced)
            {
                lock (_core.InvalidationLock)
                {
                    _core.LastInvalidation = DateTime.Now;
                }

                base.InvalidateVisual();
            }
            else
            {
                InvalidateVisual();
            }
        }

        private void DrawMap(DrawingContext g)
        {
            if (MapProvider == EmptyProvider.Instance || MapProvider == null)
            {
                return;
            }

            _core.TileDrawingListLock.AcquireReaderLock();
            _core.Matrix.EnterReadLock();

            try
            {
                foreach (var tilePoint in _core.TileDrawingList)
                {
                    _core.TileRect.Location = tilePoint.PosPixel;
                    _core.TileRect.OffsetNegative(_core.CompensationOffset);

                    //if(region.IntersectsWith(Core.tileRect) || IsRotated)
                    {
                        bool found = false;

                        var t = _core.Matrix.GetTileWithNoLock(_core.Zoom, tilePoint.PosXY);

                        if (t.NotEmpty)
                        {
                            foreach (GMapImage img in t.Overlays)
                            {
                                if (img != null && img.Img != null)
                                {
                                    if (!found)
                                        found = true;

                                    var imgRect = new Rect(_core.TileRect.X + 0.6,
                                        _core.TileRect.Y + 0.6,
                                        _core.TileRect.Width + 0.6,
                                        _core.TileRect.Height + 0.6);

                                    if (!img.IsParent)
                                    {
                                        g.DrawImage(img.Img, imgRect);
                                    }
                                    else
                                    {
                                        // TODO: move calculations to loader thread
                                        var geometry = new RectangleGeometry(imgRect);
                                        var parentImgRect =
                                            new Rect(_core.TileRect.X - _core.TileRect.Width * img.Xoff + 0.6,
                                                _core.TileRect.Y - _core.TileRect.Height * img.Yoff + 0.6,
                                                _core.TileRect.Width * img.Ix + 0.6,
                                                _core.TileRect.Height * img.Ix + 0.6);

                                        using (g.PushClip(geometry.Rect))
                                        {
                                            g.DrawImage(img.Img, parentImgRect);
                                        }
                                        geometry = null;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                _core.Matrix.LeaveReadLock();
                _core.TileDrawingListLock.ReleaseReaderLock();
            }
        }

        public override void Render(DrawingContext drawingContext)
        {
            if (!_core.IsStarted)
                return;


            if (MapScaleTransform != null)
            {
                using (drawingContext.PushPreTransform(MapScaleTransform.Value))
                using (drawingContext.PushPreTransform(MapTranslateTransform.Value))
                {
                    DrawMap(drawingContext);
                }
            }
            else
            {
                using var _ = drawingContext.PushPreTransform(MapTranslateTransform.Value);
                DrawMap(drawingContext);
            }

            base.Render(drawingContext);
        }

        private void ForceUpdateOverlays()
        {
            UpdateMarkersOffset();
            foreach (AvaloniaMapItem i in LogicalChildren.Select(_ => _ as AvaloniaMapItem).Where(_ => _ != null))
            {
                i.UpdateLocalPosition();
            }
        }

        public RectLatLng ViewArea
        {
            get
            {
                if (_core.Provider.Projection != null)
                {
                    var p = FromLocalToLatLng(0, 0);
                    var p2 = FromLocalToLatLng((int)Bounds.Width, (int)Bounds.Height);

                    return RectLatLng.FromLTRB(p.Lng, p.Lat, p2.Lng, p2.Lat);
                }

                return RectLatLng.Empty;
            }
        }

        public PointLatLng FromLocalToLatLng(int x, int y)
        {
            if (MapScaleTransform != null)
            {
                var tp = MapScaleTransform.Inverse().Transform(new Point(x, y));
                x = (int)tp.X;
                y = (int)tp.Y;
            }
            return _core.FromLocalToLatLng(x, y);
        }

        public GPoint FromLatLngToLocal(PointLatLng point)
        {
            var ret = _core.FromLatLngToLocal(point);

            if (MapScaleTransform != null)
            {
                var tp = MapScaleTransform.Transform(new Point(ret.X, ret.Y));
                ret.X = (int)tp.X;
                ret.Y = (int)tp.Y;
            }
            return ret;
        }

        #endregion

        #region MapProvider

        public static readonly DirectProperty<AvaloniaMap, GMapProvider> MapProviderProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMap, GMapProvider>(nameof(MapProvider), o => o.MapProvider, (o, v) => o.MapProvider = v);

        private GMapProvider _mapProvider;
        public GMapProvider MapProvider
        {
            get => _mapProvider;
            set
            {
                if (SetAndRaise(MapProviderProperty, ref _mapProvider, value))
                {
                    UpdateMapProvider();
                }
            }
        }

        private void UpdateMapProvider()
        {
            var viewarea = SelectedArea;

            if (viewarea != RectLatLng.Empty)
            {
                Position = new PointLatLng(viewarea.Lat - viewarea.HeightLat / 2,
                    viewarea.Lng + viewarea.WidthLng / 2);
            }
            else
            {
                viewarea = ViewArea;
            }

            _core.Provider = MapProvider;

            if (_core.IsStarted && _core.ZoomToArea)
            {
                // restore zoomrect as close as possible
                if (viewarea != RectLatLng.Empty && viewarea != ViewArea)
                {
                    int bestZoom = _core.GetMaxZoomToFitRect(viewarea);

                    if (bestZoom > 0 && Zoom != bestZoom)
                        Zoom = bestZoom;
                }
            }
        }

        #endregion

        #region RectLatLng

        public static readonly DirectProperty<AvaloniaMap, RectLatLng> SelectedAreaProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMap, RectLatLng>(nameof(SelectedArea), o => o.SelectedArea, (o, v) => o.SelectedArea = v);
        private RectLatLng _selectedArea;
        public RectLatLng SelectedArea
        {
            get => _selectedArea;
            set
            {
                if (SetAndRaise(SelectedAreaProperty, ref _selectedArea, value))
                {
                    InvalidateVisual();
                }
            }
        }

        #endregion

        #region Position

        public static readonly DirectProperty<AvaloniaMap, PointLatLng> PositionProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMap, PointLatLng>(nameof(Position), o => o.Position, (o, v) => o.Position = v);
        private PointLatLng _position = new PointLatLng(55.1644, 61.4368);
        public PointLatLng Position
        {
            get => _position;
            set
            {
                if (SetAndRaise(PositionProperty, ref _position, value))
                {
                    OnPositionChanged();
                }
            }
        }

        private void OnPositionChanged()
        {
            _core.Position = Position;
            if (_core.IsStarted)
            {
                ForceUpdateOverlays();
            }
        }

        #endregion

        #region Zoom

        public static readonly DirectProperty<AvaloniaMap, double> ZoomProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMap, double>(nameof(Zoom), o => o.Zoom, (o, v) => o.Zoom = v);
        private double _zoom;
        public double Zoom
        {
            get => _zoom;
            set
            {
                if (SetAndRaise(ZoomProperty, ref _zoom, value))
                {
                    UpdateZoom();
                }
            }
        }

        public static readonly DirectProperty<AvaloniaMap, ScaleModes> ScaleModeProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMap, ScaleModes>(nameof(ScaleMode), o => o.ScaleMode, (o, v) => o.ScaleMode = v);

        private ScaleModes _scaleMode;
        public ScaleModes ScaleMode
        {
            get => _scaleMode;
            set
            {
                if (SetAndRaise(ScaleModeProperty, ref _scaleMode, value))
                {
                    InvalidateVisual();
                }
            }
        }

        public static readonly DirectProperty<AvaloniaMap, int> MinZoomProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMap, int>(nameof(MinZoom), o => o.MinZoom, (o, v) => o.MinZoom = v);
        private int _minZoom;
        public int MinZoom
        {
            get => _minZoom;
            set
            {
                if (SetAndRaise(MinZoomProperty, ref _minZoom, value))
                {
                    _core.MinZoom = value;
                    UpdateZoom();
                }
            }
        }
        public static readonly DirectProperty<AvaloniaMap, int> MaxZoomProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMap, int>(nameof(MaxZoom), o => o.MaxZoom, (o, v) => o.MaxZoom = v);
        private int _maxZoom;
        public int MaxZoom
        {
            get => _maxZoom;
            set
            {
                if (SetAndRaise(MinZoomProperty, ref _maxZoom, value))
                {
                    _core.MaxZoom = value;
                    UpdateZoom();
                }
            }
        }

        // public static readonly DirectProperty<AvaloniaMap, double> ZoomXProperty =
        //     AvaloniaProperty.RegisterDirect<AvaloniaMap, double>(nameof(ZoomX), o => o.ZoomX, (o, v) => o.ZoomX = v);
        // private double _zoomX;
        // public double ZoomX
        // {
        //     get => _zoomX;
        //     set
        //     {
        //         if (SetAndRaise(ZoomXProperty, ref _zoomX, value))
        //         {
        //             UpdateZoom();
        //         }
        //     }
        // }
        //
        // public static readonly DirectProperty<AvaloniaMap, double> ZoomYProperty =
        //     AvaloniaProperty.RegisterDirect<AvaloniaMap, double>(nameof(ZoomY), o => o.ZoomY, (o, v) => o.ZoomY = v);
        // private double _zoomY;
        // public double ZoomY
        // {
        //     get => _zoomY;
        //     set
        //     {
        //         if (SetAndRaise(ZoomYProperty, ref _zoomY, value))
        //         {
        //             UpdateZoom();
        //         }
        //     }
        // }

        private void UpdateZoom()
        {
            if (MapProvider?.Projection != null)
            {
                double remainder = Zoom % 1;

                if (ScaleMode != ScaleModes.Integer && remainder != 0 && Bounds.Width > 0)
                {
                    bool scaleDown;

                    switch (ScaleMode)
                    {
                        case ScaleModes.ScaleDown:
                            scaleDown = true;
                            break;

                        case ScaleModes.Dynamic:
                            scaleDown = remainder > 0.25;
                            break;

                        default:
                            scaleDown = false;
                            break;
                    }

                    if (scaleDown)
                        remainder--;

                    double scaleValue = Math.Pow(2d, remainder);
                    {
                        if (MapScaleTransform == null)
                        {
                            MapScaleTransform = _lastScaleTransform;
                        }

                        MapScaleTransform.ScaleX = scaleValue;
                        _core.ScaleX = 1 / scaleValue;
                        MapScaleTransform.ScaleY = scaleValue;
                        _core.ScaleY = 1 / scaleValue;
                    }

                    _core.Zoom = Convert.ToInt32(scaleDown ? Math.Ceiling(Zoom) : Zoom - remainder);
                }
                else
                {
                    MapScaleTransform = null;

                    _core.ScaleX = 1;
                    _core.ScaleY = 1;
                    _core.Zoom = (int)Math.Floor(Zoom);
                }

                if (IsInitialized)
                {
                    ForceUpdateOverlays();
                    InvalidateVisual();
                }
            }
        }

        #endregion

        #region OnWheelChanged

        public static readonly DirectProperty<AvaloniaMap, bool> InvertedMouseWheelZoomingProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMap, bool>(nameof(InvertedMouseWheelZooming), o => o.InvertedMouseWheelZooming, (o, v) => o.InvertedMouseWheelZooming = v);
        private bool _invertedMouseWheelZooming = false;
        public bool InvertedMouseWheelZooming
        {
            get => _invertedMouseWheelZooming;
            set => SetAndRaise(InvertedMouseWheelZoomingProperty, ref _invertedMouseWheelZooming, value);
        }
        

        public static readonly DirectProperty<AvaloniaMap, bool> MouseWheelZoomEnabledProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMap, bool>(nameof(MouseWheelZoomEnabled), o => o.MouseWheelZoomEnabled, (o, v) => o.MouseWheelZoomEnabled = v);
        private bool _mouseWheelZoomEnabled = true;
        public bool MouseWheelZoomEnabled
        {
            get => _mouseWheelZoomEnabled;
            set => SetAndRaise(MouseWheelZoomEnabledProperty, ref _mouseWheelZoomEnabled, value);
        }


        public static readonly DirectProperty<AvaloniaMap, MouseWheelZoomType> MouseWheelZoomTypeProperty =
            AvaloniaProperty.RegisterDirect<AvaloniaMap, MouseWheelZoomType>(nameof(MouseWheelZoomType), o => o.MouseWheelZoomType, (o, v) => o.MouseWheelZoomType = v);
        private MouseWheelZoomType _mouseWheelZoomType;
        public MouseWheelZoomType MouseWheelZoomType
        {
            get => _mouseWheelZoomType;
            set => SetAndRaise(MouseWheelZoomTypeProperty, ref _mouseWheelZoomType, value);
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);
            //TODO: && (IsMouseDirectlyOver || IgnoreMarkerOnMouseWheel)
            if (MouseWheelZoomEnabled && !_core.IsDragging)
            {
                var p = e.GetPosition(this);

                if (MapScaleTransform != null)
                {
                    p = MapScaleTransform.Inverse().Transform(p);
                }

                if (_core.MouseLastZoom.X != (int)p.X && _core.MouseLastZoom.Y != (int)p.Y)
                {
                    if (MouseWheelZoomType == MouseWheelZoomType.MousePositionAndCenter)
                    {
                        Position = FromLocalToLatLng((int)p.X, (int)p.Y);
                    }
                    else if (MouseWheelZoomType == MouseWheelZoomType.ViewCenter)
                    {
                        Position = FromLocalToLatLng((int)Bounds.Width / 2, (int)Bounds.Height / 2);
                    }
                    else if (MouseWheelZoomType == MouseWheelZoomType.MousePositionWithoutCenter)
                    {
                        Position = FromLocalToLatLng((int)p.X, (int)p.Y);
                    }

                    _core.MouseLastZoom.X = (int)p.X;
                    _core.MouseLastZoom.Y = (int)p.Y;
                }

                // set mouse position to map center
                if (MouseWheelZoomType != MouseWheelZoomType.MousePositionWithoutCenter)
                {
                    var ps = this.PointToScreen(new Point(Bounds.Width / 2, Bounds.Height / 2));
                    // Stuff.SetCursorPos(ps.X, ps.Y);
                }

                _core.MouseWheelZooming = true;

                if (e.Delta.Y > 0)
                {
                    if (!InvertedMouseWheelZooming)
                    {
                        Zoom = (int)Zoom + 1;
                    }
                    else
                    {
                        Zoom = (int)(Zoom + 0.99) - 1;
                    }
                }
                else
                {
                    if (InvertedMouseWheelZooming)
                    {
                        Zoom = (int)Zoom + 1;
                    }
                    else
                    {
                        Zoom = (int)(Zoom + 0.99) - 1;
                    }
                }

                _core.MouseWheelZooming = false;
            }
        }

        #endregion

        #region Selection

        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return new ItemContainerGenerator<AvaloniaMapItem>(
                this,
                ContentControl.ContentProperty,
                ContentControl.ContentTemplateProperty);
        }

        public new static readonly DirectProperty<SelectingItemsControl, IList?> SelectedItemsProperty =
            SelectingItemsControl.SelectedItemsProperty;

        public new static readonly DirectProperty<SelectingItemsControl, ISelectionModel> SelectionProperty =
            SelectingItemsControl.SelectionProperty;

        public new static readonly StyledProperty<SelectionMode> SelectionModeProperty =
            SelectingItemsControl.SelectionModeProperty;

        


        public new IList? SelectedItems
        {
            get => base.SelectedItems;
            set => base.SelectedItems = value;
        }

        public new SelectionMode SelectionMode
        {
            get => base.SelectionMode;
            set => base.SelectionMode = value;
        }

        public new ISelectionModel Selection
        {
            get => base.Selection;
            set => base.Selection = value;
        }

        public bool IsDragging { get; private set; }

        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            base.OnGotFocus(e);

            if (e.NavigationMethod == NavigationMethod.Directional)
            {
                e.Handled = UpdateSelectionFromEventSource(
                    e.Source,
                    true,
                    e.KeyModifiers.HasAllFlags(KeyModifiers.Shift),
                    e.KeyModifiers.HasAllFlags(KeyModifiers.Control));
            }
        }

        public void SelectAll() => Selection.SelectAll();
        public void UnselectAll() => Selection.Clear();

        /// <summary>
        ///     map boundaries
        /// </summary>
        public RectLatLng? BoundsOfMap { get; }

        

        private ulong _onMouseUpTimestamp;
        private Cursor _cursorBefore = Cursor.Default;
        


        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            // wpf generates to many events if mouse is over some visual
            // and OnMouseUp is fired, wtf, anyway...
            // http://greatmaps.codeplex.com/workitem/16013
            if ((e.Timestamp & UInt32.MaxValue) - _onMouseUpTimestamp < 55)
            {
                Debug.WriteLine("OnMouseMove skipped: " + ((e.Timestamp & Int32.MaxValue) - _onMouseUpTimestamp) + "ms");
                return;
            }

            if (!_core.IsDragging && !_core.MouseDown.IsEmpty)
            {
                var p = e.GetPosition(this);

                if (MapScaleTransform != null)
                {
                    p = MapScaleTransform.Inverse().Transform(p);
                }

                // cursor has moved beyond drag tolerance
                if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                {
                    if (Math.Abs(p.X - _core.MouseDown.X) * 2 >= MinimumHorizontalDragDistance ||
                        Math.Abs(p.Y - _core.MouseDown.Y) * 2 >= MinimumVerticalDragDistance)
                    {
                        _core.BeginDrag(_core.MouseDown);
                    }
                }
            }

            if (_core.IsDragging)
            {
                if (!IsDragging)
                {
                    IsDragging = true;
                    Debug.WriteLine("IsDragging = " + IsDragging);
                    _cursorBefore = Cursor;
                    Cursor = new Cursor(StandardCursorType.SizeAll);
                    _mouse.Capture(this);
                }

                if (BoundsOfMap.HasValue && !BoundsOfMap.Value.Contains(Position))
                {
                    // ...
                }
                else
                {
                    var p = e.GetPosition(this);

                    if (MapScaleTransform != null)
                    {
                        p = MapScaleTransform.Inverse().Transform(p);
                    }

                    _core.MouseCurrent.X = (int)p.X;
                    _core.MouseCurrent.Y = (int)p.Y;
                    {
                        _core.Drag(_core.MouseCurrent);
                    }

                    if (_scaleMode != ScaleModes.Integer)
                    {
                        ForceUpdateOverlays();
                    }
                    else
                    {
                        UpdateMarkersOffset();
                    }
                }

                InvalidateVisual(true);
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (_core.IsDragging)
            {
                if (IsDragging)
                {
                    _onMouseUpTimestamp = e.Timestamp & ulong.MaxValue;
                    IsDragging = false;
                    Debug.WriteLine("IsDragging = " + IsDragging);
                    Cursor = _cursorBefore;
                    _mouse.Capture(null);
                }

                _core.EndDrag();

                if (BoundsOfMap.HasValue && !BoundsOfMap.Value.Contains(Position))
                {
                    if (_core.LastLocationInBounds.HasValue)
                    {
                        Position = _core.LastLocationInBounds.Value;
                    }
                }
            }
            
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            var p = e.GetPosition(this);

            if (MapScaleTransform != null)
            {
                p = MapScaleTransform.Inverse().Transform(p);
            }

            _core.MouseDown.X = (int)p.X;
            _core.MouseDown.Y = (int)p.Y;

            InvalidateVisual();

            //
            // if (e.Source is IVisual source)
            // {
            //     var point = e.GetCurrentPoint(source);
            //
            //     if (point.Properties.IsLeftButtonPressed || point.Properties.IsRightButtonPressed)
            //     {
            //         e.Handled = UpdateSelectionFromEventSource(
            //             e.Source,
            //             true,
            //             e.KeyModifiers.HasAllFlags(KeyModifiers.Shift),
            //             e.KeyModifiers.HasAllFlags(KeyModifiers.Control),
            //             point.Properties.IsRightButtonPressed);
            //     }
            // }
        }

        #endregion

        public void Dispose()
        {
            _dispose.Dispose();
        }

        
    }
}
