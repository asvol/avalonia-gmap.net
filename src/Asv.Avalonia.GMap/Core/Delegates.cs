﻿using System;
using Asv.Tools;

namespace Asv.Avalonia.GMap
{
    public delegate void PositionChanged(GeoPoint point);

    public delegate void TileLoadComplete(long elapsedMilliseconds);

    public delegate void TileLoadStart();

    public delegate void TileCacheComplete();

    public delegate void TileCacheStart();

    public delegate void TileCacheProgress(int tilesLeft);

    public delegate void MapDrag();

    public delegate void MapZoomChanged();

    public delegate void MapTypeChanged(GMapProvider type);

    public delegate void EmptyTileError(int zoom, GPoint pos);

    public delegate void ExceptionThrown(Exception exception);
}
