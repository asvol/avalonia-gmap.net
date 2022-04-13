﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Threading;
using Avalonia.Controls;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.GMap
{
    public class MapAnchorViewModel:ReactiveObject
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
        public MaterialIconKind Icon { get; set; }
        [Reactive]
        public double RotateCenterX { get; set; }
        [Reactive]
        public double RotateCenterY { get; set; }

        [Reactive]
        public OffsetXEnum OffsetX { get; set; }
        [Reactive]
        public OffsetYEnum OffsetY { get; set; }

        [Reactive]
        public bool IsSelected { get; set; }
        [Reactive]
        public bool IsVisible { get; set; } = true;
        [Reactive]
        public double RotateAngle { get; set; }
        [Reactive]
        public string Title { get; set; }
        [Reactive]
        public string Description { get; set; }
        [Reactive]
        public PointLatLng Location { get; set; }
        [Reactive]
        public double Size { get; set; } = 32;

        public virtual ReadOnlyObservableCollection<MapAnchorActionViewModel> Actions { get; }
    }
}