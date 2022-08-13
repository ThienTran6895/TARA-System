using System;

namespace MB.Common.Cache
{
    public class NoCacheProvider : CacheProvider
    {
        #region Public Properties

        public override TimeSpan LongCacheDuration
        {
            get
            {
                return TimeSpan.Zero;
            }
        }

        public override TimeSpan MediumCacheDuration
        {
            get
            {
                return TimeSpan.Zero;
            }
        }

        public override TimeSpan ShortCacheDuration
        {
            get
            {
                return TimeSpan.Zero;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override object Get(string key)
        {
            return null;
        }

        public override void Remove(string key)
        {
        }

        public override void Set(string key, object value, TimeSpan timeout)
        {
        }

        #endregion
    }
}