using System;
using System.Text;

namespace MatfyzWikiToPdf.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Funkce na odstraneni diakritiky.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(string s)
        {
            // oddělení znaků od modifikátorů (háčků, čárek, atd.)
            s = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder(s.Length);

            for (int i = 0; i < s.Length; i++)
            {
                // do řetězce přidá všechny znaky kromě modifikátorů
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(s[i]) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(s[i]);
                }
            }

            // vrátí řetězec bez diakritiky
            return sb.ToString();
        }
    }
}
