using System;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace Asv.Avalonia.GMap
{
    public class AvaloniaMapViewModel:ReactiveObject
    {
        public AvaloniaMapViewModel()
        {
            
        }


        public IObservable<IChangeSet<MapAnchorViewModel,string>> Anchors { get; set; }
    }
}