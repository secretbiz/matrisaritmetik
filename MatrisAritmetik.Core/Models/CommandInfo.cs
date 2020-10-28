using System;
using System.Collections.Generic;
using System.Text;

namespace MatrisAritmetik.Core.Models
{
    public class CommandInfo
    {
        public string fullname;

        public string description;

        public List<string> alias_list;

        public string function;

        public string[] param_types;

        public int[] required_params;

        public string function_template;

        public string function_template_filled;

        public CommandInfo()
        {

        }

        public string Info()
        {
            return "Function: !" + function + "(" + string.Join(",", param_types) + ")" + @"
        Description: " + description + @"
        Alternative: """ + string.Join(",", alias_list) + @""""+ @"
        Example: " + function_template_filled;
        
        }
    }
}
