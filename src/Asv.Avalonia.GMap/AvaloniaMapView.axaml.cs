using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace Asv.Avalonia.GMap
{
    public partial class AvaloniaMapView : ReactiveUserControl<AvaloniaMapViewModel>
    {
        private CancellationTokenSource _viewModelCancellation;
        private IObservable<EventPattern<NotifyCollectionChangedEventArgs>> _anchorsChanged;

        public AvaloniaMapView()
        {
            InitializeComponent();
            Map = this.Get<GMapControl>("Map");
            this.DataContextChanged += AvaloniaMapView_DataContextChanged;
        }

        private void AvaloniaMapView_DataContextChanged(object sender, EventArgs e)
        {
            BindMarkers();
        }

        private void BindMarkers()
        {
            if (_viewModelCancellation != null)
            {
                _viewModelCancellation.Cancel();
                _viewModelCancellation.Dispose();
                _viewModelCancellation = null;
                Map.Markers.Clear();
            }
            _viewModelCancellation = new CancellationTokenSource();

            

            // _anchorsChanged = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
            //     _ =>
            //     {
            //         if (ViewModel != null) ViewModel.Anchors.CollectionChanged += _;
            //     }, _ =>
            //     {
            //         if (ViewModel != null) ViewModel.Anchors.CollectionChanged -= _;
            //     });
            // _anchorsChanged.Where(_=>_.EventArgs.Action == NotifyCollectionChangedAction.Add)

            
        }

        public GMapControl Map { get; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        


    }
}
