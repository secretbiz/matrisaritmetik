using System;
using System.Collections.Generic;
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
     *      aritmetik operatörler:  + , - , * , / , ^ , .^ , .* , ./ , %
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
     *      // Ayardan önce "cmd:" veya "out:" ile isim ve element bloğuna özel uygulanabilir
     *      // CSS stilleri gibi çalışır
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
     *      >>>  A ; cmd:quiet
     * 
     * 
     *      Örnek (a1 ve b matris çarpımı, elementlerin arka plan gri, tüm font'lar kalın, element rengi mavi):
     * 
     *      >>>  a1 .* b ; out:background-color gray ; font-weight bold ; out:color blue 
     * 
     * 
     *      Örnek ( myMatrix'i tersi ile çarp, birim1 olarak kaydet, ama birim1 matrisini print'leme):
     * 
     *      >>>  birim1 = myMatrix./myMatrix ; quiet
     * 
     */
    /// <summary>
    /// Command class which hold information about a command's current state, output etc.
    /// </summary>
    public class Command
    {
        #region Const Strings
        /// <summary>
        /// String to look for when applying settings to command row
        /// </summary>
        private const string ApplyCmd = "cmd:";
        /// <summary>
        /// String to look for when applying settings to output row
        /// </summary>
        private const string ApplyOut = "out:";
        #endregion

        #region Public Fields
        /// <summary>
        /// List of commands and settings to evaluate, first one hold the command, rest are settings
        /// </summary>
        public string[] TermsToEvaluate;
        /// <summary>
        /// Tokens of the command
        /// </summary>
        public List<Token> Tokens = new List<Token>();
        /// <summary>
        /// Output of the command, generally a string
        /// </summary>
        public dynamic Output = "";
        /// <summary>
        /// Given command, unprocessed
        /// </summary>
        public string OriginalCommand = "";
        /// <summary>
        /// Cleaned command, with expected spacing
        /// </summary>
        public string CleanedCommand = "";
        /// <summary>
        /// Given settings, unprocessed
        /// </summary>
        public string OriginalSettings = "";
        /// <summary>
        /// Dictionary of settings and their values to be applied to command row
        /// </summary>
        public Dictionary<string, string> NameSettings = new Dictionary<string, string>();
        /// <summary>
        /// Dictionary of settings and their values to be applied to output row
        /// </summary>
        public Dictionary<string, string> ValsSettings = new Dictionary<string, string>();
        #endregion

        #region Command State Fields and Properties
        private CommandState _state = CommandState.IDLE;
        /// <summary>
        /// Current <see cref="CommandState"/> of the command
        /// </summary>
        public CommandState STATE
        {
            get => _state;
            set
            {
                _state = value;
                STATEID = (int)value;
            }
        }
        /// <summary>
        /// Command's <see cref="CommandState"/> as an integer 
        /// </summary>
        public int STATEID = -1;
        /// <summary>
        /// Current message about the state of the command
        /// </summary>
        public string STATE_MESSAGE = "";
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a command with given state, used only in session variable setting process
        /// </summary>
        /// <param name="org">Original command string</param>
        /// <param name="nset">Setting dictionary for command row</param>
        /// <param name="vset">Setting dictionary for output row</param>
        /// <param name="stat">State of the command</param>
        /// <param name="statmsg">State message of the command</param>
        /// <param name="output">Output of the command</param>
        public Command(string org,
                       Dictionary<string, string> nset,
                       Dictionary<string, string> vset,
                       int stat,
                       string statmsg,
                       string output)
        {
            OriginalCommand = org;
            NameSettings = nset;
            ValsSettings = vset;
            STATE = (CommandState)stat;
            STATE_MESSAGE = statmsg;
            Output = output;
        }

        /// <summary>
        /// Create a command and get it ready for processing
        /// </summary>
        /// <param name="cmd">Command string</param>
        public Command(string cmd)
        {
            if (cmd == "")
            {
                return;
            }

            OriginalCommand = cmd;

            /* Check if custom settings given */
            TermsToEvaluate = OriginalCommand.Split(";", StringSplitOptions.RemoveEmptyEntries);

            if (TermsToEvaluate.Length == 0)
            {
                return;
            }

            // Settings given
            if (TermsToEvaluate.Length > 1)
            {
                string[] currentsetting;

                for (int i = 1; i < TermsToEvaluate.Length; i++)
                {
                    currentsetting = TermsToEvaluate[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    switch (currentsetting.Length)
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
                                StringBuilder combinedsetting = new StringBuilder();
                                for (int j = 1; j < currentsetting.Length; j++)
                                {
                                    combinedsetting.Append(" ");
                                    combinedsetting.Append(currentsetting[j]);
                                }

                                SettingDecider(currentsetting[0], combinedsetting.ToString());

                                break;
                            }
                    }
                }
            }
            // At this point command should be ready to be examined and settings should all be done
            CleanedCommand = TermsToEvaluate[0];
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Places given setting an value to <see cref="Command.NameSettings"/> or <see cref="Command.ValsSettings"/> dictionaries
        /// </summary>
        /// <param name="settingname">Name of the setting</param>
        /// <param name="valueofsetting">Value of the setting</param>
        private void SettingDecider(string settingname, string valueofsetting)
        {
            if (settingname.Length > 5)
            {
                switch (settingname.Substring(0, 5))// add to name settings
                {
                    case ApplyCmd:
                        settingname = settingname.Replace(ApplyCmd, "");
                        if (settingname == "quiet")
                        {
                            settingname = "display";
                            valueofsetting = "none";
                        }

                        if (!NameSettings.ContainsKey(settingname))
                        {
                            NameSettings.Add(settingname, valueofsetting);
                        }
                        else
                        {
                            NameSettings[settingname] = valueofsetting;
                        }

                        return;

                    case ApplyOut:
                        settingname = settingname.Replace(ApplyOut, "");
                        if (settingname == "quiet")
                        {
                            settingname = "display";
                            valueofsetting = "none";
                        }

                        if (!ValsSettings.ContainsKey(settingname))
                        {
                            ValsSettings.Add(settingname, valueofsetting);
                        }
                        else
                        {
                            ValsSettings[settingname] = valueofsetting;
                        }

                        return;

                    default:
                        break;
                }
            }

            if (settingname == "quiet")
            {
                settingname = "display";
                valueofsetting = "none";
            }

            if (!NameSettings.ContainsKey(settingname))
            {
                NameSettings.Add(settingname, valueofsetting);
            }
            else
            {
                NameSettings[settingname] = valueofsetting;
            }

            if (!ValsSettings.ContainsKey(settingname))
            {
                ValsSettings.Add(settingname, valueofsetting);
            }
            else
            {
                ValsSettings[settingname] = valueofsetting;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Summarizes the command's current state
        /// </summary>
        /// <returns>A string with original command string, settings, state and the output</returns>
        public string CommandSummary()
        {
            StringBuilder cmdset = new StringBuilder();
            StringBuilder outset = new StringBuilder();
            foreach (string setting in NameSettings.Keys)
            {
                cmdset.Append(setting);
                cmdset.Append(":");
                cmdset.Append(NameSettings[setting]);
                cmdset.Append("\t");
            }

            foreach (string setting in ValsSettings.Keys)
            {
                outset.Append(setting);
                outset.Append(":");
                outset.Append(ValsSettings[setting]);
                outset.Append("\t");
            }

            string state = STATE switch
            {
                CommandState.ERROR => "HATA",
                CommandState.IDLE => "BEKLEMEDE",
                CommandState.SUCCESS => "İŞLENDİ",
                CommandState.UNAVAILABLE => throw new NotImplementedException("Komut işlenemedi"),
                CommandState.WARNING => throw new NotImplementedException("Komut işlenemedi"),
                _ => "---",
            };

            return "Komut: " + OriginalCommand + @"
Ek ayarlar(Komut):" + cmdset + @"
Ek ayarlar(Çıktı):" + outset + @"
Çıktı:
" + Output.ToString() + @"
Durum: " + state;

        }
        #endregion
    }

}
