using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Styling;

namespace Asv.Avalonia.GMap
{
    public class MapControl:ItemsControl
    {

        public MapControl()
        {
            // ItemTemplate = new DataTemplate()
            // {
            //     Content = new ContentPresenter
            //     {
            //         Content = new TextBlock{Text = "asdasd"}
            //     }
            // }; ;
            //
            //
            // ItemsPanel = new ItemsPanelTemplate
            // {
            //     Content = new StackPanel()
            // };
            //

            // var _styleInstance = new Style();
            // {
            //     _styleInstance.Setters.Add(new Setter(Canvas.LeftProperty, new Binding("LocalPositionX")));
            //     _styleInstance.Setters.Add(new Setter(Canvas.TopProperty, new Binding("LocalPositionY")));
            //     _styleInstance.Setters.Add(new Setter(Panel.ZIndexProperty, new Binding("ZIndex")));
            // }
            // Styles.Add(_styleInstance);
        }
    }
}