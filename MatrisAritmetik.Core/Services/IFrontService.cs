using System.Collections.Generic;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core.Services
{
    /// <summary>
    /// Service for methods related to changes in front-end
    /// </summary>
    public interface IFrontService
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
        void DeleteFromMatrisDict(string name, Dictionary<string, MatrisBase<dynamic>> matdict);
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
        /// Add a command to the given label
        /// </summary>
        /// <param name="label">Command label name</param>
        /// <param name="commandInfos">CommandInfo instances to add</param>
        void AddToCommandLabelList(string label, CommandInfo[] commandInfos);

        /// <summary>
        /// Remove a label
        /// </summary>
        /// <param name="label">Label name to remove</param>
        void ClearCommandLabel(string label);
        #endregion

        #region Command Related Methods
        /// <summary>
        /// Create a command from given string
        /// </summary>
        /// <param name="cmd">Command string</param>
        /// <returns>A cleaned <see cref="Command"/> instance, ready to be evaluated</returns>
        Command CreateCommand(string cmd);

        /// <summary>
        /// Try and find a function with the given <paramref name="name"/>, if found then store it's info in <paramref name="cmdinfo"/>
        /// </summary>
        /// <param name="name">Function name to search for</param>
        /// <param name="cmdinfo"><see cref="CommandInfo"/> instance to use if function is found</param>
        /// <returns>True if <paramref name="name"/> is a function</returns>
        bool TryParseBuiltFunc(string name, out CommandInfo cmdinfo);

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
        /// Command to clean up the command history
        /// </summary>
        void CleanUp();

        /// <summary>
        /// Returns compiler documentation
        /// </summary>
        string Help();
        #endregion
    }
}
