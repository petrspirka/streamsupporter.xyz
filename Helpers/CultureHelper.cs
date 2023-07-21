using System.Globalization;

namespace NewStreamSupporter.Helpers
{
    public static class CultureHelper
    {
        /// <summary>
        /// Obsahuje validní jazyky aplikace.
        /// </summary>
        public static CultureInfo[] Cultures { get; set; } =
        {
            new CultureInfo("en-US"),
            //new CultureInfo("cs-CZ")
        };
    }
}
