using System.Linq;
using System.Text.RegularExpressions;

namespace NAPS2.Localization
{
    public static class Rules
    {
        private static readonly Regex SuffixRegex = new Regex(@"[:.]+$");
        private static readonly Regex HotKeyRegex = new Regex(@"&(\w)");
        private static readonly Regex TextPropRegex = new Regex(@"(Text|Items\d*)$");

        public static bool IsTranslatable(bool winForms, string prop, ref string original, out string prefix, out string suffix)
        {
            prefix = "";
            suffix = "";
            if (prop == null || original == null || !original.Any(char.IsLetter))
            {
                return false;
            }
            var match = SuffixRegex.Match(original);
            if (match.Success)
            {
                suffix = match.Value;
                original = original.Substring(0, match.Index);
            }

            if (!winForms) return true;
            if (!TextPropRegex.IsMatch(prop)) return false;

            var hotKeyMatch = HotKeyRegex.Match(original);
            if (!hotKeyMatch.Success) return true;
            prefix = "&";
            original = HotKeyRegex.Replace(original, m => m.Groups[1].Value);
            return true;
        }
    }
}
