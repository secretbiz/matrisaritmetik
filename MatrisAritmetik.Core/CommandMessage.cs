using System;
using System.Diagnostics;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core
{
    /// <summary>
    /// Class for storing a <see cref="Command"/>'s <see cref="CommandStateMessage"/> and <see cref="CommandState"/> in a single instance
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class CommandMessage : IDisposable
    {
        #region Public Fields
        /// <summary>
        /// Command's current state
        /// </summary>
        private CommandState state = CommandState.IDLE;
        /// <summary>
        /// Last message
        /// </summary>
        private string message = "";

        private bool disposedValue;

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
                    // TODO: dispose managed state (managed objects)
                }

                Message = null;
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~CommandMessage()
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

    /// <summary>
    /// Const strings and static string methods for storing custom command state messages
    /// </summary>
    public static class CommandStateMessage
    {
        // CLEANUP
        public const string SUCCESS_CLEANUP = "Komut geçmişi temizlendi";

        // SUCCESSFUL RETURNS
        public const string SUCCESS_RET_NULL = "";
        public const string SUCCESS_RET_MAT = "";
        public const string SUCCESS_RET_NUM = "";

        // DOCS
        public const string SUCCESS_COMPILER_DOCS = "Derleyici kullanma bilgisi alındı";
        public static string DOCS_MAT_FUNC_FOUND(string name)
        {
            return "Matris ve komut olan " + name + " hakkında bilgi alındı";
        }

        public static string DOCS_MAT_FOUND(string name)
        {
            return "Matris " + name + " hakkında bilgi alındı";
        }

        public static string DOCS_FUNC_FOUND(string name)
        {
            return "Fonksiyon " + name + " hakkında bilgi alındı";
        }

        public static string DOCS_SPECIAL_FOUND(string name)
        {
            return "Özel değer " + name + " hakkında bilgi alındı";
        }

        public static string DOCS_NOT_MAT_FUNC(string name)
        {
            return name + " bir matris veya komut değil!";
        }

        public static string DOCS_NONE_FOUND(string name)
        {
            return name + " hakkında bir bilgi bulunamadı!";
        }

        // UNAVAILABLE
        public static string CMD_UNAVAILABLE(string org)
        {
            return "Komut işleme hatası-> \n" + org;
        }

        // ALREADY COMPILED
        public static string CMD_COMPILED(CommandState st, string msg)
        {
            return "Komut zaten işlenmiş. Durum: " + st + " Extra message: " + msg;
        }
    }
}
