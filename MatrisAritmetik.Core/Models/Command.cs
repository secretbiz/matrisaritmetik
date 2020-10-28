﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrisAritmetik.Core.Models
{
    /* Aritmetik, cebirsel veya objesel metodları çözümle ve işle
     * 
     *** Syntax:
     *      {ifade};[{ayarlar},...]
     * 
     *** İfadeler:
     * 
     *      fonksiyonlar: isimden önce "!" ile kullanılır, _builtInCmds.json içinde belirtilmiştir
     * 
     *      aritmetik operatörler:  + , - , * , / , .* , ./ , %
     * 
     *      karşılaştırma operatörleri: < , <= , > , >= , == , !=
     * 
     *      atama operatörleri:  = , += , -= , *= , /= , .*= , ./=
     *      
     *      bit-shift operatörleri: << , >> 
     *      
     *** Ayarlar:
     *
     ****   Parametresiz:
     *  
     *          noname            Çıktılarda isimleri gizle
     *          
     *          quiet             Çıktı gösterme
     *          
     *          TeX | tex         Çıktıyı TeX formatında gösterir (Diğer birçok ayarı yoksayar)
     *          
     *      // Ayardan önce "name:" veya "vals:" ile isim ve element bloğuna özel uygulanabilir
     *      // HTML tag attribute syntax'ine benzer çalışır
     ****   Parametreli: 
     *          
     *          color   {color}                     Font rengi
     *          
     *          background-color {color}            Arkaplan rengi
     *          
     *          border {width} {style} {color}      Sınır şekillendirme
     *          
     *          font-weight   {weight}              Font kalınlık ayarı
     *          
     *
     *** Ayarları kullanmadan:
     *      
     *      Örnek (A matrisini görüntüle):
     *      
     *      >>> A
     *      
     *      Örnek (B matrisinin LU ayrışımını L ve U adlı matrislere kaydet ve görüntüle):
     *      
     *      >>> L,U = !SVD(B)
     *      
     *      Örnek ( C matrisine 10 ekle ve 3 ile çarp, sonra D ile matris çarpımı yap ve sonucu görüntüle):
     *      
     *      >>> (C + 10) * 3 .* D
     *      
     *      
     *** Ayarları kullanarak:
     * 
     *      Örnek (A matrisi, sadece elementleri görüntüle):
     *  
     *      >>>  A ; noname
     * 
     * 
     *      Örnek (a1 ve b matris çarpımı, elementlerin arka plan gri, tüm font'lar kalın, element rengi mavi):
     * 
     *      >>>  a1 .* b ; vals:background-color gray ; font-weight bold ; vals:color blue 
     * 
     * 
     *      Örnek ( myMatrix'i tersi ile çarp, birim1 olarak kaydet, ama birim1 matrisini print'leme):
     * 
     *      >>>  birim1 = myMatrix./myMatrix ; quiet
     * 
     */
    public class Command
    {
        public string[] TermsToEvaluate;

        public string OriginalCommand = "";

        public string CleanedCommand = "";

        public string OriginalSettings = "";

        public Dictionary<string, string> NameSettings = new Dictionary<string, string>();

        public Dictionary<string, string> ValsSettings = new Dictionary<string, string>();

        private dynamic STATE = CommandState.IDLE;

        private string STATE_MESSAGE = "";

        private void SettingDecider(string settingname, string param)
        {
            if (settingname.Length > 5)
            {
                if (settingname.Substring(0, 5) == "name:")// add to name settings
                {
                    settingname = settingname.Replace("name:", "");

                    if (!NameSettings.ContainsKey(settingname))
                        NameSettings.Add(settingname, param);
                    else
                        NameSettings[settingname] = param;

                    return;
                }
                else if (settingname.Substring(0, 5) == "vals:")// add to vals settings
                {
                    settingname = settingname.Replace("vals:", "");

                    if (!ValsSettings.ContainsKey(settingname))
                        ValsSettings.Add(settingname, param);
                    else
                        ValsSettings[settingname] = param;

                    return;
                }
            }
            
            if (!NameSettings.ContainsKey(settingname))
                NameSettings.Add(settingname, param);
            else
                NameSettings[settingname] = param;

            if (!ValsSettings.ContainsKey(settingname))
                ValsSettings.Add(settingname, param);
            else
                ValsSettings[settingname] = param;
        }

        public Command(string cmd)
        {
            OriginalCommand = cmd;

            /* Check if custom settings given */
            TermsToEvaluate = OriginalCommand.Split(";", StringSplitOptions.RemoveEmptyEntries);

            if (TermsToEvaluate.Length == 0)
                return;

            // Settings given
            if (TermsToEvaluate.Length > 1)
            {
                string[] currentsetting;

                for(int i = 1 ; i < TermsToEvaluate.Length ; i++)
                {
                    currentsetting = TermsToEvaluate[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    switch(currentsetting.Length)
                    {
                        case 0: // Bad setting given
                            { 
                                break;
                            }
                        case 1: // Setting without parameters
                            {
                                SettingDecider(currentsetting[0], "");
                                break;
                            }
                        case 2: // Setting with a single parameter
                            {
                                SettingDecider(currentsetting[0], currentsetting[1]);
                                break;
                            }
                        default: // Setting with a multiple parameters combined
                            {
                                // ex:
                                //      from    string "border 2px solid black" 
                                //      to      Key-Value {"border":"2px solid black"}
                                string combinedsetting = "";
                                for(int j=1; j<currentsetting.Length; j++)
                                { combinedsetting += " " + currentsetting[j]; }

                                SettingDecider(currentsetting[0], combinedsetting);

                                break;
                            }
                    }
                }
            }

            // Clean the actual command
            // Remove whitespace
            CleanedCommand = String.Join("", TermsToEvaluate[0].Split(" ", StringSplitOptions.RemoveEmptyEntries));

            // At this point command should be ready to be examined and settings should all be done

        }

        public dynamic EvaluateCommand()
        {
            switch (STATE)
            {
                // Komut ilk defa işlenmekte
                case CommandState.IDLE:
                    {
                        STATE = CommandState.UNAVAILABLE;

                        break;
                    }
                // Komut işlenmekte veya hatalı
                case CommandState.UNAVAILABLE:
                    {
                        Console.WriteLine("Komut işleme hatası-> \nOriginal:" + OriginalCommand + "\nCleaned:" + CleanedCommand);
                        break;
                    }
                // Komut zaten işlenmiş
                default:
                    {
                        Console.WriteLine("Komut zaten işlenmiş. State: " + STATE + " Extra message: " + STATE_MESSAGE);
                        break;
                    }

            }

            return STATE;
        }
    }

}
