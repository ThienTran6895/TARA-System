using System.Configuration;

namespace MB.Common.Cache
{
    public class CacheProviderConfiguration : ConfigurationSection
    {
        #region Public Properties

        [ConfigurationProperty("defaultProvider", DefaultValue = "NoCache")]
        public string DefaultProvider
        {
            get
            {
                return (string)base["defaultProvider"];
            }
            set
            {
                base["defaultProvider"] = value;
            }
        }

        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get
            {
                return (ProviderSettingsCollection)base["providers"];
            }
        }

        #endregion
    }
}