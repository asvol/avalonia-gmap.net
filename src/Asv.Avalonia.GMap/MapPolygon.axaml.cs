using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.VisualTree;
using JetBrains.Annotations;

namespace Asv.Avalonia.GMap
{
    public class MapPolygon : TemplatedControl
    {

        public MapPolygon()
        {

        }

        private AvaloniaMap _map;
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            IControl a = this;
            while (a != null)
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

        private void UpdateLocalPosition()
        {
            if (_map == null) return;
            var localPath = new List<Point>();

        }

        /// <summary>
        ///     creates path from list of points, for performance set addBlurEffect to false
        /// </summary>
        /// <returns></returns>
        public static Path CreatePath(List<Point> localPath, bool addBlurEffect)
        {
            // Create a StreamGeometry to use to specify myPath.
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(localPath[0], true);
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
                if (addBlurEffect)
                {
                    //BlurEffect ef = new BlurEffect();
                    //{
                    //    ef.KernelType = KernelType.Gaussian;
                    //    ef.Radius = 3.0;
                    //    ef.RenderingBias = RenderingBias.Performance;
                    //}
                    //myPath.Effect = ef;
                }

                myPath.Stroke = Brushes.MidnightBlue;
                myPath.StrokeThickness = 5;
                myPath.StrokeJoin = PenLineJoin.Round;
                myPath.StrokeLineCap = PenLineCap.Square;
                myPath.Fill = Brushes.AliceBlue;
                myPath.Opacity = 0.6;
                myPath.IsHitTestVisible = false;
            }
            return myPath;
        }

        public static readonly DirectProperty<MapPolygon, IEnumerable> ItemsProperty =
            AvaloniaProperty.RegisterDirect<MapPolygon, IEnumerable>(nameof(Items), o => o.Items, (o, v) => o.Items = v);

        private IEnumerable _items;

        public IEnumerable Items
        {
            get => _items;
            set
            {
                if (SetAndRaise(ItemsProperty, ref _items, value))
                {
                    UpdateLocalPosition();
                };
            }
        }
    }
}
