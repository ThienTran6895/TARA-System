using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Handlers;
using System.Web.Mvc;

namespace MB.Common.Helpers
{
    public static class CommonHelper
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute =
                Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string GetDisplayNameMetaData(Type objectType, string propertyName)
        {
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, objectType, propertyName);
            string displayName = string.Empty;

            if (metadata != null)
                displayName = metadata.GetDisplayName();

            return displayName;
        }

        public static string GetPropertyName<TSource, TField>(this TSource model, Expression<Func<TSource, TField>> field)
        {
            if (object.Equals(field, null))
            {
                return string.Empty;
            }
            return (field.Body as MemberExpression ?? ((UnaryExpression)field.Body).Operand as MemberExpression).Member.Name;
        }
        #region resources
        static MethodInfo s_resourceUrlMethod = null;
        public static string GetResourceUrl<TAssemblyObject>(string resourcePath)
        {
            if (s_resourceUrlMethod == null)
            {
                var methodCandidates = typeof(AssemblyResourceLoader).GetMember("GetWebResourceUrlInternal", BindingFlags.NonPublic | BindingFlags.Static).ToList();
                foreach (var methodCandidate in methodCandidates)
                {
                    var method = methodCandidate as MethodInfo;
                    if (method == null || method.GetParameters().Length != 5) continue;
                    s_resourceUrlMethod = method;
                    break;
                }
            }
            return string.Format("{0}", s_resourceUrlMethod.Invoke(
                        null,
                        new object[] { typeof(TAssemblyObject).Assembly, resourcePath, false, false, null })
                    );
        }
        #endregion
    }
}
