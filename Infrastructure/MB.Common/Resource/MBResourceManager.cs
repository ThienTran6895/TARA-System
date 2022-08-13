using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace MB.Common.Resource
{
    public class MBResourceManager : DynamicObject
    {
        private static List<ResourceManager> _resourceList = new List<ResourceManager>();
        private static dynamic _resourceManager = null;
        private static string currentBaseName = string.Empty;
        public static dynamic LoadResource(string baseName, string assembly)
        {
            currentBaseName = baseName;
            Assembly currentAssembly;
            if (string.IsNullOrEmpty(assembly))
            {
                currentAssembly = Assembly.GetExecutingAssembly();
            }
            else
            {
                currentAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName.Contains(assembly + ","));
            }
            
            if (!_resourceList.Exists(c => c.BaseName == baseName))
            {
                _resourceList.Add(new ResourceManager(baseName, currentAssembly));
            }

            if (_resourceManager == null)
            {
                _resourceManager = new MBResourceManager();
            }
            return _resourceManager;
        }

        public static dynamic LoadResource(string keySetting = "")
        {
            string baseName = string.Empty;
            string assembly = string.Empty;
            string resourceInfo = ConfigurationManager.AppSettings[keySetting];
            if (!string.IsNullOrEmpty(resourceInfo))
            {
                var infoArray = resourceInfo.Split(',');
                baseName = infoArray[0].Trim();
                assembly = infoArray[1].Trim();
            }
            if (string.IsNullOrEmpty(baseName))
                baseName = "MB.Common.Resource." + keySetting;
            return LoadResource(baseName, assembly);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetMember(binder.Name);
            return !string.IsNullOrEmpty(result.ToString());
        }

        public string GetMember(string name)
        {
            var currentResource = _resourceList.Find(r => r.BaseName == currentBaseName);
            
            return currentResource.GetString(name, CultureInfo.CurrentUICulture);
        }
    }
}
