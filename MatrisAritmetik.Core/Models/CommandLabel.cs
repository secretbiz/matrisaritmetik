using System;

namespace MatrisAritmetik.Core.Models
{
    /// <summary>
    /// Class to hold similar functions under a label
    /// </summary>
    public class CommandLabel : IDisposable
    {
        #region Public Fields
        public string Label = "Genel";

        public CommandInfo[] Functions;
        private bool disposedValue;
        #endregion

        #region Constructors
        public CommandLabel()
        {
        }
        /// <summary>
        /// Creates an instance with the given label and a list of <see cref="CommandInfo"/ instances>
        /// </summary>
        /// <param name="label">Name of the label</param>
        /// <param name="cmds">Array of <see cref="CommandInfo"/> instances</param>
        public CommandLabel(string label, CommandInfo[] cmds)
        {
            Label = label;
            Functions = cmds;
        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Functions != null)
                    {
                        foreach (CommandInfo c in Functions)
                        {
                            c.Dispose();
                        }
                    }
                }

                Functions = null;
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~CommandLabel()
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
