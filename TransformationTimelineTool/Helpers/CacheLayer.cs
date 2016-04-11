using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace TransformationTimelineTool.Helpers
{
    public class CacheLayer
    {
        static readonly ObjectCache Cache = MemoryCache.Default;

        public static T Get<T>(string key) where T : class
        {
            try
            {
                return (T)Cache[key];
            }
            catch
            {
                return null;
            }
        }

        public static void AddItem(object objectToCache, string key)
        {
            Cache.Add(key, objectToCache, DateTime.Now.AddDays(1));
        }

        public static void EmptyTimelineCache()
        {
            if (Cache.Contains("initiative-blocks-mem-en"))
            {
                Cache.Remove("initiative-blocks-mem-en");
            }
            if (Cache.Contains("initiative-blocks-mem-fr"))
            {
                Cache.Remove("initiative-blocks-mem-fr");
            }
        }
    }
}