using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;

namespace MB.Common.Cache
{
    public static class CacheManager
    {
        #region Static Fields

        private static CacheProvider _defaultCacheProvider;

        #endregion

        //private static bool isInitialized;

        #region Constructors and Destructors

        static CacheManager()
        {
            Initialize();
        }

        #endregion

        #region Public Properties

        public static CacheProvider Provider
        {
            get
            {
                //Initialize();
                return _defaultCacheProvider;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static object Get(string key)
        {
            return Provider.Get(key);
        }

        public static T Get<T>(string key, Func<T> funcIfNull, CacheDuration duration)
        {
            var item = (T)Provider.Get(key);
            if (item == null)
            {
                item = funcIfNull();
                if (item != null)
                {
                    Set(key, item, duration);
                }
            }
            return item;
        }

        public static void Remove(string key)
        {
            Provider.Remove(key);
        }

        public static void Set(string key, object value, CacheDuration duration)
        {
            Provider.Set(key, value, GetTimeSpanFromDuration(duration));
        }

        #endregion

        #region Methods

        private static TimeSpan GetTimeSpanFromDuration(CacheDuration duration)
        {
            switch (duration)
            {
                case CacheDuration.Short:
                    return Provider.ShortCacheDuration;
                case CacheDuration.Medium:
                    return Provider.MediumCacheDuration;
                default:
                    return Provider.LongCacheDuration;
            }
        }

        private static void Initialize()
        {
            //if (isInitialized)
            //{
            //    return;
            //}
            var cacheProviderConfigSection = ConfigurationManager.GetSection("cacheProviderConfiguration") as CacheProviderConfiguration;
            if (cacheProviderConfigSection == null)
            {
                throw new ConfigurationErrorsException("Missing Cache Provider Configuration section");
            }

            var setting = cacheProviderConfigSection.Providers[cacheProviderConfigSection.DefaultProvider];
            _defaultCacheProvider = ProvidersHelper.InstantiateProvider(setting, typeof(CacheProvider)) as CacheProvider;

            if (_defaultCacheProvider == null)
            {
                throw new ProviderException("Cannot load default cache provider.");
            }
            //isInitialized = true;
        }

        #endregion
    }

    public enum CacheDuration
    {
        Short,

        Medium,

        Long
    }
}