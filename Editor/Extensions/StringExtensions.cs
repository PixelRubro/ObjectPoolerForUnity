using System.Text.RegularExpressions;
using System;

namespace PixelRouge.ObjectPooler.Extensions
{
    public static class StringExtensions 
    {
        // Author: Vermillion Vanguard Studio
        public static string ToTitleCase(this string self)
        {
            // Add spaces.
            string output = Regex.Replace(self, @"[A-Z]", " $0");

            output = output.RemoveFirstUnderscore();
            output = output.ReplaceUnderscoresWithSpaces();
            return output.CapitalizeFirstLetter();
        }

        // Author: Vermillion Vanguard Studio
        public static string ToSentenceCase(this string self)
        {
            // Add spaces and uncapitalize the following letter.
            string output = Regex.Replace(self, @"[A-Z]", " $0").ToLower();

            output = output.RemoveFirstUnderscore();
            output = output.ReplaceUnderscoresWithSpaces();
            return output.CapitalizeFirstLetter();
        }

        // Author: Vermillion Vanguard Studio
        private static string CapitalizeFirstLetter(this string self)
        {
            if (self.Length < 2)
                throw new IndexOutOfRangeException();

            return char.ToUpperInvariant(self[0]) + self.Substring(1);
        }

        // Author: Vermillion Vanguard Studio
        private static string RemoveFirstUnderscore(this string self)
        {
            if (self.Length < 2)
                throw new IndexOutOfRangeException();

            if (self[0] == '_')
                return self.Substring(1);

            return self;
        }

        // Author: Vermillion Vanguard Studio
        private static string ReplaceUnderscoresWithSpaces(this string self)
        {
            return self.Replace('_', ' ');
        }
    }
}