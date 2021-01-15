using System;
using System.Diagnostics;

namespace MatrisAritmetik.Core.Models
{
    /// <summary>
    /// Class for storing a <see cref="Command"/>'s <see cref="CommandStateMessage"/> and <see cref="CommandState"/> in a single instance
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class CommandMessage : IDisposable
    {
        #region Private Fields
        /// <summary>
        /// Command's current state
        /// </summary>
        private CommandState state = CommandState.IDLE;
        /// <summary>
        /// Last message
        /// </summary>
        private string message = "";

        private bool disposedValue;
        #endregion

        #region Public Fields
        /// <summary>
        /// Command's current state
        /// </summary>
        public CommandState State { get => state; set => state = value; }
        /// <summary>
        /// Last message
        /// </summary>
        public string Message { get => message; set => message = value; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a basic <see cref="CommandMessage"/> instance
        /// </summary>
        /// <param name="msg">Message to store</param>
        /// <param name="s"><see cref="Command"/>'s state to store</param>
        public CommandMessage(string msg, CommandState s = CommandState.IDLE)
        {
            Message = msg;
            State = s;
        }

        public CommandMessage()
        {
        }
        #endregion

        #region Debug
        private string GetDebuggerDisplay()
        {
            return State.ToString() + ":" + Message;
        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Message = string.Empty;
                    Message = null;
                    disposedValue = true;
                }

            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        //~CommandMessage()
        //{
        //    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //    Dispose(disposing: false);
        //}

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
