using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MatrisAritmetik.Core.Models
{
    /// <summary>
    /// Class for using and storing string characters as tokens
    /// <para>Current order of operations:</para>
    /// <code>OPERATOR :  PRIORITY(Higher first)</code>
    /// <code>----------------------------------</code>
    /// <code>  u+      :       200</code>
    /// <code>  u-      :       200</code>
    /// <code> FUNC     :       100</code>
    /// <code>  (       :       10</code>
    /// <code>  )       :       10</code>
    /// <code>  ./      :       6</code>
    /// <code>  .*      :       5</code>
    /// <code>  .^      :       5</code>
    /// <code>  ^       :       5</code>
    /// <code>  *       :       4</code>
    /// <code>  /       :       4</code>
    /// <code>  %       :       4</code>
    /// <code>  +       :       3</code>
    /// <code>  -       :       3</code>
    /// <code>  :       :       2</code>
    /// <code>  ,       :       1</code>
    /// <code>  =       :       0</code>
    /// 
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class Token : IDisposable
    {
        #region Fields

        #region Common Fields For Every Token
        /// <summary>
        /// Token type, see <see cref="TokenType"/>
        /// </summary>
        public TokenType tknType = TokenType.NULL;

        /// <summary>
        /// Priority of this token over other tokens, see <see cref="Token"/>
        /// </summary>
        public int priority;
        #endregion

        #region Number, Matrix or Function Token Fields
        /// <summary>
        /// Value if token is a <see cref="MatrisBase{object}"/> or a number
        /// </summary>
        public dynamic val = 0.0;

        /// <summary>
        /// Matrix or function name
        /// </summary>
        public string name = " ";

        /// <summary>
        /// Function parameter type names
        /// </summary>
        public List<string> paramTypes = new List<string>();

        /// <summary>
        /// Function's service
        /// </summary>
        public string service = "";

        /// <summary>
        /// Type name of the function returns
        /// </summary>
        public string returns = "";
        #endregion

        #region Operator Tokens
        /// <summary>
        /// Symbol of the operator
        /// </summary>
        public string symbol = " ";

        /// <summary>
        /// Associativity of this operator, see <see cref="OperatorAssociativity"/>
        /// </summary>
        public OperatorAssociativity assoc = OperatorAssociativity.LEFT;   // Order
        #endregion

        #region Operator and Function Tokens
        /// <summary>
        /// Amount of parameters this token requires
        /// </summary>
        public int paramCount;

        /// <summary>
        /// Amount of arguments passed to be used with this token
        /// </summary>
        public int argCount;
        #endregion

        #region Docs Token
        /// <summary>
        /// Information about the <see cref="Token.name"/> named matrix or function 
        /// </summary>
        public string info;

        private bool disposedValue;
        #endregion

        #endregion

        #region Constructors
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Token() { }

        /// <summary>
        /// Create a token with given arguments
        /// </summary>
        /// <param name="tknType">Token type</param>
        /// <param name="val">Stored value</param>
        /// <param name="symbol">Operator symbol</param>
        /// <param name="assoc">Associativity</param>
        /// <param name="priority">Priority</param>
        /// <param name="paramCount">Parameter count</param>
        /// <param name="name">Matrix or function name</param>
        /// <param name="paramTypes">Parameter type names</param>
        /// <param name="service">Function service name</param>
        /// <param name="returns">Return type name</param>
        public Token(TokenType tknType,
                     dynamic val,
                     string symbol,
                     OperatorAssociativity assoc,
                     int priority,
                     int paramCount,
                     string name,
                     List<string> paramTypes,
                     string service,
                     string returns)
        {
            this.tknType = tknType;
            this.val = val;
            this.symbol = symbol;
            this.assoc = assoc;
            this.priority = priority;
            this.paramCount = paramCount;
            this.name = name;
            this.paramTypes = paramTypes;
            this.service = service;
            this.returns = returns;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Update token's priorty, symbol, associativity and parameter count
        /// </summary>
        /// <param name="_symbol">New operator symbol</param>
        /// <param name="_assoc">New associativity</param>
        /// <param name="_priority">New priority</param>
        /// <param name="_paramCount">New parameter count</param>
        public void SetValues(string _symbol,
                              OperatorAssociativity _assoc,
                              int _priority,
                              int _paramCount)
        {
            symbol = _symbol;
            assoc = _assoc;
            priority = _priority;
            paramCount = _paramCount;
        }
        #endregion

        #region Overriding Methods
        /// <summary>
        /// Return a short info about the token for easier debugging-watching
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return tknType switch
            {
                TokenType.ARGSEPERATOR => "ARGSEP",
                TokenType.FUNCTION => service + "." + name + "(" + paramCount.ToString() + ")",
                TokenType.MATRIS => "MAT " + name + " " + val.ToString(),
                TokenType.NUMBER => "NUM " + val.ToString(),
                TokenType.OPERATOR => "OP '" + symbol + "'",
                TokenType.NULL => "NULL",
                TokenType.LEFTBRACE => "LEFTBR",
                TokenType.RIGHTBRACE => "RIGHTBR",
                TokenType.STRING => "STRING",
                TokenType.DOCS => "DOCS(" + info + ")",
                TokenType.OUTPUT => "TESTOUTPUT",
                TokenType.ERROR => "ERROR",
                TokenType.VOID => "VOID",
                _ => tknType.ToString(),
            };
        }

        #endregion

        #region Debug
        private string GetDebuggerDisplay()
        {
            return ToString();
        }
        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (paramTypes != null)
                    {
                        paramTypes.Clear();
                        paramTypes = null;
                    }

                    if (val is MatrisBase<object> @mat)
                    {
                        mat.Dispose();
                    }

                    val = null;
                }

                name = null;
                service = null;
                returns = null;
                symbol = null;
                info = null;
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~Token()
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
