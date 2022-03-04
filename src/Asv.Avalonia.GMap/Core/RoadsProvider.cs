using System.Collections.Generic;

namespace Asv.Avalonia.GMap
{
    /// <summary>
    ///     roads interface
    /// </summary>
    public interface RoadsProvider
    {
        MapRoute GetRoadsRoute(List<PointLatLng> points, bool interpolate);

        MapRoute GetRoadsRoute(string points, bool interpolate);
    }
}
