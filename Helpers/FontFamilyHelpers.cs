using NewStreamSupporter.Models;

namespace NewStreamSupporter.Helpers
{
    public static class FontFamilyHelpers
    {
        private static IReadOnlyCollection<FontFamily>? _types = null;
        private static readonly object _lock = new();


        public static IReadOnlyCollection<FontFamily> FontFamilies
        {
            get
            {
                if (_types == null)
                {
                    //zámek, aby nedošlo k vzniku několika těchto polí
                    lock (_lock)
                    {
                        _types ??= Array.AsReadOnly((FontFamily[])Enum.GetValues(typeof(FontFamily)));
                    }
                }
                return _types;
            }
        }

        public static string GetFontFamilyValues(this FontFamily fontFamily)
            => fontFamily switch
            {
                FontFamily.Arial => "Arial, Helvetica, sans-serif",
                FontFamily.BrushScriptMT => "\"Brush Script MT\", cursive",
                FontFamily.CourierNew => "\"Courier New\", Courier, monospace",
                FontFamily.Garamond => "Garamond, serif",
                FontFamily.Georgia => "Georgia, serif",
                FontFamily.Tahoma => "Tahoma, Verdana, sans-serif",
                FontFamily.TimesNewRoman => "\"Times New Roman\", Times, serif",
                FontFamily.TrebuchetMS => "\"Trebuchet MS\", Helvetica, sans-serif",
                FontFamily.Verdana => "Verdana, sans-serif",
                _ => throw new NotImplementedException("The specified font family is not implemented")
            };
    }
}
