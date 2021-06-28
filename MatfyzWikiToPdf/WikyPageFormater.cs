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
            //start a konec textu
            int start = html.IndexOf("<page>");
            int end = html.IndexOf("</page>");

            //vyseknuti texu
            string text = html.Substring(start + 7, end - start);

            //najiti konce hlavicky a vyseknuti textu od konce hlavicky
            int endHead = text.IndexOf("}}");
            string text2 = text.Substring(endHead + 4);

            //smazani konce (koncove tagy)
            int startBottom = text2.IndexOf("<sha1>");
            string text3 = text2.Substring(0, startBottom);

            //odstraneni textu mezi divem nebo spanem, pokud maji tridu 'noprint'
            text3 = RemoveTextBettweenStrings(text3, "&lt;div class=\"noprint\"&gt;", "&lt;/div&gt;");
            text3 = RemoveTextBettweenStrings(text3, "&lt;span class=\"noprint\"&gt;", "&lt;/span&gt;");

            string text4 = text3;
            //pokud je tam menicko (to zacina ====), tak se smaze
            if (text3.Contains("===="))
            {
                int startMenu = text3.IndexOf("====");
                text4 = text3.Substring(0, startMenu - 1); //-1 => jeden znak ne => prvni '='
            }

            //smazani vsech tagu, ty by se neprelozili do latexu
            while (text4.Contains("&lt"))
            {
                int tagStart = text4.IndexOf("&lt");
                int tagEnd = text4.IndexOf("&gt");
                text4 = text4.Remove(tagStart, tagEnd - tagStart + 4); //+4 => 4 znaky => '&gt '
            }

            return text4;
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
            int startIndex = 0;
            int endIndex = 1;

            while (text.Contains(startText) && text.Contains(endText) && startIndex < endIndex)
            {
                //vypocitani zacatecniho a koncoveho indexu krajnich textu
                startIndex = text.IndexOf(startText);
                endIndex = text.IndexOf(endText);

                //pokud tam ty texty jsou ve spravnem poradi, tak se smaze text mezi nimi
                if (startIndex < endIndex)
                {
                    //odstraneni textu mezi hodnotamy z parametru
                    text = text.Remove(startIndex, endIndex - startIndex + endText.Length);
                }
            }

            return text;
        }
    }
}
