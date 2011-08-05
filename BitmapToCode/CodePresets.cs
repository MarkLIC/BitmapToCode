using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitmapToCode
{
    internal static class CodePresets
    {
        public const string None = "None";
        private static readonly Dictionary<string, string> presets = new Dictionary<string, string>();

        // TODO: Add presets as necessary.
        public static IEnumerable<string> PresetKeys
        {
            get
            {
                yield return None;
            }
        }

        public static string GetPreset(string key)
        {
            return presets.ContainsKey(key) ? presets[key] : null;
        }
    }
}
