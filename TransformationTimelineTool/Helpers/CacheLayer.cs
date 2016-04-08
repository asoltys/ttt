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
    }
}