using System;
using LiteDB;

namespace Asv.Avalonia.GMap
{
    public class LiteDbCache : PureImageCache
    {
        public static LiteDbCache Instance { get; } = new LiteDbCache("temp.db");

        private readonly LiteDatabase _db;

        public LiteDbCache(string fileName)
        {
            // _db = new LiteDatabase(fileName);
        }

        public bool PutImageToCache(byte[] tile, int type, GPoint pos, int zoom)
        {
            return false;
        }

        public PureImage GetImageFromCache(int type, GPoint pos, int zoom)
        {
            return null;
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
