using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MatfyzWikiToPdf.Helpers;
using System.Diagnostics;
using MatfyzWikiToPdf.Conditions;

namespace MatfyzWikiToPdf
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                List<string> pages = new List<string>();
                string[] pagesNames = DataLayer.LoadPagesNames();
                ConditionList conditions = DataLayer.LoadConditions();
                string url = DataLayer.LoadUrl();
                WikyPageDownloader down = new WikyPageDownloader(url);


                Console.WriteLine("Nacitani html z wikipedie.");
                foreach (var pageName in pagesNames)
                    pages.Add(WikyPageFormater.ClearHtml(await down.LoadPageHtml(pageName.Trim())));


                Console.WriteLine("Ukladani dat do txt souboru.");
                for (int i = 0; i < pages.Count; i++)
                    DataLayer.WriteNewText(pagesNames[i].Trim(), pages[i]);


                Console.WriteLine("Konvertovani souboru z txt do latex.");
                foreach (var pageName in pagesNames)
                {
                    string pageNameTrimmed = StringHelper.RemoveDiacritics(pageName.Trim());

                    ProcessStartInfo psi = new ProcessStartInfo("pandoc", $"{pageNameTrimmed}.txt -f mediawiki -t latex --standalone -o {pageNameTrimmed}.tex");
                    psi.WorkingDirectory = DataLayer.Path;

                    var process = Process.Start(psi);
                    process.WaitForExit();
                }


//                Console.WriteLine("Mazani .txt souboru.");
//                for (int i = 0; i < pagesNames.Length; i++)
//                    DataLayer.DeleteFile(StringHelper.RemoveDiacritics(pagesNames[i].Trim()) + ".txt");


                Console.WriteLine("Odstraneni chyb, hlavicky a konce .tex souboru.");
                for (int i = 0; i < pagesNames.Length; i++)
                    DataLayer.ClearTex(pagesNames[i], conditions);


                Console.WriteLine("Generovani hlavni stranky (Matfyzacka kucharka.tex).");
                DataLayer.GenerateMainPage(pagesNames);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}