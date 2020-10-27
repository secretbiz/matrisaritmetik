using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrisAritmetik.Core.Models
{
    /* Aritmetik, cebirsel veya objesel metodları çözümle ve işle
     * Syntax:
     *      {ifade};[{ayarlar},...]
     * 
     * Örnek (A matrisi, sadece elementleri görüntüle):
     *  
     * >>>  A ; noname
     * 
     * Örnek (a1 ve b matris çarpımı, arka plan gri, matris ismi kalın font, element rengi mavi):
     * 
     * >>>  a1 .* b ; background.color gray ; name.font.weight bold ; values.color blue 
     * 
     * Örnek ( myMatrix'i tersi ile çarp, birim1 olarak kaydet, ama birim1 matrisini print'leme):
     * 
     * >>>  birim1 = myMatrix./myMatrix ; quiet
     * 
     */
    public class Command
    {
        public string[] TermsToEvaluate;

        public Dictionary<string, string> Settings;

        public Command(string cmd)
        {
            string[] temp = cmd.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            
            string cleancmd = "";
            foreach(string val in temp)
            { cleancmd += val; }

            TermsToEvaluate = cleancmd.Split(";", StringSplitOptions.RemoveEmptyEntries);
            
            // Settings given
            if(TermsToEvaluate.Length > 1)
            {

            }

            // Evaluate expressions
        }

    }
}
