﻿using System.Windows.Input;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.GMap
{
    public class MapAnchorActionViewModel: ReactiveObject
    {
        [Reactive]
        public MaterialIconKind Icon { get; set; }
        [Reactive]
        public string Title { get; set; }
        [Reactive]
        public ICommand Command { get; set; }
        [Reactive]
        public object CommandParameter { get; set; }
        
    }
}