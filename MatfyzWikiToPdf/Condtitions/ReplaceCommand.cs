using System;
using System.Collections.Generic;
using System.Text;

namespace MatfyzWikiToPdf.Condtitions
{
    public class ReplaceCommand : CommandBase
    {
        /// <summary>
        /// Text ktery se zmeni.
        /// </summary>
        public string From;

        /// <summary>
        /// Text do ktereho se to zmeni.
        /// </summary>
        public string To;

        /// <summary>
        /// Text od ktereho se to zmeni. Tento text take bude zmenen.
        /// </summary>
        public string StartString;

        /// <summary>
        /// Provede prikaz a vrati zpracovany text z parametru.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public override string Run(string text)
        {
            //pokud command obsahuje StartString
            if (!String.IsNullOrEmpty(StartString))
            {
                int i = text.IndexOf(StartString);

                //pokud je v textu StartString
                if (i != -1)
                {
                    //vraceni textu s textem nahrazenym od StartString do To
                    return text.Substring(0, i) + To;
                }
            }

            //nahrazeni From do To
            return text.Replace(From, To);
        }
    }
}
