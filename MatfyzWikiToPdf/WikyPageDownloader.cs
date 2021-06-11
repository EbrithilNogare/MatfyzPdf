using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MatfyzWikiToPdf
{
    public class WikyPageDownloader
    {
        /// <summary>
        /// Url stranky ke stazeni.
        /// </summary>
        private string url;

        private HttpClient client;
        
        /// <summary>
        /// Kesovani HttpClientu.
        /// </summary>
        public HttpClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new HttpClient();
                }

                return client;
            }
        }

        public WikyPageDownloader(string url)
        {
            this.url = url;
        }

        /// <summary>
        /// Asynchroni funkce ktera nacte html stranky, podle jmena stranky v parametru a vratiho ho ve stringu.
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public async Task<string> LoadPageHtml(string pageName)
        {
            //hodnoty do postu
            var values = new Dictionary<string, string>
            {
                { "catname", "Exportováno" },
                //musi to tady byt dvakrat, jinak se nacetlo jenom menicko
                { "pages", $"pages={pageName}\n{pageName}"},
                { "curonly", "1" },
                { "wpDownload", "1" },
                { "wpEditToken", @"=+\" },
                { "title", "Speciální:Exportovat+stránky" }
            };

            //prevedeni dat na HttpContent
            string result;
            var content = new FormUrlEncodedContent(values);

            //post na web a nacteni vysledku do result
            using (HttpClient Client = new HttpClient())
            {
                var response = await Client.PostAsync(url, content);
                result = response.Content.ReadAsStringAsync().Result;
            }

            return result;
        }
    }
}
