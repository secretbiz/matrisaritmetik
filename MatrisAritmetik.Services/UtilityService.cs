using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;

namespace MatrisAritmetik.Services
{
    public class UtilityService<T> : IUtilityService<T>
    {
        public List<List<T>> StringTo2DList(string text, char delimiter = ' ', char newline = '\n', bool removeliterals = true)
        {
            string filteredText = text;
            if (removeliterals)
            {
                filteredText = filteredText.Replace('\t', delimiter).Replace('\r', ' ');
            }
            List<List<T>> vals = new List<List<T>>();
            int temp = -1;
            string[] rowsplit;
            List<T> temprow;

            foreach (string row in filteredText.Split(newline))
            {
                temprow = new List<T>();
                rowsplit = row.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                if (rowsplit.Length != temp && temp != -1)
                {
                    throw new Exception(CompilerMessage.MAT_UNEXPECTED_COLUMN_SIZE(temp.ToString(), rowsplit.Length.ToString()));
                }

                temp = 0;

                foreach (string val in rowsplit)
                {
                    if (float.TryParse(val, out float element))
                    {
                        temprow.Add((dynamic)element);
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.ARG_PARSE_ERROR(val, "float"));
                    }
                    temp += 1;
                }
                vals.Add(temprow);
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

            // Check required parameters and if they are referenced more than once
            Dictionary<string, bool> parameterRequired = new Dictionary<string, bool>();
            for (int i = 0; i < funcinfo.param_names.Length; i++)
            {
                parameterRequired.Add(funcinfo.param_names[i], (Array.IndexOf(funcinfo.required_params, i) != -1));
            }

            // Store given arguments
            Dictionary<string, object> param_dict = new Dictionary<string, object>();

            string[] args = filteredText.Split(argseperator);
            string currentParamName;
            string currentParamType;
            string currentArg;

            if (args.Length < funcinfo.required_params.Length || args.Length > funcinfo.param_names.Length)
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
                    if(paramHintUsed)
                    {
                        throw new Exception(CompilerMessage.ARG_GIVEN_AFTER_HINTED_PARAM);
                    }

                    currentArg = rowsplit[0];
                    currentParamName = funcinfo.param_names[argind];
                    currentParamType = funcinfo.param_types[argind];

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

                    if (Array.IndexOf(funcinfo.param_names, currentParamName) == -1)
                    {
                        throw new Exception(CompilerMessage.PARAMETER_NAME_INVALID(currentParamName));
                    }

                    if (param_dict.ContainsKey(currentParamName))
                    {
                        throw new Exception(CompilerMessage.MULTIPLE_REFERENCES(currentParamName));
                    }

                    currentParamType = funcinfo.param_types[Array.IndexOf(funcinfo.param_names, currentParamName)];
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
            foreach (int reqindex in funcinfo.required_params)
            {
                if (!param_dict.ContainsKey(funcinfo.param_names[reqindex]))
                {
                    throw new Exception(CompilerMessage.MISSING_ARGUMENT(funcinfo.param_names[reqindex]));
                }
            }

            object[] param_arg = new object[funcinfo.param_names.Length];
            int ind;

            // Create the service
            ConstructorInfo service = Type.GetType("MatrisAritmetik.Services.SpecialMatricesService").GetConstructor(Type.EmptyTypes);
            object serviceObject = service.Invoke(new object[] { });

            // Find and invoke method inside named same as given function name in funcinfo
            MethodInfo method = Type.GetType("MatrisAritmetik.Services.SpecialMatricesService").GetMethod(funcinfo.function);
            ParameterInfo[] paraminfo = method.GetParameters();

            // Put values in order
            foreach(string par in param_dict.Keys)
            {
                ind = Array.IndexOf(funcinfo.param_names, par);
                param_arg[ind] = param_dict[par];
            }

            for (int k = 0; k < funcinfo.param_names.Length; k++)
            {
                if(param_arg[k] != null)    // Skip already parsed values
                {
                    continue;
                }

                else if(paraminfo[k].DefaultValue != null) // default value wasn't null
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

        public async Task ReadAndDecodeRequest(Stream reqbody, Encoding enc, List<string> ignoredparams, Dictionary<string, string> decodedRequestDict)
        {
            using StreamReader reader = new StreamReader(reqbody, enc);
            string url = await reader.ReadToEndAsync();

            string[] body = WebUtility.UrlDecode(url).Split("&");    // body = "param=somevalue&param2=someothervalue"
            string[] pairsplit;

            decodedRequestDict.Clear();

            foreach (string pair in body)
            {
                pairsplit = pair.Split("="); // pairsplit[] = { key , value }

                if (ignoredparams.Contains(pairsplit[0]))
                {
                    continue;
                }

                if (!decodedRequestDict.ContainsKey(pairsplit[0]))
                {
                    decodedRequestDict.Add(pairsplit[0], pairsplit[1].Replace("!__EQ!", "=").Replace("!__REVMUL!", "./"));
                }
            }

        }

    }
}
