using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MatrisAritmetik.Core.Models
{
    /// <summary>
    /// Command information class which hold information about built-in functions
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class CommandInfo : IDisposable
    {
        #region Public Fields
        /// <summary>
        /// Function name or a short description
        /// </summary>
        public string fullname;
        /// <summary>
        /// Full description of the command
        /// </summary>
        public string description;
        /// <summary>
        /// A list of alternative operators and functions that can achieve the same goal as this command
        /// </summary>
        public List<string> alias_list;
        /// <summary>
        /// Real command name
        /// </summary>
        public string function;
        /// <summary>
        /// Parameter type names
        /// </summary>
        public string[] param_types;
        /// <summary>
        /// Parameter names
        /// </summary>
        public string[] param_names;
        /// <summary>
        /// Indices of which parameters are required
        /// </summary>
        public int[] required_params;
        /// <summary>
        /// Type name of what the "function" returns
        /// </summary>
        public string returns;
        /// <summary>
        /// Unfilled command template, no arguments only commas 
        /// </summary>
        public string function_template;
        /// <summary>
        /// Filled command template, parameter names and commas
        /// </summary>
        public string function_template_filled;
        /// <summary>
        /// Name of the service the "function" is declared in
        /// </summary>
        public string service;
        private bool disposedValue;
        #endregion

        #region Constructors
        public CommandInfo()
        {

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a string which shows an example of the current command with it's required parameters filled 
        /// </summary>
        /// <returns>An example call of the command</returns>
        public string MinimalFormat()
        {
            StringBuilder req = new StringBuilder();
            List<int> paraminds = new List<int>(required_params);
            for (int i = 0; i < param_types.Length; i++)
            {
                if (paraminds.Contains(i))
                {
                    req.Append(param_names[i]);
                    req.Append(":");
                    req.Append(param_types[i]);
                    if (i != param_types.Length - 1)
                    {
                        if (paraminds.Contains(i + 1))
                        {

                            req.Append(", ");
                        }
                    }
                }

            }
            return req.ToString();
        }

        /// <summary>
        /// Creates an info string about the command
        /// </summary>
        /// <returns>Function templates, description, settings and an example call</returns>
        public string Info()
        {

            return "Fonksiyon(Tam): " + function_template_filled + @"
        Açıklama: " + description + @"
        Alternatif: """ + string.Join(" , ", alias_list) + @"""" + @"
        Gerekli Minimal Format: !" + function + "(" + MinimalFormat() + ")" + @"
        Örnek: !" + function + "(" + string.Join(",", param_types) + ")";

        }
        #endregion

        #region Debug
        private string GetDebuggerDisplay()
        {
            return service + "." + function;
        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    alias_list.Clear();
                    alias_list = null;
                }

                fullname = null;
                returns = null;
                function = null;
                description = null;
                function_template = null;
                function_template_filled = null;
                service = null;
                required_params = null;
                param_names = null;
                param_types = null;

                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~CommandInfo()
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
