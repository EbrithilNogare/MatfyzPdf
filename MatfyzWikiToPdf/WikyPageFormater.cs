using System;
using System.Collections.Generic;
using System.Text;

namespace MatfyzWikiToPdf
{
    public static class WikyPageFormater
    {
        /// <summary>
        /// Zpracuje obsah stranky (vrati pouze text, zbytek smaze - hlavicku, konec, menicka)
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string ClearHtml(string html)
        {
            // trim page content
            int startPage = html.IndexOf("<page>");
            int endPage = html.IndexOf("</page>", startPage);
            string text = html.Substring(startPage + "<page>".Length, endPage - startPage);

            // find and trim header
            int endHead = text.IndexOf("}}");
            text = text.Substring(endHead + 4);

            // remove tail
            int startBottom = text.IndexOf("<sha1>");
            text = text.Substring(0, startBottom);

            //odstraneni textu mezi divem nebo spanem, pokud maji tridu 'noprint'
            text = RemoveTextBettweenStrings(text, "&lt;div class=\"noprint\"&gt;", "&lt;/div&gt;");
            text = RemoveTextBettweenStrings(text, "&lt;span class=\"noprint\"&gt;", "&lt;/span&gt;");

            // remove Table of contents
            if (text.Contains("====Podkapitoly"))
                text = text.Substring(0, text.IndexOf("====Podkapitoly") - 1);
            if (text.Contains("==== Podkapitoly"))
                text = text.Substring(0, text.IndexOf("==== Podkapitoly") - 1);

            // remove double span image in table
            string colspanEntity = "\n|colspan=\"2\"";
            text = RemoveTextBettweenStrings(text, colspanEntity, "]]");

            // remove all xml tags
            while (text.Contains("&lt"))
            {
                int tagStart = text.IndexOf("&lt;");
                int tagEnd = text.IndexOf("&gt;", tagStart);
                text = text.Remove(tagStart, tagEnd - tagStart + "&gt;".Length);
            }

            return text;
        }

        /// <summary>
        /// Vymaze text z mezi mezi krajnimi texty z parametru, texty z parametru odstrani taky.
        /// Pokud jsou texty ve spatnem poradi nebo tam nejsou, tak se nic nestane.
        /// Pokud text obsahuje vyraz vycektrat, tak se odstrani vsechny.
        /// </summary>
        /// <param name="startText"></param>
        /// <param name="endText"></param>
        /// <returns>Text s odstranenou casti mezi parametry.</returns>
        private static string RemoveTextBettweenStrings(string text, string startText, string endText)
        {
            int startIndex;
            int endIndex;

            while ((startIndex = text.IndexOf(startText)) != -1 && (endIndex = text.IndexOf(endText, startIndex)) != -1)
                text = text.Remove(startIndex, endIndex - startIndex + endText.Length);

            return text;
        }
    }
}
