using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NewStreamSupporter.Helpers
{
    public static class ModelExtensions
    {
        // Taken from https://stackoverflow.com/questions/68197068/asp-net-core-razor-pages-want-displayattribute-description-to-display-as-title
        public static string? GetDescription<T>(string propertyName) where T : class
        {
            MemberInfo? memberInfo = typeof(T).GetProperty(propertyName);
            if (memberInfo == null)
            {
                return null;
            }

            return memberInfo.GetCustomAttribute<DisplayAttribute>()?.GetDescription();
        }

        public static string? GetMinimum<T>(string propertyName) where T : class
        {
            MemberInfo? memberInfo = typeof(T).GetProperty(propertyName);
            if (memberInfo == null)
            {
                return null;
            }

            return memberInfo.GetCustomAttribute<RangeAttribute>()?.Minimum.ToString();
        }

        public static string? GetMaximum<T>(string propertyName) where T : class
        {
            MemberInfo? memberInfo = typeof(T).GetProperty(propertyName);
            if (memberInfo == null)
            {
                return null;
            }

            return memberInfo.GetCustomAttribute<RangeAttribute>()?.Maximum.ToString();
        }
    }
}