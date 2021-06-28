using System;
using System.Xml;
using MatfyzWikiToPdf.Condtitions;
using MatfyzWikiToPdf.Helpers;

namespace MatfyzWikiToPdf.Conditions
{
    public static class ConditionParser
    {
        public static ConditionList LoadConditions(string xml)
        {
            ConditionList list = new ConditionList();

            //vytvoreni xml documentu a naplneni xml
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            //proiterovani condition nodu
            var nodes = doc.SelectNodes("/root/conditions/condition");
            foreach (XmlNode node in nodes)
            {
                //nacteni atributu conditiony
                Condition c = new Condition(node.Attributes["type"].Value, node.Attributes["pageName"].Value, node.Attributes["name"].Value);

                //nacteni child nodu
                foreach(XmlNode n in node.ChildNodes)
                {
                    //vytvoreni commandu
                    switch (n.Name)
                    {
                        //vytvoreni replace commandu
                        case "replace":
                            c.Commands.Add(LoadReplaceCommand(n));
                            break;
                    }
                }

                list.Add(c);
            }

            return list;
        }

        #region Load commands
        /// <summary>
        /// Vytvori a vrati replaceCommand z xmlNodu.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static CommandBase LoadReplaceCommand(XmlNode n)
        {
            ReplaceCommand c = new ReplaceCommand();

            //nacteni hodnoty ze 'StartString'
            c.StartString = n.Attributes["startString"]?.Value;

            //pokud neni 'startString' tak se nacte 'from'
            if (String.IsNullOrEmpty(c.StartString))
            {
                c.From = XmlHelper.GetStringAttributeOrNode(n, "from");
            }

            //nacteni hodnoty z 'to' atributu nebo nodu
            c.To = XmlHelper.GetStringAttributeOrNode(n, "to");

            return c;
        }

        
        #endregion
    }
}
