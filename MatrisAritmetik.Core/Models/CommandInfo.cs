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

        public string[] param_names;

        public int[] required_params;

        public string returns;

        public string function_template;

        public string function_template_filled;

        public string service;

        public CommandInfo()
        {

        }

        public string MinimalFormat()
        {
            string reqparams = "";
            List<int> paraminds = new List<int>(required_params);
            for (int i = 0; i < param_types.Length; i++)
            {
                if (paraminds.Contains(i))
                {
                    reqparams += param_names[i] + ":" + param_types[i];
                    if (i != param_types.Length - 1)
                    {
                        if (paraminds.Contains(i + 1))
                            reqparams += ", ";
                    }
                }

            }
            return reqparams;
        }

        public string Info()
        {

            return "Fonksiyon(Tam): " + function_template_filled  + @"
        Açıklama: " + description + @"
        Alternatif: """ + string.Join(",", alias_list) + @""""+ @"
        Gerekli Minimal Format: !"+ function + "(" + MinimalFormat() + ")"+ @"
        Örnek: !" + function + "(" + string.Join(",", param_types) + ")";
        
        }
    }
}
