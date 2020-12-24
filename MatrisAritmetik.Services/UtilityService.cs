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

namespace MatrisAritmetik.Services
{
    public class UtilityService<T> : IUtilityService<T>
    {
        #region Const Strings
        /// <summary>
        /// Encoded version of "=" sign from the body
        /// </summary>
        private const string EQSignSpecial = "!__EQ!";

        /// <summary>
        /// Encoded version of "./" sign from the body
        /// </summary>
        private const string ReverMatMulSpecial = "!__REVMUL!";
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
                                            bool removeliterals = true)
        {
            string filteredText = text;
            string[] literals = new string[9] { "\t", "\r", "\\", "\"", "\'", "\f", "\v", "\a", "\b" };
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
            int m = Math.Min((int)MatrisLimits.forRows, rows.Length);

            if (typestring)
            {
                for (int i = 0; i < m; i++)
                {
                    temprow = new List<T>();
                    rowsplit = rows[i].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    if (rowsplit.Length != temp && temp != -1)
                    {
                        throw new Exception(CompilerMessage.MAT_UNEXPECTED_COLUMN_SIZE(temp.ToString(), rowsplit.Length.ToString()));
                    }

                    temp = 0;

                    for (int j = 0; j < Math.Min((int)MatrisLimits.forCols, rowsplit.Length); j++)
                    {
                        temprow.Add((dynamic)rowsplit[j]);
                        temp += 1;
                    }
                    vals.Add(temprow);
                }
            }
            else
            {
                for (int i = 0; i < m; i++)
                {
                    temprow = new List<T>();
                    rowsplit = rows[i].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    if (rowsplit.Length != temp && temp != -1)
                    {
                        throw new Exception(CompilerMessage.MAT_UNEXPECTED_COLUMN_SIZE(temp.ToString(), rowsplit.Length.ToString()));
                    }

                    temp = 0;

                    for (int j = 0; j < Math.Min((int)MatrisLimits.forCols, rowsplit.Length); j++)
                    {
                        if (float.TryParse(rowsplit[j], out float element))
                        {
                            temprow.Add((dynamic)element);
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

        public MatrisBase<T> SpecialStringTo2DList(string text,
                                                   CommandInfo funcinfo,
                                                   Dictionary<string, MatrisBase<dynamic>> matdict,
                                                   char argseperator = ',',
                                                   char argnamevalseperator = ':',
                                                   bool removeliterals = true)
        {
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
                switch (currentParamType)
                {
                    case "int":
                        {
                            if (int.TryParse(currentArg, out int element))
                            {
                                param_dict.Add(currentParamName, element);
                            }
                            else
                            {
                                throw new Exception(CompilerMessage.ARG_PARSE_ERROR(currentArg, "tamsayı"));
                            }

                            break;
                        }
                    case "float":
                        {
                            if (float.TryParse(currentArg, out float element))
                            {
                                param_dict.Add(currentParamName, element);
                            }
                            else
                            {
                                throw new Exception(CompilerMessage.ARG_PARSE_ERROR(currentArg, "ondalıklı"));
                            }

                            break;
                        }
                    case "string":
                        {
                            param_dict.Add(currentParamName, currentArg);
                            break;
                        }
                    case "Matris":
                        {
                            if (matdict.ContainsKey(currentArg))
                            {
                                param_dict.Add(currentParamName, matdict[currentArg]);
                            }
                            else
                            {
                                throw new Exception(CompilerMessage.NOT_SAVED_MATRIX(currentArg));
                            }

                            break;
                        }
                    default:
                        {
                            throw new Exception(CompilerMessage.UNKNOWN_PARAMETER_TYPE(currentArg));
                        }
                }

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

            for (int k = 0; k < funcinfo.Param_names.Length; k++)
            {
                if (param_arg[k] != null)    // Skip already parsed values
                {
                    continue;
                }

                else if (paraminfo[k].DefaultValue != null) // default value wasn't null
                {
                    switch (paraminfo[k].DefaultValue.GetType().ToString())
                    {
                        case "System.DBNull":
                            {
                                throw new Exception(CompilerMessage.MISSING_ARGUMENT(paraminfo[k].Name));
                            }
                        case "System.Int32":
                            {
                                param_arg[k] = Convert.ToInt32(paraminfo[k].DefaultValue);
                                break;
                            }
                        case "System.Single":
                            {
                                param_arg[k] = Convert.ToSingle(paraminfo[k].DefaultValue);
                                break;
                            }
                        case "System.Double":
                            {
                                param_arg[k] = Convert.ToDouble(paraminfo[k].DefaultValue);
                                break;
                            }
                        default:
                            throw new Exception(CompilerMessage.PARAM_DEFAULT_PARSE_ERROR(paraminfo[k].Name, paraminfo[k].ParameterType.Name));
                    }
                }
            }
            MatrisBase<T> result = (MatrisBase<T>)method.Invoke(serviceObject, param_arg);

            return result;
        }

        public async Task ReadFileFromRequest(Stream reqbody,
                                              Encoding enc,
                                              Dictionary<string, string> filedata)
        {
            MultipartFormDataParser parser = await MultipartFormDataParser.ParseAsync(reqbody, enc).ConfigureAwait(false);
            Stream datastream = parser.Files[0].Data;

            using StreamReader reader = new StreamReader(datastream, enc);

            filedata.Add("data",
                         await reader.ReadToEndAsync());

            filedata.Add("type",
                         parser.GetParameterValue("type"));

            filedata.Add("delim",
                         FixLiterals(parser.GetParameterValue("delim")));

            filedata.Add("newline",
                         FixLiterals(parser.GetParameterValue("newline")));

            filedata.Add("name",
                         parser.GetParameterValue("name"));
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
                    decodedRequestDict.Add(pairsplit[0], pairsplit[1].Replace(EQSignSpecial, "=").Replace(ReverMatMulSpecial, "./"));
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
                if ((float.Parse(val.ToString()) < (float.Parse(currentmin.ToString()))))
                {
                    currentmin = val;
                }
            }
            return currentmin;
        }

        #endregion
    }
}
