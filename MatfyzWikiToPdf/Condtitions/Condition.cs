using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MatfyzWikiToPdf.Condtitions;

namespace MatfyzWikiToPdf.Conditions
{
    public class Condition
    {
        public string Name { get; }
        public string Type { get;  }
        public string PageName { get; }
        public CommandBaseList Commands { get; }

        public Condition(string type, string pageName, string name)
        {
            Commands = new CommandBaseList();

            Type = type;
            PageName = pageName;
            Name = name;
        }

        /// <summary>
        /// Provede vsechny commandy.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Run(string text)
        {
            //proiterovani a provedeni commandu
            foreach (CommandBase command in Commands)
            {
                text = command.Run(text);
            }

            return text;
        }
    }

    public class ConditionList : List<Condition>
    {
        public string Run(string conditionType, string pageName, string text)
        {
            string processingText = text;

            foreach (Condition c in this.Where(c => c.Type == conditionType && c.PageName == pageName))
            {
                processingText = c.Run(processingText);
            }

            return processingText;
        }
    }
}
