using System;
using System.Xml;

namespace MatfyzWikiToPdf.Helpers
{
    public static class XmlHelper
    {
        public static int DefaultInt = 0;
        public static double DefaultDouble = 0;
        public static string DefaultString = "";
        public static DateTime DefaultDateTime = new DateTime();

        #region GetType
        /// <summary>
        /// Vrati int hodnotu z nodu, pokdu tam neni, tak vrati DefaultInt.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetInt(XmlNode node, string name)
        {
            //nacteni
            var nText = node.SelectSingleNode(name);
            if (nText == null)
            {
                return DefaultInt;
            }

            string text = nText.InnerText;

            //kontrola jestli tam neco je
            if (String.IsNullOrEmpty(text))
            {
                return DefaultInt;
            }

            //parsovani
            int value;
            if (Int32.TryParse(text, out value))
            {
                return value;
            }

            return DefaultInt;
        }

        /// <summary>
        /// Vrati double hodnotu z nodu, pokdu tam neni, tak vrati DefaultDouble.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static double GetDouble(XmlNode node, string name)
        {
            //nacteni
            var nText = node.SelectSingleNode(name);
            if (nText == null)
            {
                return DefaultDouble;
            }

            string text = nText.InnerText;

            //kontrola jestli tam neco je
            if (String.IsNullOrEmpty(text))
            {
                return DefaultInt;
            }

            //parsovani
            double value;
            if (Double.TryParse(text, out value))
            {
                return value;
            }

            return DefaultDouble;
        }

        /// <summary>
        /// Vrati string hodnotu z nodu, pokud tam neni, tak vrati DefaultString.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetString(XmlNode node, string name)
        {
            //nacteni
            var nText = node.SelectSingleNode(name);
            if (nText == null)
            {
                return DefaultString;
            }

            string text = nText.InnerText;

            //kontrola jestli tam neco je
            if (String.IsNullOrEmpty(text))
            {
                return DefaultString;
            }

            return text;
        }

        /// <summary>
        /// Vrati int? hodnotu z nodu,  pokud tam neni, tak vrati DefaultInt
        /// a pokud je v promenne null, tak zapise do souboru 'null'.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static int? GetNullInt(XmlNode node, string name)
        {
            //nacteni
            var nText = node.SelectSingleNode(name);
            if (nText == null)
            {
                return DefaultInt;
            }

            string text = nText.InnerText;

            if (text == "null")
            {
                return null;
            }

            //parsovani
            int value;
            if (Int32.TryParse(text, out value))
            {
                return value;
            }

            return DefaultInt;
        }

        /// <summary>
        /// Prevede int? do stringu, pokud je v pomenne null tak vrati 'null'.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        internal static string ToNullInt(int? number)
        {
            if (number == null)
            {
                return "null";
            }

            return number.Value.ToString();
        }

        /// <summary>
        ///  Vrati dateTime hodnotu z nodu, pokud tam neni, tak vrati DefaultDateTime.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        internal static DateTime GetDateTime(XmlNode node, string name)
        {
            //nacteni
            var nText = node.SelectSingleNode(name);
            if (nText == null)
            {
                return DefaultDateTime;
            }

            string text = nText.InnerText;

            //kontrola jestli tam neco je
            if (String.IsNullOrEmpty(text))
            {
                return DefaultDateTime;
            }

            //parsovani
            DateTime value;
            if (DateTime.TryParse(text, out value))
            {
                return value;
            }

            return DefaultDateTime;
        }
        #endregion

        #region GetTypeAtributeOrNode
        /// <summary>
        /// Vrati string hodnotu atributu podle jmena.
        /// Pokud je atribut nullOrEmpty, tak vrati hodnotu nodu.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="property"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetStringAttributeOrNode(XmlNode n, string name)
        {
            string value = n.Attributes[name]?.Value;

            if (String.IsNullOrEmpty(value))
            {
                return GetString(n, name);
            }

            return value;
        }
        #endregion
    }
}
