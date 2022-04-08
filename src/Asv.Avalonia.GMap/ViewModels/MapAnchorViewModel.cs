using System;
using System.Reactive.Disposables;
using System.Threading;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.GMap
{
    public abstract class MapAnchorViewModel:ReactiveObject,IDisposable
    {
        [Reactive]
        public MaterialIconKind Icon { get; set; }
        [Reactive]
        public double RotateCenterX { get; set; }
        [Reactive]
        public double RotateCenterY { get; set; }

        [Reactive]
        public double OffsetX { get; set; }
        [Reactive]
        public double OffsetY { get; set; }

        [Reactive]
        public bool IsSelected { get; set; }
        [Reactive]
        public bool IsVisible { get; set; } = true;
        [Reactive]
        public double RotateAngle { get; set; }
        [Reactive]
        public string Title { get; set; }
        [Reactive]
        public PointLatLng Location { get; set; }
        [Reactive]
        public double Size { get; set; } = 32;

        public abstract void Dispose();

    }
}