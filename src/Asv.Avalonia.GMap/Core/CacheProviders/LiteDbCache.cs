using System;
using System.IO;
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

    public class FolderDbCache : PureImageCache
    {
        private readonly string _rootFolder;
        public static FolderDbCache Instance { get; } = new FolderDbCache("map");

        public FolderDbCache(string rootFolder)
        {
            _rootFolder = rootFolder;
            if (Directory.Exists(_rootFolder) == false) Directory.CreateDirectory(_rootFolder);
        }

        public bool PutImageToCache(byte[] tile, int type, GPoint pos, int zoom)
        {
            var fileName = GetFileName(type, zoom, pos.X,pos.Y, out var dir);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
                // if directory not exist, file not exist too
            }
            else
            {
                if (File.Exists(fileName)) File.Delete(fileName);
            }
            
            
            File.WriteAllBytes(fileName,tile);
            return true;
        }

        private string GetFileName(int type, int zoom, long posX, long posY, out string dir)
        {
            dir = Path.Combine(_rootFolder, $"T_{type}", $"Z_{zoom:000}");
            return Path.Combine(dir, $"X_{posX}_Y_{posY}.jpg");
        }

        public PureImage GetImageFromCache(int type, GPoint pos, int zoom)
        {
            var fileName = GetFileName(type, zoom, pos.X, pos.Y, out var dir);
            if (File.Exists(fileName) == false) return null;
            using var file = File.OpenRead(fileName);
            var data = new byte[file.Length];
            file.Read(data, 0, data.Length);
            return GMapProvider.TileImageProxy.FromArray(data);
        }

        public int DeleteOlderThan(DateTime date, int? type)
        {
            //TODO: delete files by creation time
            return 0;
        }
    }

    // public class LiteDbCache : PureImageCache
    // {
    //     public static LiteDbCache Instance { get; } = new LiteDbCache("temp.db");
    //
    //     private readonly LiteDatabase _db;
    //     private readonly ILiteCollection<LiteDbTile> _tiles;
    //
    //     public LiteDbCache(string fileName)
    //     {
    //         _db = new LiteDatabase(fileName);
    //         _tiles = _db.GetCollection<LiteDbTile>("Tiles");
    //     }
    //
    //     public bool PutImageToCache(byte[] tile, int type, GPoint pos, int zoom)
    //     {
    //         _tiles.Upsert(new LiteDbTile { Id = $"{type} {zoom} {pos.X} {pos.Y}", Data = tile, Zoom = zoom,X=pos.X,Y=pos.Y,Updated = DateTime.Now,Type = type});
    //         return true;
    //     }
    //
    //     public PureImage GetImageFromCache(int type, GPoint pos, int zoom)
    //     {
    //         var id = $"{type} {zoom} {pos.X} {pos.Y}";
    //         var a = _tiles.FindOne(_=>_.Id == id);
    //         if (a == null) return null;
    //         return GMapProvider.TileImageProxy.FromArray(a.Data);
    //     }
    //
    //     public int DeleteOlderThan(DateTime date, int? type)
    //     {
    //         return 0;
    //     }
    //
    //     public void Ping()
    //     {
    //         
    //     }
    //
    //     public bool ExportMapDataToDB(string toString, string file)
    //     {
    //         return false;
    //     }
    // }
}
