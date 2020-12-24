using System;
using System.Collections.Generic;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core.Services
{
    /// <summary>
    /// Service for methods related to changes in front-end
    /// </summary>
    public interface IFrontService : IDisposable
    {
        #region Matrix dictionary
        /// <summary>
        /// Add a matrix to the matrix dictionary
        /// </summary>
        /// <param name="name">Matrix name</param>
        /// <param name="mat">Matrix to add</param>
        /// <param name="matdict">Matrix dictionary to add the matrix to</param>
        void AddToMatrisDict(string name, MatrisBase<dynamic> mat, Dictionary<string, MatrisBase<dynamic>> matdict);

        /// <summary>
        /// Remove a matrix from the matrix dictionary
        /// </summary>
        /// <param name="name">Matrix name</param>
        /// <param name="matdict">Matrix dictionary to use</param>
        /// <returns>True if matrix was found and deleted</returns>
        bool DeleteFromMatrisDict(string name, Dictionary<string, MatrisBase<dynamic>> matdict);
        #endregion

        #region Built-in Command Methods
        /// <summary>
        /// Read "_builtInCmds.json" file
        /// </summary>
        void ReadCommandInformation();

        /// <summary>
        /// Get built-in command informations in labels
        /// </summary>
        /// <param name="filter">Label names to pick from all labels</param>
        /// <returns>A list of <see cref="CommandLabel"/>s containing built-in command information</returns>
        List<CommandLabel> GetCommandLabelList(List<string> filter = null);

        /// <summary>
        /// Sets given <paramref name="vals"/> values dictionary and <paramref name="opts"/> options dictionary according to <paramref name="dict"/> matrix dictionary
        /// </summary>
        /// <param name="dict">Matrix dictionary to refer to</param>
        /// <param name="vals">Values dictionary to be updated</param>
        /// <param name="opts">Options dictionary to be updated</param>
        void SetMatrixDicts(Dictionary<string, MatrisBase<dynamic>> dict,
                            Dictionary<string, List<List<object>>> vals,
                            Dictionary<string, Dictionary<string, dynamic>> opts);

        #endregion

        #region Command Related Methods
        /// <summary>
        /// Create a command from given string
        /// </summary>
        /// <param name="cmd">Command string</param>
        /// <returns>A cleaned <see cref="Command"/> instance, ready to be evaluated</returns>
        Command CreateCommand(string cmd);

        /// <summary>
        /// Try and find a function with the given <paramref name="name"/>
        /// </summary>
        /// <param name="name">Function name to search for</param>
        /// <returns>A <see cref="CommandInfo"/> instance if given <paramref name="name"/> was found, or null otherwise</returns>
        CommandInfo TryParseBuiltFunc(string name);

        /// <summary>
        /// Tokenize a string expression
        /// </summary>
        /// <param name="exp">String expression</param>
        /// <returns>List of tokens created from given expression</returns>
        List<Token> Tokenize(string exp);

        /// <summary>
        /// Shunting Yard Algorithm to order tokens
        /// </summary>
        /// <param name="tkns">List of tokens to order</param>
        /// <returns>Ordered token list, ready to be evaluated</returns>
        List<Token> ShuntingYardAlg(List<Token> tkns);

        /// <summary>
        /// Evaluate a tokenized command, use <paramref name="matdict"/> as matrix dictionary reference and <paramref name="cmdHistory"/> as command history
        /// </summary>
        /// <param name="cmd">Tokenized command to evaluate</param>
        /// <param name="matdict">Matrix dictionary to reference to</param>
        /// <param name="cmdHistory">Command history to add this command to</param>
        /// <returns>The state of the <paramref name="cmd"/></returns>
        CommandState EvaluateCommand(Command cmd, Dictionary<string, MatrisBase<dynamic>> matdict, List<Command> cmdHistory);

        /// <summary>
        /// Check if last command sent was dated old enough ( ><see cref="CompilerLimits.forCmdSendRateInSeconds"/> seconds old )
        /// </summary>
        /// <returns><c>true</c> if <paramref name="date"/> was null or more than <see cref="CompilerLimits.forCmdSendRateInSeconds"/> seconds old
        /// <para> Otherwise sets current session variables and returns <c>false</c></para></returns>
        bool CheckCmdDate(DateTime calldate);

        /// <summary>
        /// Command to clean up the command history
        /// </summary>
        void CleanUp();

        /// <summary>
        /// Returns information about <paramref name="term"/> if any given, if "<see cref="null"/>" given return compiler help
        /// </summary>
        string Help(dynamic term = null);
        #endregion
    }
}
