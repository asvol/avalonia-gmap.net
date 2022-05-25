using System.Collections.Generic;
using Asv.Tools;

namespace Asv.Avalonia.GMap
{
    /// <summary>
    ///     roads interface
    /// </summary>
    public interface RoadsProvider
    {
        MapRoute GetRoadsRoute(List<GeoPoint> points, bool interpolate);

        MapRoute GetRoadsRoute(string points, bool interpolate);
    }
}
