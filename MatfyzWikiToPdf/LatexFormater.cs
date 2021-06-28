using MatfyzWikiToPdf.Conditions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatfyzWikiToPdf
{
    public static class LatexFormater
    {
        /// <summary>
        /// Odstrani chyby z latex textu a prida nadpisy.
        /// </summary>
        /// <param name="text"></param>
        public static string ClearTex(string text, string pageName, ConditionList conditions)
        {
            //zmeneni '\emph' na '\textit', \emph nefungovalo a misto kurzivy delalo podtrhnuti
            text = text.Replace(@"\emph", @"\textit");
            
            //vytahnuti prostredku textu (bez hlavicky a konce) a vlozeni zpet
            string beginDoc = @"\begin{document}";
            int startIndex = text.IndexOf(beginDoc);

            //substring: delka hlavicky + delka '\begin{document}' (startovni 'tag' pro text) + 4 entery,
            //delka souboru - delka hlavicky - delka '\begin{document}' (startovni 'tag' pro text) - 
            //'\end{document}' (koncovy 'tag' pro text) - 2 entery - 8 znaku
            string endDoc = @"\end{document}";
            text = text.Substring(startIndex + beginDoc.Length + 4, 
                text.Length - startIndex - beginDoc.Length - endDoc.Length - 10);

            text = text.Replace(@"\toprule" + Environment.NewLine + @"& \\" + Environment.NewLine, "");
            text = text.Replace(@"\toprule" + Environment.NewLine, "");
            text = text.Replace(@"\midrule" + Environment.NewLine, "");
            text = text.Replace(@"\bottomrule" + Environment.NewLine, "");

            //vyhodnoceni podminek a vraceni zpracovaneho textu
            text = conditions.Run("latexClearText", pageName.Trim(), text);

            //upraveni tabulek

            //vytvoreni prefixu - \section nebo \subsection, podle typu kapitoly
            string prefix = "\\subsection";
            if (!pageName.StartsWith(' '))
            {
                prefix = "\\section";
            }

            //vraceni s prefixem na zacatku
            return prefix + "{" + pageName.Replace("_", " ").Trim() + "}\n" + text;
        }
    }
}
