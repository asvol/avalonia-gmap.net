using System;
using LiteDB;

namespace Asv.Avalonia.GMap
{
    public class LiteDbTile
    {
        [BsonId]
        public string Id { get; set; }
        public int Zoom { get; set; }
        public long Y { get; set; }
        public long X { get; set; }
        public int Type { get; set; }
        public DateTime Updated { get; set; }
        public byte[] Data { get; set; }
    }

    public class LiteDbCache : PureImageCache
    {
        public static LiteDbCache Instance { get; } = new LiteDbCache("temp.db");

        private readonly LiteDatabase _db;
        private readonly ILiteCollection<LiteDbTile> _tiles;

        public LiteDbCache(string fileName)
        {
            _db = new LiteDatabase(fileName);
            _tiles = _db.GetCollection<LiteDbTile>("Tiles");
        }

        public bool PutImageToCache(byte[] tile, int type, GPoint pos, int zoom)
        {
            _tiles.Upsert(new LiteDbTile { Id = $"{type} {zoom} {pos.X} {pos.Y}", Data = tile, Zoom = zoom,X=pos.X,Y=pos.Y,Updated = DateTime.Now,Type = type});
            return true;
        }

        public PureImage GetImageFromCache(int type, GPoint pos, int zoom)
        {
            var id = $"{type} {zoom} {pos.X} {pos.Y}";
            var a = _tiles.FindOne(_=>_.Id == id);
            if (a == null) return null;
            return GMapProvider.TileImageProxy.FromArray(a.Data);
        }

        public int DeleteOlderThan(DateTime date, int? type)
        {
            return 0;
        }

        public void Ping()
        {
            
        }

        public bool ExportMapDataToDB(string toString, string file)
        {
            return false;
        }
    }
}
