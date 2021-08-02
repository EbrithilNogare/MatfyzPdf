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
        /// <summary>
        /// Cesta do slozky se souborama.
        /// </summary>
        public static string Path
        {
            get
            {
                string location = Assembly.GetExecutingAssembly().Location;
                return System.IO.Path.GetDirectoryName(location);
            }
        }

        /// <summary>
        /// Nacte url ze souboru Data.txt.
        /// </summary>
        /// <returns></returns>
        public static string LoadUrl()
        {
            string p = System.IO.Path.Combine(Path, "Data.txt");

            //kontrola existence souboru
            if (!File.Exists(p))
            {
                throw new ApplicationException("Soubor 'Data.txt' neexistuje. " + p);
            }

            //nacte textu a vraceni
            return File.ReadAllText(p);
        }

        /// <summary>
        /// Napise text do souboru.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        public static void WriteText(string fileName, string text)
        {
            //kontrola jestli je nazev souboru validni
            if (fileName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ApplicationException($"Neplatný název souboru '{fileName}'");
            }

            File.WriteAllText(System.IO.Path.Combine(Path, StringHelper.RemoveDiacritics(fileName) + ".txt"), text);
        }


        /// <summary>
        /// Napise text do noveho souboru (smaze puvodni).
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        public static void WriteNewText(string fileName, string text)
        {
            if (File.Exists(fileName))
                DeleteFile(fileName);
            WriteText(fileName, text);
        }

        /// <summary>
        /// Nacte jmena stranek ze souboru Pages.txt.
        /// </summary>
        /// <returns></returns>
        public static string[] LoadPagesNames()
        {
            //kontrola jestli soubor existuje
            if (!File.Exists(System.IO.Path.Combine(Path, "Pages.txt")))
            {
                throw new ApplicationException("Soubor s jménama stránek 'Pages.txt' neexistuje. " + Path);
            }

            //nacteni a vraceni textu
            return File.ReadAllLines(System.IO.Path.Combine(Path, "Pages.txt"));
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
            string p = System.IO.Path.Combine(Path, fileName);
            
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
            string p = System.IO.Path.Combine(Path, StringHelper.RemoveDiacritics(fileName.Trim()) + ".tex");

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
            File.WriteAllText(System.IO.Path.Combine(Path, "Matfyzacka kucharka.tex"), sb.ToString());
        }

        /// <summary>
        /// Nacte podminky ze souboru a vrati je v listu.
        /// </summary>
        /// <returns></returns>
        public static ConditionList LoadConditions()
        {
            string p = System.IO.Path.Combine(Path, "Conditions.xml");

            //kontrola existence souboru
            if (!File.Exists(p))
            {
                throw new ApplicationException("Soubor 'Conditions.xml' neexistuje. " + p);
            }

            return ConditionParser.LoadConditions(File.ReadAllText(p));
        }
    }
}
