using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core
{
    /// <summary>
    /// Enumerated <see cref="Command"/> states
    /// </summary>
    public enum CommandState
    {
        /// <summary>
        /// Command was created but was not processed
        /// </summary>
        IDLE,

        /// <summary>
        /// Command is currently being proccessed or had an unknown issue during a process
        /// </summary>
        UNAVAILABLE,

        /// <summary>
        /// Command was successfully processed
        /// </summary>
        SUCCESS,

        /// <summary>
        /// Command returned/threw a warning message
        /// </summary>
        WARNING,

        /// <summary>
        /// Command threw an error message
        /// </summary>
        ERROR
    };

    /// <summary>
    /// Enumerated <see cref="Token"/> types
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Unknown token
        /// </summary>
        NULL,

        /// <summary>
        /// String token
        /// </summary>
        STRING,

        /// <summary>
        /// Number token
        /// </summary>
        NUMBER,

        /// <summary>
        /// Matrix token
        /// </summary>
        MATRIS,

        /// <summary>
        /// Function token
        /// </summary>
        FUNCTION,

        /// <summary>
        /// Function argument seperator token
        /// </summary>
        ARGSEPERATOR,

        /// <summary>
        /// Operator with a symbol token
        /// </summary>
        OPERATOR,

        /// <summary>
        /// Left brace token
        /// </summary>
        LEFTBRACE,

        /// <summary>
        /// Right brace token
        /// </summary>
        RIGHTBRACE,

        /// <summary>
        /// Token for "?" character
        /// </summary>
        DOCS,

        /// <summary>
        /// Output type for tests
        /// </summary>
        OUTPUT,

        /// <summary>
        /// Error type for tests
        /// </summary>
        ERROR,

        /// <summary>
        /// Void function return token, 
        /// </summary>
        VOID
    };

    /// <summary>
    /// Enumerated operator associativity sides
    /// </summary>
    public enum OperatorAssociativity
    {
        LEFT,
        RIGHT
    };

    /// <summary>
    /// Compiler dictionary reference mode
    /// </summary>
    public enum CompilerDictionaryMode
    {
        /// <summary>
        /// Compiler mode that only enables usage of matrix dictionaries
        /// </summary>
        Matrix,

        /// <summary>
        /// Compiler mode that only enables usage of dataframe dictionaries
        /// </summary>
        Dataframe,

        /// <summary>
        /// Compiler mode that enables both dataframe and matrix dictionaries
        /// </summary>
        All
    }
}
