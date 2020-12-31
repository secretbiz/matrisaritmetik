using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core.Services
{
    /// <summary>
    /// Service for methods related to common custom operations on lists, dictionaries and strings
    /// </summary>
    /// <typeparam name="T">Type to use for matrix and list values</typeparam>
    public interface IUtilityService<T>
    {
        #region String Related Methods
        /// <summary>
        /// Convert the string <paramref name="text"/> using <paramref name="delimiter"/> and <paramref name="newline"/> options
        /// to a 2D list of values casted as <typeparamref name="T"/>
        /// </summary>
        /// <param name="text">String to get values from</param>
        /// <param name="delimiter">Delimiter to seperate values from each other</param>
        /// <param name="newline">New-line character to use to seperate rows</param>
        /// <param name="removeliterals">Wheter to remove string literals('\t', '\r', ...)</param>
        /// <param name="rowlimits">Limit for row dimension</param>
        /// <param name="collimits">Limit for column dimension</param>
        /// <param name="allowNonNumber">Wheter to allow non-number values in the list</param>
        /// <param name="options">List of extra option names</param>
        /// <param name="nullfiller">Type to fill empty strings with</param>
        /// <returns>2D list of values found in the given <paramref name="text"/></returns>
        List<List<T>> StringTo2DList(string text,
                                     string delimiter = " ",
                                     string newline = "\n",
                                     bool removeliterals = true,
                                     int rowlimits = (int)MatrisLimits.forRows,
                                     int collimits = (int)MatrisLimits.forCols,
                                     bool allowNonNumber = false,
                                     List<string> options = null,
                                     Type nullfiller = null);

        /// <summary>
        /// Fix \\ characters in literals
        /// </summary>
        /// <param name="old">String to be fixed</param>
        /// <returns>Fixed string, i.e "\\n" -> "\n" </returns>
        string FixLiterals(string old);

        /// <summary>
        /// Create a special matrix from given args in <paramref name="text"/> and function <paramref name="funcinfo"/>
        /// </summary>
        /// <param name="text">Arguments</param>
        /// <param name="funcinfo">Function info instance</param>
        /// <param name="matdict">Matrix dictionary to reference to</param>
        /// <param name="argseperator">Argument seperator used in <paramref name="text"/></param>
        /// <param name="argnamevalseperator">Character to show a parameter name was referred</param>
        /// <param name="removeliterals">Wheter to remove string literals('\t','\r',...)</param>
        /// <param name="mode">Compiler mode</param>
        /// <returns>A special matrix created with given arguments in <paramref name="text"/></returns>
        MatrisBase<T> SpecialStringToMatris(string text,
                                            CommandInfo funcinfo,
                                            Dictionary<string, MatrisBase<dynamic>> matdict,
                                            char argseperator = ',',
                                            char argnamevalseperator = ':',
                                            bool removeliterals = true,
                                            CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix);

        /// <summary>
        /// Create a special dataframe from given args in <paramref name="text"/> and function <paramref name="funcinfo"/>
        /// </summary>
        /// <param name="text">Arguments</param>
        /// <param name="funcinfo">Function info instance</param>
        /// <param name="dfdict">Dataframe dictionary to reference to</param>
        /// <param name="argseperator">Argument seperator used in <paramref name="text"/></param>
        /// <param name="argnamevalseperator">Character to show a parameter name was referred</param>
        /// <param name="removeliterals">Wheter to remove string literals('\t','\r',...)</param>
        /// <param name="options">List of option strings</param>
        /// <param name="mode">Compiler mode</param>
        /// <returns>A special dataframe created with given arguments in <paramref name="text"/></returns>
        Dataframe SpecialStringToDataframe(string text,
                                           CommandInfo funcinfo,
                                           Dictionary<string, Dataframe> dfdict,
                                           char argseperator = ',',
                                           char argnamevalseperator = ':',
                                           bool removeliterals = true,
                                           List<string> options = null,
                                           CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix);
        /// <summary>
        /// Performs an async <see cref="Task"/> to read and decode the request body <paramref name="reqbody"/> with given encoding <paramref name="enc"/>
        /// <para>Ignores request parameters given in <paramref name="ignoredparams"/></para>
        /// <para>Adds parameters and values to <paramref name="decodedRequestDict"/> dictionary</para>
        /// </summary>
        /// <param name="reqbody"></param>
        /// <param name="enc"></param>
        /// <param name="ignoredparams"></param>
        /// <param name="decodedRequestDict"></param>
        /// <returns>Awaitable</returns>
        Task ReadAndDecodeRequest(Stream reqbody,
                                  Encoding enc,
                                  List<string> ignoredparams,
                                  Dictionary<string, string> decodedRequestDict);

        /// <summary>
        /// Read the file data from given request body
        /// </summary>
        /// <param name="reqbody">Request body</param>
        /// <param name="enc">Encoding to use</param>
        /// <param name="filedata">String object to store read data</param>
        /// <returns>Awaitable</returns>
        Task ReadFileFromRequest(Stream reqbody,
                                 Encoding enc,
                                 Dictionary<string, string> filedata);

        /// <summary>
        /// Read option string from given dictionary
        /// </summary>
        /// <param name="dict">Request parameter-value dictionary</param>
        /// <returns>List of option strings</returns>
        List<string> GetOptionList(Dictionary<string, string> dict);

        /// <summary>
        /// Dispose given dictionaries
        /// </summary>
        /// <param name="dfdict">Dataframes</param>
        /// <param name="lbls">Labels</param>
        void DisposeDfDicts(Dictionary<string, Dataframe> dfdict,
                            Dictionary<string, Dictionary<string, List<LabelList>>> lbls);
        #endregion

        #region List Related Methods
        /// <summary>
        /// Gets the index of the absolute max value in the list <paramref name="lis"/>
        /// <para>If the absolute max value occurs more than once, first index is returned</para>
        /// </summary>
        /// <param name="lis">List of values to get absolute max value's index of</param>
        /// <returns>Zero-based index of the absolute max value in <paramref name="lis"/></returns>
        int IndexOfAbsMax(List<T> lis);

        /// <summary>
        /// Gets the minimum value of the list <paramref name="lis"/>
        /// </summary>
        /// <param name="lis">List of values to get the minimum of</param>
        /// <returns>Minimum value in the <paramref name="lis"/></returns>
        T MinOfList(List<T> lis);

        #endregion
    }
}
