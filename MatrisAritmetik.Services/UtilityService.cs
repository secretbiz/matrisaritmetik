using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HttpMultipartParser;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;
using MatrisAritmetik.Models.Core;

namespace MatrisAritmetik.Services
{
    public class UtilityService<T> : IUtilityService<T>
    {
        #region Const Strings
        /// <summary>
        /// Encoded version of "=" sign from the body
        /// </summary>
        private const string EQSignSpecial = "!!__EQ!!";

        /// <summary>
        /// Encoded version of "./" sign from the body
        /// </summary>
        private const string ReverMatMulSpecial = "!!__REVMUL!!";

        /// <summary>
        /// Encoded version of "&" sign from the body
        /// </summary>
        private const string ANDSignSpecial = "!!__AND!!";

        private const long max_size = (long)5e+6;

        #endregion

        #region Private Methods
        /// <summary>
        /// Add a custom null value to given list, if filler is not typeof(float) or null,
        /// </summary>
        /// <param name="lis">List to add a value to</param>
        /// <param name="filler">Type of custom filler</param>
        /// <param name="val">Value to check if <paramref name="filler"/> was not type float or null</param>
        private void FillWithCustomNull(List<T> lis,
                                        Type filler,
                                        string val,
                                        bool allowNonNumber)
        {

            if (string.IsNullOrWhiteSpace(val)) // Check again just in case
            {
                InnerFillWithCustomNull(lis, filler, string.Empty, allowNonNumber);
            }
            else if (!UseConstantIfExist(lis, val))
            {
                InnerFillWithCustomNull(lis, filler, val, allowNonNumber);
            }
        }
        private void InnerFillWithCustomNull(List<T> lis,
                                             Type filler,
                                             string val,
                                             bool allowNonNumber)
        {
            if (filler != typeof(float))
            {

                if (allowNonNumber)
                {
                    lis.Add((dynamic)val);
                }
                else if (filler == null)
                {
                    lis.Add((dynamic)new None());
                }
                else
                {
                    lis.Add((dynamic)float.NaN);
                }
            }
            else
            {
                lis.Add((dynamic)float.NaN);
            }
        }

        /// <summary>
        /// Check given value and add corresponding constant to list if its a constant value's name
        /// </summary>
        /// <param name="lis">List to update</param>
        /// <param name="val">Value to check</param>
        /// <returns>True if value is added</returns>
        private bool UseConstantIfExist(List<T> lis, string val)
        {
            if (val[0] == '!')
            {
                if (Constants.Contains(val[1..^0]))
                {
                    lis.Add((dynamic)Constants.Get(val[1..^0]));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Check given value and add corresponding constant to list if its a constant value's name
        /// </summary>
        /// <param name="dict">Dictionary to update</param>
        /// <param name="name">Dictionary key to use</param>
        /// <param name="val">Value to check</param>
        /// <returns>True if value is added</returns>
        private bool UseConstantIfExist(Dictionary<string, object> dict,
                                        string name,
                                        string val)
        {
            if (val[0] == '!')
            {
                if (Constants.Contains(val[1..^0]))
                {
                    dict.Add(name, (dynamic)Constants.Get(val[1..^0]));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private void ParseArgumentAsParamType(string currentParamType,
                                              string currentParamName,
                                              string currentArg,
                                              Dictionary<string, object> param_dict,
                                              Dictionary<string, MatrisBase<object>> matdict,
                                              CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            switch (currentParamType)
            {
                case "int":
                    {
                        if (int.TryParse(currentArg, out int element))
                        {
                            param_dict.Add(currentParamName, element);
                        }
                        else if (!UseConstantIfExist(param_dict, currentParamName, currentArg))
                        {
                            throw new Exception(CompilerMessage.ARG_PARSE_ERROR(currentArg, currentParamType));
                        }
                        else if (int.TryParse(param_dict[currentParamName].ToString(), out int const_cast)) // Check again if constant was found
                        {
                            param_dict[currentParamName] = const_cast;
                        }
                        else
                        {
                            throw new Exception(CompilerMessage.ARG_PARSE_ERROR(param_dict[currentParamName].ToString(), currentParamType));
                        }
                        break;
                    }
                case "float":
                    {
                        if (float.TryParse(currentArg, out float element))
                        {
                            param_dict.Add(currentParamName, element);
                        }
                        else if (!UseConstantIfExist(param_dict, currentParamName, currentArg))
                        {
                            throw new Exception(CompilerMessage.ARG_PARSE_ERROR(currentArg, currentParamType));
                        }
                        else if (float.TryParse(param_dict[currentParamName].ToString(), out float const_cast)) // Check again if constant was found
                        {
                            param_dict[currentParamName] = const_cast;
                        }
                        else
                        {
                            throw new Exception(CompilerMessage.ARG_PARSE_ERROR(param_dict[currentParamName].ToString(), currentParamType));
                        }

                        break;
                    }
                case "string":
                    {
                        if (!UseConstantIfExist(param_dict, currentParamName, currentArg))
                        {
                            param_dict.Add(currentParamName, currentArg);
                        }
                        else
                        {
                            param_dict[currentParamName] = param_dict[currentParamName].ToString();
                        }
                        break;
                    }
                case "Matris":
                    {
                        if (matdict.ContainsKey(currentArg))
                        {
                            Validations.CheckModeAndMatrixReference(mode, matdict[currentArg]);
                            param_dict.Add(currentParamName, matdict[currentArg]);
                        }
                        else // TO-DO: ADD COMMAND EVALUATION HERE
                        {
                            throw new Exception(CompilerMessage.NOT_SAVED_MATRIX(currentArg));
                        }

                        break;
                    }
                case "Veri Tablosu":
                    {
                        if (matdict.ContainsKey(currentArg))
                        {
                            Validations.CheckModeAndMatrixReference(mode, matdict[currentArg]);
                            param_dict.Add(currentParamName, matdict[currentArg]);
                        }
                        else // TO-DO: ADD COMMAND EVALUATION HERE
                        {
                            throw new Exception(CompilerMessage.NOT_SAVED_DF(currentArg));
                        }

                        break;
                    }
                case "dinamik":
                    {
                        if (float.TryParse(currentArg, out float element)) // Try as float
                        {
                            param_dict.Add(currentParamName, element);
                        }
                        else if (!UseConstantIfExist(param_dict, currentParamName, currentArg)) // Try as constant
                        {
                            param_dict.Add(currentParamName, currentArg); // Just use as string
                        }
                        else
                        {
                            if (float.TryParse(param_dict[currentParamName].ToString(), out float el)) // Try parsing the constant
                            {
                                param_dict[currentParamName] = el; // Use constant's float cast
                            }
                            else
                            {
                                param_dict[currentParamName] = param_dict[currentParamName].ToString(); // Otherwise use constant's string cast
                            }
                        }
                        break;
                    }
                default:
                    {
                        throw new Exception(CompilerMessage.UNKNOWN_PARAMETER_TYPE(currentArg));
                    }
            }
        }
        private void ParseArgumentAsParamType(string currentParamType,
                                              string currentParamName,
                                              string currentArg,
                                              Dictionary<string, object> param_dict,
                                              Dictionary<string, Dataframe> matdict,
                                              CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            switch (currentParamType)
            {
                case "int":
                    {
                        if (int.TryParse(currentArg, out int element))
                        {
                            param_dict.Add(currentParamName, element);
                        }
                        else if (!UseConstantIfExist(param_dict, currentParamName, currentArg))
                        {
                            throw new Exception(CompilerMessage.ARG_PARSE_ERROR(currentArg, currentParamType));
                        }
                        else if (int.TryParse(param_dict[currentParamName].ToString(), out int const_cast)) // Check again if constant was found
                        {
                            param_dict[currentParamName] = const_cast;
                        }
                        else
                        {
                            throw new Exception(CompilerMessage.ARG_PARSE_ERROR(param_dict[currentParamName].ToString(), currentParamType));
                        }
                        break;
                    }
                case "float":
                    {
                        if (float.TryParse(currentArg, out float element))
                        {
                            param_dict.Add(currentParamName, element);
                        }
                        else if (!UseConstantIfExist(param_dict, currentParamName, currentArg))
                        {
                            throw new Exception(CompilerMessage.ARG_PARSE_ERROR(currentArg, currentParamType));
                        }
                        else if (float.TryParse(param_dict[currentParamName].ToString(), out float const_cast)) // Check again if constant was found
                        {
                            param_dict[currentParamName] = const_cast;
                        }
                        else
                        {
                            throw new Exception(CompilerMessage.ARG_PARSE_ERROR(param_dict[currentParamName].ToString(), currentParamType));
                        }

                        break;
                    }
                case "string":
                    {
                        if (!UseConstantIfExist(param_dict, currentParamName, currentArg))
                        {
                            param_dict.Add(currentParamName, currentArg);
                        }
                        else
                        {
                            param_dict[currentParamName] = param_dict[currentParamName].ToString();
                        }
                        break;
                    }
                case "Matris":
                    {
                        if (matdict.ContainsKey(currentArg))
                        {
                            Validations.CheckModeAndMatrixReference(mode, matdict[currentArg]);
                            param_dict.Add(currentParamName, matdict[currentArg]);
                        }
                        else // TO-DO: ADD COMMAND EVALUATION HERE
                        {
                            throw new Exception(CompilerMessage.NOT_SAVED_MATRIX(currentArg));
                        }

                        break;
                    }
                case "Veri Tablosu":
                    {
                        if (matdict.ContainsKey(currentArg))
                        {
                            Validations.CheckModeAndMatrixReference(mode, matdict[currentArg]);
                            param_dict.Add(currentParamName, matdict[currentArg]);
                        }
                        else // TO-DO: ADD COMMAND EVALUATION HERE
                        {
                            throw new Exception(CompilerMessage.NOT_SAVED_DF(currentArg));
                        }

                        break;
                    }
                case "dinamik":
                    {
                        if (float.TryParse(currentArg, out float element)) // Try as float
                        {
                            param_dict.Add(currentParamName, element);
                        }
                        else if (!UseConstantIfExist(param_dict, currentParamName, currentArg)) // Try as constant
                        {
                            param_dict.Add(currentParamName, currentArg); // Just use as string
                        }
                        else
                        {
                            if (float.TryParse(param_dict[currentParamName].ToString(), out float el)) // Try parsing the constant
                            {
                                param_dict[currentParamName] = el; // Use constant's float cast
                            }
                            else
                            {
                                param_dict[currentParamName] = param_dict[currentParamName].ToString(); // Otherwise use constant's string cast
                            }
                        }
                        break;
                    }
                default:
                    {
                        throw new Exception(CompilerMessage.UNKNOWN_PARAMETER_TYPE(currentArg));
                    }
            }
        }

        #endregion

        #region UtilityService Methods
        public string FixLiterals(string old)
        {
            string[] literals = new string[10] { "\n", "\t", "\r", "\\", "\"", "\'", "\f", "\v", "\a", "\b" };
            string[] literalsBad = new string[10] { "\\n", "\\t", "\\r", "\\\\", "\"", "\'", "\\f", "\\v", "\\a", "\\b" };
            for (int i = 0; i < 9; i++)
            {
                old = old.Replace(literalsBad[i], literals[i]);
            }
            return old;
        }

        public List<List<T>> StringTo2DList(string text,
                                            string delimiter = " ",
                                            string newline = "\n",
                                            bool removeliterals = true,
                                            int rowlimits = (int)MatrisLimits.forRows,
                                            int collimits = (int)MatrisLimits.forCols,
                                            bool allowNonNumber = false,
                                            List<string> options = null,
                                            Type nullfiller = null)
        {
            if (text.Length > max_size)
            {
                throw new Exception(CompilerMessage.TEXT_SIZE_INVALID);
            }
            string filteredText = text;
            string[] literals = new string[9] { "\t", "\r", "\\", "\"", "\'", "\f", "\v", "\a", "\b" };
            string single_space = " ";

            if (removeliterals)
            {
                foreach (string lit in literals)
                {
                    if (delimiter.IndexOf(lit) != -1 && newline.IndexOf(lit) != -1)
                    {
                        filteredText = filteredText.Replace(lit, delimiter);
                    }
                }
            }

            filteredText = filteredText.Trim();
            if (string.IsNullOrEmpty(filteredText))
            {
                throw new Exception(CompilerMessage.MAT_INVALID);
            }

            while (filteredText[^1] == '\n')
            {
                filteredText = filteredText[0..^1];
            }

            List<List<T>> vals = new List<List<T>>();
            int temp = -1;
            string[] rowsplit;
            List<T> temprow;
            bool typestring = typeof(T).Name == "String";

            string[] rows = filteredText.Split(newline);
            int nr = Math.Min(rowlimits, rows.Length);

            if (typestring)
            {
                for (int i = 0; i < nr; i++)
                {
                    temprow = new List<T>();
                    rowsplit = rows[i].Split(delimiter, delimiter == single_space ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);

                    if (rowsplit.Length != temp && temp != -1)
                    {
                        throw new Exception(CompilerMessage.MAT_UNEXPECTED_COLUMN_SIZE(temp.ToString(), rowsplit.Length.ToString()));
                    }

                    temp = 0;

                    for (int j = 0; j < Math.Min(collimits, rowsplit.Length); j++)
                    {
                        if (!string.IsNullOrWhiteSpace(rowsplit[j]))
                        {
                            temprow.Add((dynamic)rowsplit[j]);
                        }
                        else if (nullfiller == typeof(float))
                        {
                            temprow.Add((dynamic)float.NaN);
                        }
                        else
                        {
                            temprow.Add((dynamic)new None());
                        }
                        temp += 1;
                    }
                    vals.Add(temprow);
                }
            }
            else
            {
                int start = options != null
                    ? options.Contains("use_row_as_lbl")
                        ? 1
                        : 0
                    : 0;

                nr = Math.Min(rowlimits + start, rows.Length);

                // Parse first row as strings if option was given
                for (int i = 0; i < start; i++)
                {
                    temprow = new List<T>();
                    rowsplit = rows[i].Split(delimiter, delimiter == single_space ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
                    for (int j = 0; j < Math.Min(rowlimits, rowsplit.Length); j++)
                    {
                        if (!string.IsNullOrWhiteSpace(rowsplit[j]))
                        {
                            temprow.Add((dynamic)rowsplit[j]);
                        }
                        else
                        {
                            FillWithCustomNull(temprow, nullfiller, rowsplit[j], allowNonNumber);
                        }
                    }

                    vals.Add(temprow);
                }

                // Try parsing rest accordingly
                for (int i = start; i < nr; i++)
                {
                    temprow = new List<T>();
                    rowsplit = rows[i].Split(delimiter, delimiter == single_space ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);

                    if (rowsplit.Length != temp && temp != -1)
                    {
                        throw new Exception(CompilerMessage.MAT_UNEXPECTED_COLUMN_SIZE(temp.ToString(), rowsplit.Length.ToString()));
                    }

                    temp = 0;

                    for (int j = 0; j < Math.Min(rowlimits, rowsplit.Length); j++)
                    {
                        if (!string.IsNullOrWhiteSpace(rowsplit[j]))
                        {
                            if (float.TryParse(rowsplit[j], out float element))
                            {
                                temprow.Add((dynamic)element);
                            }
                            else
                            {
                                FillWithCustomNull(temprow, nullfiller, rowsplit[j], allowNonNumber);
                            }
                        }
                        else if (allowNonNumber)
                        {
                            FillWithCustomNull(temprow, nullfiller, rowsplit[j], allowNonNumber);
                        }
                        else if (nullfiller == typeof(float))
                        {
                            temprow.Add((dynamic)float.NaN);
                        }
                        else
                        {
                            throw new Exception(CompilerMessage.ARG_PARSE_ERROR(rowsplit[j], "float"));
                        }
                        temp += 1;
                    }
                    vals.Add(temprow);
                }
            }

            return vals;
        }

        public MatrisBase<T> SpecialStringToMatris(string text,
                                                   CommandInfo funcinfo,
                                                   Dictionary<string, MatrisBase<dynamic>> matdict,
                                                   char argseperator = ',',
                                                   char argnamevalseperator = ':',
                                                   bool removeliterals = true,
                                                   CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            if (text.Length > max_size)
            {
                throw new Exception(CompilerMessage.TEXT_SIZE_INVALID);
            }
            string filteredText = text;
            if (removeliterals)
            {
                filteredText = filteredText.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ').Replace(" ", "");
            }

            string[] rowsplit;

            // Store given arguments
            Dictionary<string, object> param_dict = new Dictionary<string, object>();

            string[] args = filteredText.Split(argseperator);
            string currentParamName;
            string currentParamType;
            string currentArg;

            if (args.Length < funcinfo.Required_params || args.Length > funcinfo.Param_names.Length)
            {
                throw new Exception(CompilerMessage.ARG_COUNT_ERROR);
            }

            bool paramHintUsed = false;

            // Start checking arguments, parse them
            for (int argind = 0; argind < args.Length; argind++)
            {
                rowsplit = args[argind].Split(argnamevalseperator);

                // Positional
                if (rowsplit.Length == 1)
                {
                    if (paramHintUsed)
                    {
                        throw new Exception(CompilerMessage.ARG_GIVEN_AFTER_HINTED_PARAM);
                    }

                    currentArg = rowsplit[0];
                    currentParamName = funcinfo.Param_names[argind];
                    currentParamType = funcinfo.Param_types[argind];

                    if (param_dict.ContainsKey(currentParamName))
                    {
                        throw new Exception(CompilerMessage.MULTIPLE_REFERENCES(currentParamName));
                    }
                }
                // Parameter name given
                else if (rowsplit.Length == 2)
                {
                    paramHintUsed = true;
                    currentArg = rowsplit[1];
                    currentParamName = rowsplit[0];

                    if (Array.IndexOf(funcinfo.Param_names, currentParamName) == -1)
                    {
                        throw new Exception(CompilerMessage.PARAMETER_NAME_INVALID(currentParamName));
                    }

                    if (param_dict.ContainsKey(currentParamName))
                    {
                        throw new Exception(CompilerMessage.MULTIPLE_REFERENCES(currentParamName));
                    }

                    currentParamType = funcinfo.Param_types[Array.IndexOf(funcinfo.Param_names, currentParamName)];
                }
                else
                {
                    throw new Exception(CompilerMessage.STRING_FORMAT_INVALID("param_1:değer_1, param_2:değer_2 ... veya değer_1,değer_2,..."));
                }

                // Parse as param type
                ParseArgumentAsParamType(currentParamType, currentParamName, currentArg, param_dict, matdict, mode);

            }

            // Check if all required parameters had values
            for (int i = 0; i < funcinfo.Required_params; i++)
            {
                if (!param_dict.ContainsKey(funcinfo.Param_names[i]))
                {
                    throw new Exception(CompilerMessage.MISSING_ARGUMENT(funcinfo.Param_names[i]));
                }
            }

            object[] param_arg = new object[funcinfo.Param_names.Length];
            int ind;

            // Create the service
            ConstructorInfo service = Type.GetType("MatrisAritmetik.Services.SpecialMatricesService").GetConstructor(Type.EmptyTypes);
            object serviceObject = service.Invoke(Array.Empty<object>());

            // Find and invoke method inside named same as given function name in funcinfo
            MethodInfo method = Type.GetType("MatrisAritmetik.Services.SpecialMatricesService").GetMethod(funcinfo.Function);
            ParameterInfo[] paraminfo = method.GetParameters();

            // Put values in order
            foreach (string par in param_dict.Keys)
            {
                ind = Array.IndexOf(funcinfo.Param_names, par);
                param_arg[ind] = param_dict[par];
            }

            CompilerUtils.ParseDefaultValues(funcinfo.Param_names.Length, param_arg, paraminfo);

            MatrisBase<T> result = (MatrisBase<T>)method.Invoke(serviceObject, param_arg);

            Validations.CheckModeAndMatrixReference(mode, result, true);
            return result;
        }

        public Dataframe SpecialStringToDataframe(string text,
                                                  CommandInfo funcinfo,
                                                  Dictionary<string, Dataframe> dfdict,
                                                  char argseperator = ',',
                                                  char argnamevalseperator = ':',
                                                  bool removeliterals = true,
                                                  List<string> options = null,
                                                  CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            if (text.Length > max_size)
            {
                throw new Exception(CompilerMessage.TEXT_SIZE_INVALID);
            }
            string filteredText = text;
            if (removeliterals)
            {
                filteredText = filteredText.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ').Replace(" ", "");
            }

            string[] rowsplit;

            // Store given arguments
            Dictionary<string, object> param_dict = new Dictionary<string, object>();

            string[] args = filteredText.Split(argseperator);
            string currentParamName;
            string currentParamType;
            string currentArg;

            if (args.Length < funcinfo.Required_params || args.Length > funcinfo.Param_names.Length)
            {
                throw new Exception(CompilerMessage.ARG_COUNT_ERROR);
            }

            bool paramHintUsed = false;

            // Start checking arguments, parse them
            for (int argind = 0; argind < args.Length; argind++)
            {
                rowsplit = args[argind].Split(argnamevalseperator);

                // Positional
                if (rowsplit.Length == 1)
                {
                    if (paramHintUsed)
                    {
                        throw new Exception(CompilerMessage.ARG_GIVEN_AFTER_HINTED_PARAM);
                    }

                    currentArg = rowsplit[0];
                    currentParamName = funcinfo.Param_names[argind];
                    currentParamType = funcinfo.Param_types[argind];

                    if (param_dict.ContainsKey(currentParamName))
                    {
                        throw new Exception(CompilerMessage.MULTIPLE_REFERENCES(currentParamName));
                    }
                }
                // Parameter name given
                else if (rowsplit.Length == 2)
                {
                    paramHintUsed = true;
                    currentArg = rowsplit[1];
                    currentParamName = rowsplit[0];

                    if (Array.IndexOf(funcinfo.Param_names, currentParamName) == -1)
                    {
                        throw new Exception(CompilerMessage.PARAMETER_NAME_INVALID(currentParamName));
                    }

                    if (param_dict.ContainsKey(currentParamName))
                    {
                        throw new Exception(CompilerMessage.MULTIPLE_REFERENCES(currentParamName));
                    }

                    currentParamType = funcinfo.Param_types[Array.IndexOf(funcinfo.Param_names, currentParamName)];
                }
                else
                {
                    throw new Exception(CompilerMessage.STRING_FORMAT_INVALID("param_1:değer_1, param_2:değer_2 ... veya değer_1,değer_2,..."));
                }

                // Parse as param type
                ParseArgumentAsParamType(currentParamType, currentParamName, currentArg, param_dict, dfdict, mode);

            }

            // Check if all required parameters had values
            for (int i = 0; i < funcinfo.Required_params; i++)
            {
                if (!param_dict.ContainsKey(funcinfo.Param_names[i]))
                {
                    throw new Exception(CompilerMessage.MISSING_ARGUMENT(funcinfo.Param_names[i]));
                }
            }

            object[] param_arg = new object[funcinfo.Param_names.Length];
            int ind;

            // Create the service
            ConstructorInfo service = Type.GetType("MatrisAritmetik.Services.SpecialMatricesService").GetConstructor(Type.EmptyTypes);
            object serviceObject = service.Invoke(Array.Empty<object>());

            // Find and invoke method inside named same as given function name in funcinfo
            MethodInfo method = Type.GetType("MatrisAritmetik.Services.SpecialMatricesService").GetMethod(funcinfo.Function);
            ParameterInfo[] paraminfo = method.GetParameters();

            // Put values in order
            foreach (string par in param_dict.Keys)
            {
                ind = Array.IndexOf(funcinfo.Param_names, par);
                param_arg[ind] = param_dict[par];
            }

            CompilerUtils.ParseDefaultValues(funcinfo.Param_names.Length, param_arg, paraminfo);
            Dataframe df = (Dataframe)method.Invoke(serviceObject, param_arg);

            Validations.CheckModeAndMatrixReference(mode, df, true);

            df.ApplyOptions(df.GetValues(), options, true);
            return df;
        }

        public async Task ReadFileFromRequest(Stream reqbody,
                                              Encoding enc,
                                              Dictionary<string, string> filedata)
        {
            MultipartFormDataParser parser = await MultipartFormDataParser.ParseAsync(reqbody, enc).ConfigureAwait(false);
            Stream datastream = parser.Files[0].Data;

            List<string> typelist = new List<string>() { "text/plain", "text/csv", "application/vnd.ms-excel" };

            using StreamReader reader = new StreamReader(datastream, enc);

            if (!typelist.Contains(parser.GetParameterValue("type")))
            {
                throw new Exception(CompilerMessage.FILE_TYPE_INVALID);
            }

            filedata.Add("data",
                         await reader.ReadToEndAsync());

            if ((filedata["data"].Length) > max_size)
            {
                throw new Exception(CompilerMessage.FILE_SIZE_INVALID);
            }

            filedata.Add("type",
                         parser.GetParameterValue("type"));

            filedata.Add("delim",
                         FixLiterals(parser.GetParameterValue("delim")));

            filedata.Add("newline",
                         FixLiterals(parser.GetParameterValue("newline")));

            filedata.Add("name",
                         parser.GetParameterValue("name"));

            filedata.Add("extras",
                         parser.GetParameterValue("extras"));
        }

        public async Task ReadAndDecodeRequest(Stream reqbody,
                                               Encoding enc,
                                               List<string> ignoredparams,
                                               Dictionary<string, string> decodedRequestDict)
        {
            using StreamReader reader = new StreamReader(reqbody, enc);
            string url = await reader.ReadToEndAsync();

            string[] body = WebUtility.UrlDecode(url).Split("&");    // body = "param=somevalue&param2=someothervalue"
            string[] pairsplit;

            decodedRequestDict.Clear();

            foreach (string pair in body)
            {
                pairsplit = pair.Split("="); // pairsplit[] = { key , value }

                if (pairsplit.Length != 2)
                {   // Something wasn't right
                    decodedRequestDict.Clear();
                    return;
                }

                if (ignoredparams.Contains(pairsplit[0]))
                {
                    continue;
                }

                if (!decodedRequestDict.ContainsKey(pairsplit[0]))
                {
                    decodedRequestDict.Add(pairsplit[0], pairsplit[1].Replace(EQSignSpecial, "=").Replace(ANDSignSpecial, "&").Replace(ReverMatMulSpecial, "./"));
                }
            }

        }

        public int IndexOfAbsMax(List<T> lis)
        {
            if (lis.Count == 0)
            {
                throw new Exception("Liste boş!");
            }

            if (lis.Count == 1)
            {
                return 0;
            }

            int currentmax = 0;
            for (int i = 1; i < lis.Count; i++)
            {
                if (Math.Abs(float.Parse(lis[i].ToString())) > Math.Abs(float.Parse(lis[currentmax].ToString())))
                {
                    currentmax = i;
                }
            }
            return currentmax;
        }

        public T MinOfList(List<T> lis)
        {
            if (lis.Count == 0)
            {
                throw new Exception("Liste boş!");
            }

            if (lis.Count == 1)
            {
                return lis[0];
            }

            T currentmin = lis[0];
            foreach (dynamic val in lis.GetRange(1, lis.Count - 1))
            {
                if (float.Parse(val.ToString()) < float.Parse(currentmin.ToString()))
                {
                    currentmin = val;
                }
            }
            return currentmin;
        }

        #endregion
    }
}
