using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class Command : IDisposable
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

        #region Private Fields
        /// <summary>
        /// List of commands and settings to evaluate, first one hold the command, rest are settings
        /// </summary>
        private string[] termsToEvaluate;
        /// <summary>
        /// Tokens of the command
        /// </summary>
        private List<Token> tokens = new List<Token>();
        /// <summary>
        /// Output of the command, generally a string
        /// </summary>
        private dynamic output = "";
        /// <summary>
        /// Given command, unprocessed
        /// </summary>
        private string originalCommand = "";
        /// <summary>
        /// Cleaned command, with expected spacing
        /// </summary>
        private string cleanedCommand = "";
        /// <summary>
        /// Given settings, unprocessed
        /// </summary>
        private string originalSettings = "";
        /// <summary>
        /// Dictionary of settings and their values to be applied to command row
        /// </summary>
        private Dictionary<string, string> nameSettings = new Dictionary<string, string>();
        /// <summary>
        /// Dictionary of settings and their values to be applied to output row
        /// </summary>
        private Dictionary<string, string> valsSettings = new Dictionary<string, string>();
        #endregion

        #region Command State Fields and Public Properties
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

        public dynamic Output { get => output; set => output = value; }

        public string OriginalCommand { get => originalCommand; set => originalCommand = value; }

        public string CleanedCommand { get => cleanedCommand; set => cleanedCommand = value; }

        public string OriginalSettings { get => originalSettings; set => originalSettings = value; }

        public int STATEID { get => stateID; set => stateID = value; }

        public string[] GetTermsToEvaluate()
        {
            return termsToEvaluate ?? Array.Empty<string>();
        }

        public void SetTermsToEvaluate(string[] value)
        {
            termsToEvaluate = value ?? Array.Empty<string>();
        }

        public List<Token> GetTokens()
        {
            return tokens ?? new List<Token>();
        }

        public void SetTokens(List<Token> value)
        {
            tokens = value ?? new List<Token>();
        }

        public string GetStateMessage()
        {
            return StateMessage;
        }

        public void SetStateMessage(string value)
        {
            StateMessage = value;
        }

        public Dictionary<string, string> GetNameSettings()
        {
            return nameSettings ?? new Dictionary<string, string>();
        }

        public void SetNameSettings(Dictionary<string, string> value)
        {
            nameSettings = value ?? new Dictionary<string, string>();
        }

        public Dictionary<string, string> GetValsSettings()
        {
            return valsSettings ?? new Dictionary<string, string>();
        }

        public void SetValsSettings(Dictionary<string, string> value)
        {
            valsSettings = value ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Command's <see cref="CommandState"/> as an integer 
        /// </summary>
        private int stateID = -1;
        /// <summary>
        /// Current message about the state of the command
        /// </summary>
        private string StateMessage = "";

        private bool disposedValue;
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
            SetNameSettings(nset);
            SetValsSettings(vset);
            STATE = (CommandState)stat;
            SetStateMessage(statmsg);
            Output = output;
        }

        /// <summary>
        /// Create a command and get it ready for processing
        /// </summary>
        /// <param name="cmd">Command string</param>
        public Command(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                return;
            }

            OriginalCommand = cmd;

            /* Check if custom settings given */
            SetTermsToEvaluate(OriginalCommand.Split(";", StringSplitOptions.RemoveEmptyEntries));

            if (GetTermsToEvaluate().Length == 0)
            {
                return;
            }

            // Settings given
            if (GetTermsToEvaluate().Length > 1)
            {
                string[] currentsetting;

                for (int i = 1; i < GetTermsToEvaluate().Length; i++)
                {
                    currentsetting = GetTermsToEvaluate()[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);

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
                                    combinedsetting.Append(' ');
                                    combinedsetting.Append(currentsetting[j]);
                                }

                                SettingDecider(currentsetting[0], combinedsetting.ToString());

                                break;
                            }
                    }
                }
            }
            // At this point command should be ready to be examined and settings should all be done
            CleanedCommand = GetTermsToEvaluate()[0];
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Places given setting an value to <see cref="Command.GetNameSettings()"/> or <see cref="Command.GetValsSettings()"/> dictionaries
        /// </summary>
        /// <param name="settingname">Name of the setting</param>
        /// <param name="valueofsetting">Value of the setting</param>
        private void SettingDecider(string settingname, string valueofsetting)
        {
            if (settingname.Length > ApplyCmd.Length)
            {
                switch (settingname.Substring(0, ApplyCmd.Length))// add to name settings
                {
                    case ApplyCmd:
                        settingname = settingname.Replace(ApplyCmd, "");
                        if (settingname == "quiet")
                        {
                            settingname = "display";
                            valueofsetting = "none";
                        }

                        if (!GetNameSettings().ContainsKey(settingname))
                        {
                            GetNameSettings().Add(settingname, valueofsetting);
                        }
                        else
                        {
                            GetNameSettings()[settingname] = valueofsetting;
                        }

                        return;

                    case ApplyOut:
                        settingname = settingname.Replace(ApplyOut, "");
                        if (settingname == "quiet")
                        {
                            settingname = "display";
                            valueofsetting = "none";
                        }

                        if (!GetValsSettings().ContainsKey(settingname))
                        {
                            GetValsSettings().Add(settingname, valueofsetting);
                        }
                        else
                        {
                            GetValsSettings()[settingname] = valueofsetting;
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

            if (!GetNameSettings().ContainsKey(settingname))
            {
                GetNameSettings().Add(settingname, valueofsetting);
            }
            else
            {
                GetNameSettings()[settingname] = valueofsetting;
            }

            if (!GetValsSettings().ContainsKey(settingname))
            {
                GetValsSettings().Add(settingname, valueofsetting);
            }
            else
            {
                GetValsSettings()[settingname] = valueofsetting;
            }
        }
        #endregion

        #region Debug
        private string GetDebuggerDisplay()
        {
            return STATE.ToString() + GetStateMessage();
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

            foreach (KeyValuePair<string, string> pair in GetNameSettings())
            {
                cmdset.Append('\n')
                      .Append('\t')
                      .Append(' ')
                      .Append(pair.Key);

                if (!string.IsNullOrWhiteSpace(pair.Value))
                {
                    cmdset.Append(' ')
                          .Append('=')
                          .Append(' ')
                          .Append(pair.Value)
                          .Append(';');
                }
            }

            foreach (KeyValuePair<string, string> pair in GetValsSettings())
            {
                outset.Append('\n')
                      .Append('\t')
                      .Append(' ')
                      .Append(pair.Key);

                if (!string.IsNullOrWhiteSpace(pair.Value))
                {
                    outset.Append(' ')
                          .Append('=')
                          .Append(' ')
                          .Append(pair.Value)
                          .Append(';');
                }
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

            string output = (Output is null || Output == null) ? string.Empty : ((object)Output).ToString();
            return $"Komut: {OriginalCommand}"
                   + $"\nDurum: {state}"
                   + (cmdset.Length == 0 ? string.Empty : ("\nKomut stili:" + cmdset.ToString()))
                   + (outset.Length == 0 ? string.Empty : ("\nÇıktı stili:" + outset.ToString()))
                   + (string.IsNullOrWhiteSpace(output) ? string.Empty : ("\nÇıktı:\n" + output));

        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (GetNameSettings() != null)
                    {
                        SetNameSettings(null);
                    }
                    if (GetValsSettings() != null)
                    {
                        SetValsSettings(null);
                    }
                    if (tokens != null)
                    {
                        foreach (Token d in tokens)
                        {
                            d.Dispose();
                        }
                        SetTokens(null);
                    }
                }

                Output = null;
                OriginalCommand = null;
                CleanedCommand = null;
                OriginalSettings = null;
                SetTermsToEvaluate(null);
                SetStateMessage(null);
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~Command()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

}
