using System;
using System.Collections.Generic;
using System.Text;

namespace SubtitlesTranslator.App.WPF.Lib
{
    public static class LanguageOptions
    {
        public static Dictionary<string, string> Types
        {
            get
            {
                if (_types == null)
                {
                    _types = new Dictionary<string, string>
                    {
                        { "EN", "en" },
                        { "ESP", "es" }
                    };

                }
                return _types;
            }
        }
        private static Dictionary<string, string> _types;
    }
}
