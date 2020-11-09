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

            foreach (var row in filteredText.Split(newline))
            {
                temprow = new List<T>();
                rowsplit = row.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                if (rowsplit.Length != temp && temp != -1)
                {
                    throw new Exception("Bad column size: expected " + temp.ToString() + " got " + rowsplit.Length.ToString());
                }

                temp = 0;

                foreach (var val in rowsplit)
                {
                    if (float.TryParse(val, out float element)) temprow.Add((dynamic)element);
                    else
                    {
                        throw new Exception("Parsing failed: " + val);
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

            if (args.Length < funcinfo.required_params.Length)
                throw new Exception("Yeterli sayıda parametre verilmedi!");

            // Start checking arguments, parse them
            for (int argind = 0; argind < args.Length; argind++)
            {
                rowsplit = args[argind].Split(argnamevalseperator);

                // Positional
                if (rowsplit.Length == 1)
                {
                    currentArg = rowsplit[0];
                    currentParamName = funcinfo.param_names[argind];
                    currentParamType = funcinfo.param_types[argind];

                    if (param_dict.ContainsKey(currentParamName))
                        throw new Exception("Can't reference parameter " + currentParamName + " more than once");

                }
                // Parameter name given
                else if (rowsplit.Length == 2)
                {
                    currentArg = rowsplit[1];
                    currentParamName = rowsplit[0];

                    if (Array.IndexOf(funcinfo.param_names, currentParamName) == -1)
                        throw new Exception("No parameter is named " + currentParamName);

                    if (param_dict.ContainsKey(currentParamName))
                        throw new Exception("Can't reference parameter " + currentParamName + " more than once");

                    currentParamType = funcinfo.param_types[Array.IndexOf(funcinfo.param_names, currentParamName)];
                }
                else
                {
                    throw new Exception("Bad format. Acceptable formats: |1) param1 : value1, param2 : value2 ... |2) value1,value2...");
                }

                // Parse as param type
                switch (currentParamType)
                {
                    case "int":
                        {
                            if (int.TryParse(currentArg, out int element))
                                param_dict.Add(currentParamName, (object)element);
                            else
                                throw new Exception("Parsing failed as int: " + currentArg);

                            break;
                        }
                    case "float":
                        {
                            if (float.TryParse(currentArg, out float element))
                                param_dict.Add(currentParamName, (object)element);
                            else
                                throw new Exception("Parsing failed as float: " + currentArg);

                            break;
                        }
                    case "string":
                        {
                            param_dict.Add(currentParamName, (object)currentArg);
                            break;
                        }
                    case "Matris":
                        {
                            if (matdict.ContainsKey(currentArg))
                                param_dict.Add(currentParamName, (object)matdict[currentArg]);
                            else
                                throw new Exception("No matris found named: " + currentArg);

                            break;
                        }
                    default:
                        {
                            throw new Exception("No type found named: " + currentArg);
                        }
                }

            }

            // Check if all required parameters had values
            foreach (int reqindex in funcinfo.required_params)
            {
                if (!param_dict.ContainsKey(funcinfo.param_names[reqindex]))
                    throw new Exception("Parameter " + funcinfo.param_names[reqindex] + " requires a value");

            }

            object[] param_arg = new object[funcinfo.param_names.Length];

            // Put values in order
            for (int k = 0; k < param_dict.Count; k++)
            {
                param_arg[k] = param_dict[funcinfo.param_names[k]];
            }
            // Create the service
            ConstructorInfo service = Type.GetType("MatrisAritmetik.Services.SpecialMatricesService").GetConstructor(Type.EmptyTypes);
            object serviceObject = service.Invoke(new object[] { });

            // Find and invoke method inside named same as given function name in funcinfo
            MethodInfo method = Type.GetType("MatrisAritmetik.Services.SpecialMatricesService").GetMethod(funcinfo.function);
            MatrisBase<T> result = (MatrisBase<T>)method.Invoke(serviceObject, param_arg);

            return result;
        }

        public async Task ReadAndDecodeRequest(Stream reqbody, Encoding enc, List<string> ignoredparams, Dictionary<string, string> decodedRequestDict)
        {
            using var reader = new StreamReader(reqbody, enc);
            string url = await reader.ReadToEndAsync();

            string[] body = WebUtility.UrlDecode(url).Split("&");    // body = "param=somevalue&param2=someothervalue"
            string[] pairsplit;

            decodedRequestDict.Clear();

            foreach (var pair in body)
            {
                pairsplit = pair.Split("="); // pairsplit[] = { key , value }

                if (ignoredparams.Contains(pairsplit[0]))
                    continue;

                if (!decodedRequestDict.ContainsKey(pairsplit[0]))
                    decodedRequestDict.Add(pairsplit[0], pairsplit[1].Replace("!__EQ!", "=").Replace("!__REVMUL!", "./"));
            }

        }

    }
}
