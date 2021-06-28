using System;
using System.Collections.Generic;
using System.Text;

namespace MatfyzWikiToPdf.Condtitions
{
    public class CommandBase
    {
        public virtual string Run(string text)
        {
            return "";
        }
    }

    public class CommandBaseList : List<CommandBase>
    { 
    }
}
