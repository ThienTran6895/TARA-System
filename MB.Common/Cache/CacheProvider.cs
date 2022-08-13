using System;
using System.Configuration.Provider;

namespace MB.Common.Cache
{
    public abstract class CacheProvider : ProviderBase
    {
        #region Public Properties

        public abstract TimeSpan LongCacheDuration { get; }
        public abstract TimeSpan MediumCacheDuration { get; }
        public abstract TimeSpan ShortCacheDuration { get; }

        #endregion

        #region Public Methods and Operators

        public abstract object Get(string key);

        public abstract void Remove(string key);

        public abstract void Set(string key, object value, TimeSpan timeout);

        #endregion
    }
}