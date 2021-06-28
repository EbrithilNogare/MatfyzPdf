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
                //nacteni jmen stranek a ulozeni do pole
                Console.WriteLine("Nacitani jmen stranek.");
                string[] pagesNames = DataLayer.LoadPagesNames();

                //nacteni podminek a ulozeni do listu
                Console.WriteLine("Nacitani podminek.");
                ConditionList conditions = DataLayer.LoadConditions();

                //nacteni url
                Console.WriteLine("Nacitani url.");
                string url = DataLayer.LoadUrl();

                //list stringu s html stranek
                List<string> pages = new List<string>();
                WikyPageDownloader down = new WikyPageDownloader(url);

                //nacteni html stranek a ulozeni do listu
                Console.WriteLine("Nacitani html z wikipedie.");
                foreach (string pageName in pagesNames)
                {
                    string html = await down.LoadPageHtml(pageName.Trim());
                    pages.Add(WikyPageFormater.ClearHtml(html));
                }

                //ulozeni do txt souboru
                Console.WriteLine("Ukladani dat do txt souboru.");
                for (int i = 0; i < pages.Count; i++)
                {
                    DataLayer.WriteText(pagesNames[i].Trim(), pages[i]);
                }

                //zkonvertovani souboru z txt do latex
                Console.WriteLine("Konvertovani souboru z txt do latex.");
                for (int i = 0; i < pagesNames.Length; i++)
                {
                    //vytvoreni nazvu
                    string pageName = StringHelper.RemoveDiacritics(pagesNames[i].Trim());

                    //vytvoreni prikazu, ktery zkonvertuje txt to latexu
                    ProcessStartInfo psi = new ProcessStartInfo("pandoc", $"{pageName}.txt -f mediawiki -t latex --standalone -o {pageName}.tex");
                    psi.WorkingDirectory = DataLayer.Path;

                    //zapnuti procesu a pockani nez dobehne
                    var process = Process.Start(psi);
                    process.WaitForExit();
                }

                //smazani .txt souboru
                Console.WriteLine("Mazani .txt souboru.");
                for (int i = 0; i < pagesNames.Length; i++)
                {
                    DataLayer.DeleteFile(StringHelper.RemoveDiacritics(pagesNames[i].Trim()) + ".txt");
                }

                //odstraneni chyb, hlavicky a konce .tex souboru
                Console.WriteLine("Odstraneni chyb, hlavicky a konce .tex souboru.");
                for (int i = 0; i < pagesNames.Length; i++)
                {
                    DataLayer.ClearTex(pagesNames[i], conditions);
                }

                //generovani hlavni stranky (Matfyzacka kucharka.tex)
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