using System;
using System.Collections.Generic;
using System.Text;

namespace MatrisAritmetik.Core.Models
{
    public class CommandLabel
    {
        public string Label = "Genel";

        public CommandInfo[] Functions;

        public CommandLabel()
        {
        }

        public CommandLabel(string label,CommandInfo[] cmds)
        {
            Label = label;
            Functions = cmds;
        }
    }
}
