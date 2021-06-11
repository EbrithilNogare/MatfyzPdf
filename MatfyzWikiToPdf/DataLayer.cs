using MatfyzWikiToPdf.Conditions;
using MatfyzWikiToPdf.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MatfyzWikiToPdf
{
    /// <summary>
    /// Datova vrstva na pristup k souborum.
    /// </summary>
    public static class DataLayer
    {
        private static string path = "C:\\Users\\kukub\\OneDrive\\Plocha\\pandoc";

        /// <summary>
        /// Nacte url ze souboru Data.txt.
        /// </summary>
        /// <returns></returns>
        public static string LoadUrl()
        {
            string p = Path.Combine(path, "Data.txt");

            //kontrola existence souboru
            if (!File.Exists(p))
            {
                throw new ApplicationException("Soubor 'Data.txt' neexistuje. " + p);
            }

            //nacte textu a vraceni
            return File.ReadAllText(p);
        }

        /// <summary>
        /// Cesta do slozky se souborama.
        /// </summary>
        /*private static string path
        {
            get
            {
                string location = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(location);
            }
        }*/

        /// <summary>
        /// Napise text do souboru.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        public static void WriteText(string fileName, string text)
        {
            //kontrola jestli je nazev souboru validni
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ApplicationException($"Neplatný název souboru '{fileName}'");
            }

            File.WriteAllText(Path.Combine(path, StringHelper.RemoveDiacritics(fileName) + ".txt"), text);
        }

        /// <summary>
        /// Nacte jmena stranek ze souboru Pages.txt.
        /// </summary>
        /// <returns></returns>
        public static string[] LoadPagesNames()
        {
            //kontrola jestli soubor existuje
            if (!File.Exists(Path.Combine(path, "Pages.txt")))
            {
                throw new ApplicationException("Soubor s jménama stránek 'Pages.txt' neexistuje. " + path);
            }

            //nacteni a vraceni textu
            return File.ReadAllLines(Path.Combine(path, "Pages.txt"));
        }

        public static string RemoveDiacritics(string s)
        {
            //oddelení znaku od modifikatoru (hacku, carek, atd.)
            s = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                //do retezce přidá vsechny znaky krome modifikatoru
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(s[i]) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(s[i]);
                }
            }

            //vrati retezec bez diakritiky
            return sb.ToString();
        }

        /// <summary>
        /// Smaze soubor, pokud existuje.
        /// </summary>
        /// <param name="fileName"></param>
        public static void DeleteFile(string fileName)
        {
            //vytvoreni cesty k souboru
            string p = Path.Combine(path, fileName);
            
            //pokud soubor existuje, tak bude smazan
            if (File.Exists(p))
            {
                File.Delete(p);
            }
        }

        /// <summary>
        /// Odstrani chyby v .tex souboru.
        /// </summary>
        /// <param name="fileName"></param>
        public static void ClearTex(string fileName, ConditionList conditions)
        {
            string p = Path.Combine(path, StringHelper.RemoveDiacritics(fileName.Trim()) + ".tex");

            //kontrola existence souboru
            if (!File.Exists(p))
            {
                throw new ApplicationException($"Soubor '{fileName.Trim()}' neexistuje. {p}");
            }

            //zpracovani textu
            string text = File.ReadAllText(p);
            File.WriteAllText(p, LatexFormater.ClearTex(text, fileName, conditions));
        }

        /// <summary>
        /// Vygeneruje hlavni stranku pdfka.
        /// </summary>
        /// <param name="pagesNames"></param>
        public static void GenerateMainPage(string[] pagesNames)
        {
            //pridani hlavicky do stringBuilderu
            StringBuilder sb = new StringBuilder(5000);
            sb.Append(@"\documentclass[a5paper]{article}

\usepackage{geometry}
\usepackage[utf8]{inputenc}
\usepackage{graphicx}
\usepackage[unicode]{hyperref}            %unicode pro nadpisy (labely)
\usepackage{tabularx}
\usepackage[english, czech]{babel}
\usepackage{relsize}
\usepackage{sectsty}
\usepackage{longtable}                    % kvuli seznamu ve slovniku pres dve stranky
\usepackage{ulem}                         % preskrtnuty text
\usepackage{lmodern}

\def\readyForPrint{1} % switch pro tisk [0:preview, 1:tisk]        %pro spravne nacitani fontu

\providecommand{\tightlist}{%
  \setlength{\itemsep}{0pt}\setlength{\parskip}{0pt}}

\input{commands}

\begin{document}

\input{titlePage}
\input{mainPage}
");

            //pridani stranek do menu
            for (int i = 0; i < pagesNames.Length; i++)
            {
                string pageName = pagesNames[i];

                //pokud je to hlavni kapitola (zacina mezerou) tak se tam vlozi odskoceni
                if (pagesNames[i].StartsWith(' '))
                {
                    sb.Append("   ");
                }
                
                sb.Append(@"\input{" + StringHelper.RemoveDiacritics(pageName.Trim()) + "}\n");
            }

            //pridani konce stranky
            sb.Append(@"\input{zaver}
\input{endPage}

\end{document}");

            //vypsani do souboru
            File.WriteAllText(Path.Combine(path, "Matfyzacka kucharka.tex"), sb.ToString());
        }

        /// <summary>
        /// Nacte podminky ze souboru a vrati je v listu.
        /// </summary>
        /// <returns></returns>
        public static ConditionList LoadConditions()
        {
            string p = Path.Combine(path, "Conditions.xml");

            //kontrola existence souboru
            if (!File.Exists(p))
            {
                throw new ApplicationException("Soubor 'Conditions.xml' neexistuje. " + p);
            }

            return ConditionParser.LoadConditions(File.ReadAllText(p));
        }
    }
}
