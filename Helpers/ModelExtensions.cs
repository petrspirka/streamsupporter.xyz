using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NewStreamSupporter.Helpers
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Získá popis vlastnosti modelu
        /// </summary>
        /// <typeparam name="T">Typ pro který se má zjistit popis vlastnosti</typeparam>
        /// <param name="propertyName">Jméno vlastnosti, pro kterou se popis zjišťuje</param>
        /// <returns>Popis vlastnosti pokud existuje, jinak null</returns>
        public static string? GetDescription<T>(string propertyName) where T : class
        {
            // Taken from https://stackoverflow.com/questions/68197068/asp-net-core-razor-pages-want-displayattribute-description-to-display-as-title
            MemberInfo? memberInfo = typeof(T).GetProperty(propertyName);
            if (memberInfo == null)
            {
                return null;
            }

            return memberInfo.GetCustomAttribute<DisplayAttribute>()?.GetDescription();
        }

        /// <summary>
        /// Získá minimální hodnotu modelu pro danou vlastnost
        /// </summary>
        /// <typeparam name="T">Typ, pro který se zjišťuje minimální hodnota</typeparam>
        /// <param name="propertyName">Název vlastnosti, pro který se hodnota zjišťuje</param>
        /// <returns>Minimální hodnotu jako řetězec pokud existuje, jinak null</returns>
        public static string? GetMinimum<T>(string propertyName) where T : class
        {
            MemberInfo? memberInfo = typeof(T).GetProperty(propertyName);
            if (memberInfo == null)
            {
                return null;
            }

            return memberInfo.GetCustomAttribute<RangeAttribute>()?.Minimum.ToString();
        }

        /// <summary>
        /// Získá maximální hodnotu modelu pro danou vlastnost
        /// </summary>
        /// <typeparam name="T">Typ, pro který se zjišťuje maximální hodnota</typeparam>
        /// <param name="propertyName">Název vlastnosti, pro který se hodnota zjišťuje</param>
        /// <returns>Maximální hodnotu jako řetězec pokud existuje, jinak null</returns>
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