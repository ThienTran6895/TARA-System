using System;
using System.Collections.Specialized;
using System.Runtime.Caching;

namespace MB.Common.Cache
{
    public class MemoryCacheProvider : CacheProvider
    {
        #region Fields

        private ObjectCache cache;

        private TimeSpan longCacheDuration;

        private TimeSpan mediumCacheDuration;

        private TimeSpan shortCacheDuration;

        #endregion

        #region Public Properties

        public override TimeSpan LongCacheDuration
        {
            get
            {
                return longCacheDuration;
            }
        }

        public override TimeSpan MediumCacheDuration
        {
            get
            {
                return mediumCacheDuration;
            }
        }

        public override TimeSpan ShortCacheDuration
        {
            get
            {
                return shortCacheDuration;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override object Get(string key)
        {
            return cache.Get(key);
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
            cache = MemoryCache.Default;

            shortCacheDuration = TimeSpanHelper.Parse(config["shortCacheDuration"]);
            mediumCacheDuration = TimeSpanHelper.Parse(config["mediumCacheDuration"]);
            longCacheDuration = TimeSpanHelper.Parse(config["longCacheDuration"]);
        }

        public override void Remove(string key)
        {
            cache.Remove(key);
        }

        public override void Set(string key, object value, TimeSpan timeout)
        {
            var policy = new CacheItemPolicy { SlidingExpiration = timeout };
            cache.Set(key, value, policy);
        }

        #endregion
    }

    public class TimeSpanHelper
    {
        public static TimeSpan Parse(string timeSpanString)
        {
            try
            {
                var timeSpanParts = timeSpanString.Split(':');
                switch (timeSpanParts.Length)
                {
                    case 3:
                        return new TimeSpan(
                            int.Parse(timeSpanParts[0]),
                            int.Parse(timeSpanParts[1]),
                            int.Parse(timeSpanParts[2]));
                    case 4:
                        return new TimeSpan(
                            int.Parse(timeSpanParts[0]),
                            int.Parse(timeSpanParts[1]),
                            int.Parse(timeSpanParts[2]),
                            int.Parse(timeSpanParts[3]));
                    default:
                        throw new ArgumentException(string.Format("Cannot parse the value {0} to timespan", timeSpanString));
                }
            }
            catch
            {
                throw new ArgumentException(string.Format("Cannot parse the value {0} to timespan", timeSpanString));
            }
        }
    }
}