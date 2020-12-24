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
        #region Private Fields
        /// <summary>
        /// Function name or a short description
        /// </summary>
        private string fullname;
        /// <summary>
        /// Full description of the command
        /// </summary>
        private string description;
        /// <summary>
        /// A list of alternative operators and functions that can achieve the same goal as this command
        /// </summary>
        private List<string> alias_list = new List<string>();
        /// <summary>
        /// Real command name
        /// </summary>
        private string function;
        /// <summary>
        /// Parameter type names
        /// </summary>
        private string[] param_types;
        /// <summary>
        /// Parameter names
        /// </summary>
        private string[] param_names;
        /// <summary>
        /// Indices of which parameters are required
        /// </summary>
        private int[] required_params;
        /// <summary>
        /// Type name of what the "function" returns
        /// </summary>
        private string returns;
        /// <summary>
        /// Unfilled command template, no arguments only commas 
        /// </summary>
        private string function_template;
        /// <summary>
        /// Filled command template, parameter names and commas
        /// </summary>
        private string function_template_filled;
        /// <summary>
        /// Name of the service the "function" is declared in
        /// </summary>
        private string service;

        private bool disposedValue;

        #endregion

        #region Public Fields
        public string Fullname { get => fullname; set => fullname = value; }

        public string Description { get => description; set => description = value; }

        public string Function { get => function; set => function = value; }

        public string Service { get => service; set => service = value; }

        public string Returns { get => returns; set => returns = value; }

        public List<string> Alias_list { get => alias_list ?? new List<string>(); set => alias_list = value ?? new List<string>(); }

        public string[] Param_types { get => param_types ?? Array.Empty<string>(); set => param_types = value ?? Array.Empty<string>(); }

        public string[] Param_names { get => param_names ?? Array.Empty<string>(); set => param_names = value ?? Array.Empty<string>(); }

        public int[] Required_params { get => required_params ?? Array.Empty<int>(); set => required_params = value ?? Array.Empty<int>(); }

        public string Function_template { get => function_template; set => function_template = value; }

        public string Function_template_filled { get => function_template_filled; set => function_template_filled = value; }

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
            List<int> paraminds = new List<int>(Required_params);
            for (int i = 0; i < Param_types.Length; i++)
            {
                if (paraminds.Contains(i))
                {
                    req.Append(Param_names[i]);
                    req.Append(':');
                    req.Append(Param_types[i]);
                    if (i != Param_types.Length - 1)
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

            return "Fonksiyon(Tam): " + Function_template_filled + @"
        Açıklama: " + Description + @"
        Alternatif: """ + string.Join(" , ", Alias_list) + @"""" + @"
        Gerekli Minimal Format: !" + Function + "(" + MinimalFormat() + ")" + @"
        Örnek: !" + Function + "(" + string.Join(",", Param_types) + ")";

        }

        public CommandInfo Copy()
        {
            string[] temp = new string[Alias_list.Count];
            string[] tempparamnames = new string[Param_names.Length];
            string[] tempparamtypes = new string[Param_types.Length];
            int[] tempreq = new int[Required_params.Length];

            Alias_list.CopyTo(temp);

            Array.Copy(Param_names, tempparamnames, Param_names.Length);
            Array.Copy(Param_types, tempparamtypes, Param_types.Length);
            Array.Copy(Required_params, tempreq, Required_params.Length);

            return new CommandInfo()
            {
                Fullname = Fullname.ToString(),
                Description = Description.ToString(),
                Function = Function.ToString(),
                Alias_list = new List<string>(temp),
                Function_template = Function_template.ToString(),
                Function_template_filled = Function_template_filled.ToString(),
                Service = Service.ToString(),
                Returns = Returns.ToString(),
                Param_names = tempparamnames,
                Param_types = tempparamtypes,
                Required_params = tempreq
            };
        }
        #endregion

        #region Debug
        private string GetDebuggerDisplay()
        {
            return Service + "." + Function;
        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (alias_list != null)
                    {
                        alias_list.Clear();
                        alias_list = null;
                    }
                }

                Fullname = null;
                Returns = null;
                Function = null;
                Description = null;
                Service = null;
                Function_template = null;
                Function_template_filled = null;
                Required_params = null;
                Param_names = null;
                Param_types = null;

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
