using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;
using MatrisAritmetik.Models.Core;
using Newtonsoft.Json;

namespace MatrisAritmetik.Services
{
    public class FrontService : IFrontService
    {
        #region Private Fields
        /// <summary>
        /// Hold state of command history clean-up
        /// </summary>
        private bool CleanUp_state;
        /// <summary>
        /// Built-in commands
        /// </summary>
        private List<CommandLabel> builtInCommands = new List<CommandLabel>();

        private bool disposedValue;
        #endregion

        #region Private Methods

        /// <summary>
        /// Set basic fields of a <see cref="Token"/> <paramref name="tkn"/> by checking its <paramref name="symbol"/>
        /// </summary>
        /// <param name="tkn"><see cref="Token"/> to use</param>
        /// <param name="symbol">Symbol/operator to use</param>
        /// <returns>Given token with values set</returns>
        private static Token SetTokenFieldsViaSymbol(Token tkn,
                                              string symbol)
        {
            switch (symbol)
            {
                case ":":
                    {
                        tkn.tknType = TokenType.OPERATOR;
                        tkn.symbol = ":";
                        tkn.priority = 2;
                        tkn.assoc = OperatorAssociativity.RIGHT;
                        tkn.paramCount = 2;
                        break;
                    }
                case "=":
                    {
                        tkn.tknType = TokenType.OPERATOR;
                        tkn.symbol = "=";
                        tkn.priority = 0;
                        tkn.assoc = OperatorAssociativity.RIGHT;
                        tkn.paramCount = 2;
                        break;
                    }
                case "+":
                    {
                        tkn.tknType = TokenType.OPERATOR;
                        tkn.symbol = "+";
                        tkn.priority = 3;
                        tkn.assoc = OperatorAssociativity.LEFT;
                        tkn.paramCount = 2;
                        break;
                    }
                case "-":
                    {
                        tkn.tknType = TokenType.OPERATOR;
                        tkn.symbol = "-";
                        tkn.priority = 3;
                        tkn.assoc = OperatorAssociativity.LEFT;
                        tkn.paramCount = 2;
                        break;
                    }
                case "*":
                    {
                        tkn.tknType = TokenType.OPERATOR;
                        tkn.symbol = "*";
                        tkn.priority = 4;
                        tkn.assoc = OperatorAssociativity.LEFT;
                        tkn.paramCount = 2;
                        break;
                    }
                case "/":
                    {
                        tkn.tknType = TokenType.OPERATOR;
                        tkn.symbol = "/";
                        tkn.priority = 4;
                        tkn.assoc = OperatorAssociativity.LEFT;
                        tkn.paramCount = 2;
                        break;
                    }
                case "%":
                    {
                        tkn.tknType = TokenType.OPERATOR;
                        tkn.symbol = "%";
                        tkn.priority = 4;
                        tkn.assoc = OperatorAssociativity.LEFT;
                        tkn.paramCount = 2;
                        break;
                    }
                case "^":
                    {
                        tkn.tknType = TokenType.OPERATOR;
                        tkn.symbol = "^";
                        tkn.priority = 5;
                        tkn.assoc = OperatorAssociativity.LEFT;
                        tkn.paramCount = 2;
                        break;
                    }
                case ".^":
                    {
                        tkn.tknType = TokenType.OPERATOR;
                        tkn.symbol = ".^";
                        tkn.priority = 5;
                        tkn.assoc = OperatorAssociativity.LEFT;
                        tkn.paramCount = 2;
                        break;
                    }
                case ".*":
                    {
                        tkn.tknType = TokenType.OPERATOR;
                        tkn.symbol = ".*";
                        tkn.priority = 5;
                        tkn.assoc = OperatorAssociativity.LEFT;
                        tkn.paramCount = 2;
                        break;
                    }
                case "./":
                    {
                        tkn.tknType = TokenType.OPERATOR;
                        tkn.symbol = "./";
                        tkn.priority = 6;
                        tkn.assoc = OperatorAssociativity.LEFT;
                        tkn.paramCount = 2;
                        break;
                    }
                default:
                    {
                        tkn.tknType = TokenType.NULL;
                        break;
                    }
            }
            return tkn;
        }

        /// <summary>
        /// Set given token as a document token about given <paramref name="value"/>
        /// </summary>
        /// <param name="tkn">Token to apply values to</param>
        /// <param name="value">Value to search docs for</param>
        private void SetAsDocsToken(Token tkn,
                                    string value)
        {
            tkn.tknType = TokenType.DOCS;
            using CommandInfo cmdinfo = TryParseBuiltFunc(value);
            if (cmdinfo != null)
            {   // VALID FUNCTION
                tkn.name = value;
                tkn.info = cmdinfo.Info();
            }
            else if (Validations.ValidMatrixName(value))
            {   // MATRIX
                tkn.name = value;
                tkn.info = "matrix";
            }
            else if (string.IsNullOrEmpty(value)) // Spammed bunch of question marks
            {
                tkn.info = "info";
            }
            else    // NOTHING FOUND
            {
                tkn.val = value;
                tkn.info = "null";
            }
        }

        /// <summary>
        /// Try to create a special value token with given <paramref name="name"/> if it's a special mathematical constant's name
        /// </summary>
        /// <param name="tkn">Token to apply values to </param>
        /// <param name="name">Special value's name</param>
        /// <returns>True if token was set as a special value token</returns>
        private static bool SetAsSpecialToken(Token tkn,
                                       string name)
        {
            if (name == "null")
            {
                tkn.val = new None();
                tkn.tknType = TokenType.NULL;
            }
            else if (name == "e")
            {
                tkn.val = 2.7182818;
                tkn.tknType = TokenType.NUMBER;
            }
            else if (name == "pi")
            {
                tkn.val = 3.1415926;
                tkn.tknType = TokenType.NUMBER;
            }
            else if (name == "tau")
            {
                tkn.val = 6.2831853;
                tkn.tknType = TokenType.NUMBER;
            }
            else if (name == "sq2")
            {
                tkn.val = 1.4142135;
                tkn.tknType = TokenType.NUMBER;
            }
            else if (name == "sq3")
            {
                tkn.val = 1.7320508;
                tkn.tknType = TokenType.NUMBER;
            }
            else if (name == "sq5")
            {
                tkn.val = 2.2360679;
                tkn.tknType = TokenType.NUMBER;
            }
            else if (name == "golden")
            {
                tkn.val = 1.6180339;
                tkn.tknType = TokenType.NUMBER;
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Try to create a function token from if given <paramref name="name"/> is a built-in function name
        /// </summary>
        /// <param name="tkn">Token to apply values to</param>
        /// <param name="name">Function name to search and use</param>
        /// <returns>True if token was set to be a function token</returns>
        private bool SetAsFunctionToken(Token tkn,
                                        string name)
        {
            using CommandInfo cmdinfo = TryParseBuiltFunc(name);
            if (cmdinfo != null) // VALID FUNCTION
            {
                tkn.paramCount = cmdinfo.Param_names.Length;
                tkn.service = cmdinfo.Service.ToString();
                tkn.tknType = TokenType.FUNCTION;
                tkn.name = name;
                tkn.returns = cmdinfo.Returns.ToString();
                tkn.paramTypes = new List<string>(cmdinfo.Param_types);
                tkn.priority = 100;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Set a token from given expression as a function, a special value, a matrix, documentation or null token
        /// </summary>
        /// <param name="tkn">Token to set values to</param>
        /// <param name="exp">Expression to process</param>
        private void SetSpecialValueToken(Token tkn,
                                          string exp)
        {
            if (exp[0] == '!')          // A function or a special value
            {
                exp = exp.Replace("!", "");
                if (!SetAsFunctionToken(tkn, exp))
                {
                    if (!SetAsSpecialToken(tkn, exp))
                    {
                        throw new Exception(CompilerMessage.NOT_A_(tkn.ToString() + ":" + exp, "fonksiyon ya da özel bir değer"));
                    }
                }
            }
            else if (Validations.ValidMatrixName(exp))  // MATRIX
            {
                tkn.tknType = TokenType.MATRIS;
                tkn.name = exp;
            }
            else if (exp[0] == '?')                     // Information about following expression
            {
                exp = exp.Replace("?", "").Trim();
                SetAsDocsToken(tkn, exp);
            }
            else                                        // SHOULDN'T ENT UP HERE, RETURN NULL
            {
                SetAsSpecialToken(tkn, "null");
            }
        }

        /// <summary>
        /// Create a token from a string expression
        /// </summary>
        /// <param name="exp"> String expression to tokenize </param>
        /// <returns>Token created from the expression</returns>
        private Token String2Token(string exp)
        {
            Token tkn = new Token();

            if (double.TryParse(exp, out double val))
            {
                tkn.tknType = TokenType.NUMBER;                     // NUMBER
                tkn.val = val;
            }

            else if (exp == "(")
            {
                tkn.tknType = TokenType.LEFTBRACE;                  // LEFT BRACE
                tkn.priority = 10;
            }

            else if (exp == ")")
            {
                tkn.tknType = TokenType.RIGHTBRACE;                 // RIGHT BRACE
                tkn.priority = 10;
            }

            else if (exp.Length > 1
                     && exp != ".^"
                     && exp != ".*"
                     && exp != "./")
            {
                SetSpecialValueToken(tkn, exp);
            }

            else if (Validations.ValidMatrixName(exp))
            {
                tkn.tknType = TokenType.MATRIS;                   // MATRIX
                tkn.name = exp;
            }

            else if (exp == "?")
            {
                tkn.info = "info";
                tkn.tknType = TokenType.DOCS;
            }

            else if (exp == ",")
            {
                tkn.tknType = TokenType.ARGSEPERATOR;               // ARGUMENT SEPERATOR
                tkn.priority = 1;
            }

            else
            {                                                       // OPERATOR
                tkn = SetTokenFieldsViaSymbol(tkn, exp);
            }

            return tkn;
        }

        /// <summary>
        /// Split a string expression to tokenizable elements
        /// </summary>
        /// <param name="exp">String expression to split</param>
        /// <returns>Array of tokenizable string expressions</returns>
        private static string[] TokenizeSplit(string exp)
        {
            exp = exp
                .Replace("+", " + ")
                .Replace("+ =", "+=")
                .Replace("-", " - ")
                .Replace("- =", "-=")
                .Replace("*", " * ")
                .Replace("* =", "*=")
                .Replace(". *", ".*")
                .Replace("/", " / ")
                .Replace("/ =", "/=")
                .Replace(". /", "./")
                .Replace("(", " ( ")
                .Replace(")", " ) ")
                .Replace(",", " , ")
                .Replace("%", " % ")
                .Replace("% =", "%=")
                .Replace("^", " ^ ")
                .Replace("^ =", "^=")
                .Replace(". ^", ".^")
                .Replace(".*", " .* ")
                .Replace(".^", " .^ ")
                .Replace("./", " ./ ")
                .Replace("=", " = ")
                .Replace(":", " : ")
                .Trim();

            if (exp.Contains("=")) // Matris_name = some_expression
            {
                string[] expsplits = exp.Split("=", StringSplitOptions.RemoveEmptyEntries);
                if (expsplits.Length == 0)
                {
                    throw new Exception(CompilerMessage.EQ_FORMAT);
                }
                else if (expsplits.Length != 2)
                {
                    throw new Exception(CompilerMessage.EQ_MULTIPLE_USE);
                }
                else
                {
                    expsplits[0] = expsplits[0].Trim();
                    expsplits[1] = expsplits[1].Trim();
                    if (string.IsNullOrEmpty(expsplits[0]) || string.IsNullOrEmpty(expsplits[1]))
                    {
                        throw new Exception(CompilerMessage.EQ_FORMAT);
                    }

                    string newPart = "";
                    switch (expsplits[0][^1].ToString())
                    {
                        case "+":
                            {
                                expsplits[0] = expsplits[0][0..^1].Trim();
                                newPart = expsplits[0] + " + ";
                                break;
                            }
                        case "-":
                            {
                                expsplits[0] = expsplits[0][0..^1].Trim();
                                newPart = expsplits[0] + " - ";
                                break;
                            }
                        case "*":
                            {
                                expsplits[0] = expsplits[0][0..^1].Trim();
                                newPart = expsplits[0] + " * ";
                                break;
                            }
                        case "/":
                            {
                                expsplits[0] = expsplits[0][0..^1].Trim();
                                newPart = expsplits[0] + " / ";
                                break;
                            }
                        case "^":
                            {
                                expsplits[0] = expsplits[0][0..^1].Trim();
                                newPart = expsplits[0] + " ^ ";
                                break;
                            }
                        case "%":
                            {
                                expsplits[0] = expsplits[0][0..^1].Trim();
                                newPart = expsplits[0] + " % ";
                                break;
                            }
                        default:
                            {
                                if (!Validations.ValidMatrixName(expsplits[0]))
                                {
                                    throw new Exception(CompilerMessage.MAT_NAME_INVALID);
                                }
                                break;
                            }
                    }

                    exp = $"{expsplits[0]} = {newPart}( {expsplits[1]} )";   // Matris_name = (some_expression)
                }
            }
            while (exp.Contains("  "))
            {
                exp = exp.Replace("  ", " ");
            }
            return exp.Split(" ");
        }

        /// <summary>
        /// Get the index of parameter <paramref name="name"/> in the parameter array
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="paramarr">Parameter array</param>
        /// <returns>Index of the <paramref name="name"/> if found, -1 otherwise</returns>
        private static int GetParamIndex(string name,
                                  ParameterInfo[] paramarr)
        {
            int ind = 0;
            foreach (ParameterInfo p in paramarr)
            {
                if (p.Name.ToString() == name)
                {
                    break;
                }
                ind++;
            }
            return (ind < paramarr.Length) ? ind : -1;
        }

        /// <summary>
        /// Check if given token <paramref name="tkn"/> is a <see cref="MatrisBase{object}"/>
        /// </summary>
        /// <param name="tkn">Token to check</param>
        /// <param name="matDict">Matrix dictionary to reference to</param>
        /// <returns>True if given token holds a <see cref="MatrisBase{object}"/></returns>
        private static bool CheckMatrixAndUpdateVal(Token tkn,
                                             Dictionary<string, MatrisBase<dynamic>> matDict)
        {
            switch (tkn.tknType)
            {
                case TokenType.MATRIS:
                    if (matDict.ContainsKey(tkn.name))
                    {
                        tkn.val = matDict[tkn.name];
                    }
                    else if (!(tkn.val is MatrisBase<object>))
                    {
                        throw new Exception(CompilerMessage.NOT_SAVED_MATRIX(tkn.name));
                    }

                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Invoke given <paramref name="method"/> of <paramref name="serviceObject"/> with given <paramref name="arguments"/> and set <paramref name="operands"/> index 0 as it's result
        /// </summary>
        /// <param name="op">Operator used</param>
        /// <param name="operands">Operands</param>
        /// <param name="serviceObject">Service object</param>
        /// <param name="method">Method information object</param>
        /// <param name="arguments">Arguments to pass to <paramref name="method"/></param>
        /// <returns>Whatever the <paramref name="method"/> returns stored in a <see cref="Token"/></returns>
        private Token InvokeMethods(Token op,
                                    List<Token> operands,
                                    object serviceObject,
                                    MethodInfo method,
                                    object[] arguments)
        {
            switch (op.service)
            {
                case "FrontService":
                    switch (op.name)
                    {
                        case "CleanUp":
                            {
                                CleanUp();
                                operands.Add(new Token() { val = null, tknType = TokenType.VOID });
                                break;
                            }
                        case "Help":
                            {
                                return new Token() { name = "", val = Help(), tknType = TokenType.DOCS };
                            }
                        default:
                            throw new Exception(CompilerMessage.UNKNOWN_FRONTSERVICE_FUNC(op.name));
                    }
                    break;

                default:
                    if (!string.IsNullOrEmpty(op.service))
                    {
                        try
                        {
                            if (method == null)
                            {
                                throw new Exception(CompilerMessage.UNKNOWN_SERVICE(op.service));
                            }
                            // Invoke the method
                            // No parameters
                            if (arguments.Length == 0)
                            {
                                method.Invoke(serviceObject, null);
                                operands.Add(new Token() { val = null, tknType = TokenType.VOID });
                            }
                            else
                            {
                                operands[0].val = (dynamic)method.Invoke(serviceObject, arguments);
                            }
                        }
                        catch (Exception err)
                        {
                            if (err.InnerException != null)
                            {
                                throw new Exception(err.InnerException.Message);
                            }

                            throw new Exception(err.Message);
                        }

                        operands[0].tknType = op.returns switch
                        {
                            "Matris" => TokenType.MATRIS,
                            "int" => TokenType.NUMBER,
                            "float" => TokenType.NUMBER,
                            "void" => TokenType.VOID,
                            _ => TokenType.NULL,
                        };
                    }
                    break;
            }

            operands[0].name = "";

            return operands[0];
        }

        /// <summary>
        /// Parse argument's value depending on <see cref="TokenType"/> of <paramref name="operand"/>
        /// </summary>
        /// <param name="op">Operator used</param>
        /// <param name="operand">Operand used</param>
        /// <param name="arguments">Arguments</param>
        /// <param name="argIndex">Index of the argument</param>
        /// <param name="matDict">Matrix dictionary to reference to</param>
        /// <returns>Argument at index <paramref name="argIndex"/> after changes applied</returns>
        private static object ApplyArgumentValue(Token op,
                                          Token operand,
                                          object[] arguments,
                                          int argIndex,
                                          Dictionary<string, MatrisBase<dynamic>> matDict)
        {
            switch (operand.tknType)
            {
                case TokenType.NULL: arguments[argIndex] = null; break;

                case TokenType.NUMBER: arguments[argIndex] = operand.val; break;

                case TokenType.MATRIS:
                    {
                        arguments[argIndex] = matDict.ContainsKey(operand.name)
                            ? matDict[operand.name]
                            : operand.val is MatrisBase<object>
                                ? (object)operand.val
                                : throw new Exception(CompilerMessage.UNKNOWN_VARIABLE(operand.name));

                        break;
                    }

                default:
                    {
                        throw new Exception(CompilerMessage.UNKNOWN_ARGUMENT_TYPE(op.name, argIndex + 1));
                    }
            }
            return arguments[argIndex];
        }

        /// <summary>
        /// Checks given <paramref name="arguments"/> and wheter they are referenced correctly, then parses them as expected types
        /// </summary>
        /// <param name="op">Operator token</param>
        /// <param name="operands">Operand tokens list</param>
        /// <param name="arguments">Arguments array</param>
        /// <param name="paraminfo">Parameter information array</param>
        /// <param name="param_dict">Parameter dictionary to keep track of parameters</param>
        /// <param name="matDict">Matrix dictionary to reference to if needed</param>
        /// <returns>Parsed <paramref name="arguments"/></returns>
        private static object[] CheckAndParseArgumentsAndHints(Token op,
                                                        List<Token> operands,
                                                        object[] arguments,
                                                        ParameterInfo[] paraminfo,
                                                        Dictionary<string, MatrisBase<dynamic>> matDict)
        {
            Dictionary<string, dynamic> param_dict = new Dictionary<string, dynamic>();
            bool hintUsed = false;

            for (int k = 0; k < op.argCount; k++)
            {
                int pind;
                if (operands[k].info != null)    // hint was given
                {
                    hintUsed = true;
                    pind = GetParamIndex(operands[k].info, paraminfo);

                    if (pind == -1)
                    {
                        throw new Exception(CompilerMessage.PARAMETER_NAME_INVALID(operands[k].info));
                    }
                    else
                    {
                        if (param_dict.ContainsKey(paraminfo[pind].Name.ToString()))
                        {
                            throw new Exception(CompilerMessage.MULTIPLE_REFERENCES(paraminfo[pind].Name.ToString()));
                        }
                        param_dict.Add(paraminfo[pind].Name.ToString(), operands[k].val);
                    }
                }
                else if (hintUsed)
                {
                    throw new Exception(CompilerMessage.ARG_GIVEN_AFTER_HINTED_PARAM);
                }
                else if (param_dict.ContainsKey(paraminfo[k].Name.ToString()))
                {
                    throw new Exception(CompilerMessage.MULTIPLE_REFERENCES(paraminfo[k].Name.ToString()));
                }
                else
                {
                    param_dict.Add(paraminfo[k].Name.ToString(), operands[k].val);
                    pind = k;
                }

                arguments[pind] = ApplyArgumentValue(op, operands[k], arguments, pind, matDict);

                if (arguments[pind] != null) // Parse given value if not null
                {
                    arguments[pind] = ParseTokenValAsParameterType(op, operands, arguments, k, pind);
                }

            }
            return arguments;
        }

        /// <summary>
        /// Parse argument at <paramref name="argumentIndex"/> of <paramref name="arguments"/> as type given at <paramref name="argumentIndex"/> of <paramref name="op"/>'s <see cref="Token.paramTypes"/>
        /// </summary>
        /// <param name="op">Operator token</param>
        /// <param name="operands">Operands list</param>
        /// <param name="arguments">Arguments array</param>
        /// <param name="operandIndex">Index of operand to use</param>
        /// <param name="argumentIndex">Index of argument to parse</param>
        /// <returns> Argument at index <paramref name="argumentIndex"/> after parsed as parameter's type</returns>
        private static object ParseTokenValAsParameterType(Token op,
                                                    List<Token> operands,
                                                    object[] arguments,
                                                    int operandIndex,
                                                    int argumentIndex)
        {
            try
            {
                switch (op.paramTypes[argumentIndex])
                {
                    case "int":
                        {
                            if (operands[operandIndex].tknType == TokenType.MATRIS) // Try parsing scalar matrix as an integer
                            {
                                if (!((MatrisBase<dynamic>)arguments[argumentIndex]).IsScalar())
                                {
                                    throw new Exception();
                                }
                                else
                                {
                                    arguments[argumentIndex] = Convert.ToInt32((string)((MatrisBase<dynamic>)arguments[argumentIndex])[0, 0].ToString(), CultureInfo.CurrentCulture);
                                }
                            }
                            else
                            {
                                arguments[argumentIndex] = Convert.ToInt32(arguments[argumentIndex], CultureInfo.CurrentCulture);
                            }
                            break;
                        }
                    case "float":
                        {
                            if (operands[operandIndex].tknType == TokenType.MATRIS)  // Try parsing scalar matrix as a float
                            {
                                if (!((MatrisBase<dynamic>)arguments[argumentIndex]).IsScalar())
                                {
                                    throw new Exception();
                                }
                                else
                                {
                                    arguments[argumentIndex] = Convert.ToSingle((string)((MatrisBase<dynamic>)arguments[argumentIndex])[0, 0].ToString(), CultureInfo.CurrentCulture);
                                }
                            }
                            else
                            {
                                arguments[argumentIndex] = Convert.ToSingle(arguments[argumentIndex], CultureInfo.CurrentCulture);
                            }
                            break;
                        }
                    case "Matris":
                        {
                            arguments[argumentIndex] = (MatrisBase<dynamic>)arguments[argumentIndex];
                            break;
                        }
                    default:
                        {
                            throw new Exception(CompilerMessage.UNKNOWN_PARAMETER_TYPE(op.paramTypes[argumentIndex]));
                        }
                }
            }
            catch (Exception)
            {
                throw new Exception(CompilerMessage.ARG_PARSE_ERROR(arguments[argumentIndex].ToString(), op.paramTypes[argumentIndex]));
            }

            return arguments[argumentIndex];
        }

        /// <summary>
        /// Parse default values from <paramref name="parameterInfo"/> into given "null" values in <paramref name="arguments"/> 
        /// </summary>
        /// <param name="parameterCount">How many parameters to iterate over</param>
        /// <param name="arguments">Arguments array</param>
        /// <param name="parameterInfo">Parameter information array</param>
        /// <returns><paramref name="arguments"/> where "null" values are parsed as default values picked from <paramref name="parameterInfo"/></returns>
        private static object[] ParseDefaultValues(int parameterCount,
                                            object[] arguments,
                                            ParameterInfo[] parameterInfo)
        {
            for (int k = 0; k < parameterCount; k++)
            {
                if (arguments[k] != null)    // Skip already parsed values
                {
                    continue;
                }
                else if (parameterInfo[k].DefaultValue != null) // default value wasn't null
                {
                    switch (parameterInfo[k].DefaultValue.GetType().ToString())
                    {
                        case "System.DBNull":
                            {
                                throw new Exception(CompilerMessage.MISSING_ARGUMENT(parameterInfo[k].Name));
                            }
                        case "System.Int32":
                            {
                                arguments[k] = Convert.ToInt32(parameterInfo[k].DefaultValue, CultureInfo.CurrentCulture);
                                break;
                            }
                        case "System.Single":
                            {
                                arguments[k] = Convert.ToSingle(parameterInfo[k].DefaultValue, CultureInfo.CurrentCulture);
                                break;
                            }
                        case "System.Double":
                            {
                                arguments[k] = Convert.ToDouble(parameterInfo[k].DefaultValue, CultureInfo.CurrentCulture);
                                break;
                            }
                        default:
                            throw new Exception(CompilerMessage.PARAM_DEFAULT_PARSE_ERROR(parameterInfo[k].Name, parameterInfo[k].ParameterType.Name));
                    }
                }
            }

            return arguments;
        }

        /// <summary>
        /// Custom power operator
        /// </summary>
        /// <param name="a">base value</param>
        /// <param name="b">exponential value</param>
        /// <returns>a^b</returns>
        private static double PowerMethod(double a, double b)
        {
            double result;
            if (b == 0.0)
            {
                if (a == 0.0) // 0^0
                {
                    result = double.NaN;
                }
                else
                {
                    result = 1.0;    // x^0
                }
            }
            else if (b < 0.0)
            {
                result = a == 0.0 ? double.PositiveInfinity : Math.Pow(a, b);
            }
            else
            {
                result = a == 0.0 ? 0.0 : Math.Pow(a, b);
            }
            return result;
        }

        /// <summary>
        /// Evaluate expressino where <paramref name="op"/> is <see cref="TokenType.OPERATOR"/> type
        /// </summary>
        /// <param name="op">Operator token, type <see cref="TokenType.OPERATOR"/></param>
        /// <param name="operands">List of operands</param>
        /// <param name="matDict">Matrix dictionary to refer to if necessary</param>
        /// <returns>Operands list after evaluation</returns>
        private static List<Token> EvalWithSymbolOperator(Token op,
                                                   List<Token> operands,
                                                   Dictionary<string, MatrisBase<dynamic>> matDict)
        {
            switch (op.symbol)
            {
                case "+":
                    {
                        CheckMatrixAndUpdateVal(operands[0], matDict);
                        CheckMatrixAndUpdateVal(operands[1], matDict);

                        operands[0].val = operands[1].val + operands[0].val;
                        operands[0].tknType = operands[1].tknType == TokenType.MATRIS ? TokenType.MATRIS : operands[0].tknType;

                        break;
                    }
                case "-":
                    {
                        CheckMatrixAndUpdateVal(operands[0], matDict);
                        CheckMatrixAndUpdateVal(operands[1], matDict);

                        operands[0].val = operands[1].val - operands[0].val;
                        operands[0].tknType = operands[1].tknType == TokenType.MATRIS ? TokenType.MATRIS : operands[0].tknType;

                        break;
                    }
                case "*":
                    {
                        CheckMatrixAndUpdateVal(operands[0], matDict);
                        CheckMatrixAndUpdateVal(operands[1], matDict);

                        operands[0].val *= operands[1].val;
                        operands[0].tknType = operands[1].tknType == TokenType.MATRIS ? TokenType.MATRIS : operands[0].tknType;

                        break;
                    }
                case "/":
                    {
                        CheckMatrixAndUpdateVal(operands[0], matDict);
                        CheckMatrixAndUpdateVal(operands[1], matDict);

                        operands[0].val = operands[1].val / operands[0].val;
                        operands[0].tknType = operands[1].tknType == TokenType.MATRIS ? TokenType.MATRIS : operands[0].tknType;
                        break;
                    }
                case "%":
                    {
                        if (operands[0].tknType == TokenType.NUMBER) // dynamic % number
                        {
                            CheckMatrixAndUpdateVal(operands[1], matDict);

                            operands[0].val = operands[1].val % (dynamic)(int)operands[0].val;
                            operands[0].tknType = operands[1].tknType;
                        }
                        else if (CheckMatrixAndUpdateVal(operands[0], matDict)) // matris % matris
                        {
                            // base operands[0]
                            // term to get mod of operands[1]should be matrix
                            operands[0].val = CheckMatrixAndUpdateVal(operands[1], matDict) || ((MatrisBase<dynamic>)operands[0].val).IsScalar()
                                ? operands[1].val % operands[0].val
                                : throw new Exception(CompilerMessage.MOD_MAT_THEN_BASE_MAT);
                        }
                        else
                        {
                            throw new Exception(CompilerMessage.MODULO_FORMATS);
                        }

                        break;
                    }
                case "^":   // A^3 == A'nın elemanlarının 3. kuvvetleri
                    {
                        if (operands[0].tknType != TokenType.NUMBER)
                        {
                            operands[0].val = !CheckMatrixAndUpdateVal(operands[0], matDict)
                                              ? throw new Exception(CompilerMessage.EXPO_NOT_SCALAR)
                                              : !((MatrisBase<dynamic>)operands[0].val).IsScalar()
                                                  ? throw new Exception(CompilerMessage.MAT_SHOULD_BE_SCALAR)
                                                  : ((MatrisBase<dynamic>)operands[0].val)[0, 0];
                        }

                        if (CheckMatrixAndUpdateVal(operands[1], matDict))  // base matrix
                        {
                            operands[0].val = ((MatrisBase<object>)operands[1].val).Power((dynamic)operands[0].val);
                            operands[0].tknType = TokenType.MATRIS;
                        }
                        else // base is number
                        {
                            operands[0].val = PowerMethod(double.Parse(operands[1].val.ToString()), double.Parse(operands[0].val.ToString()));
                            operands[0].tknType = TokenType.NUMBER;
                        }

                        break;
                    }
                case ".^":      // A.^3 == A@A@A  , A kare matris
                    {
                        if (operands[0].tknType != TokenType.NUMBER)
                        {
                            throw new Exception(CompilerMessage.EXPO_NOT_SCALAR);
                        }

                        if (operands[0].val < 0)
                        {
                            throw new Exception(CompilerMessage.SPECOP_MATPOWER_EXPO);
                        }
                        else if (operands[0].val == 0)
                        {
                            operands[0].val = 1;
                            operands[0].tknType = TokenType.NUMBER;
                            break;
                        }

                        if (CheckMatrixAndUpdateVal(operands[1], matDict))
                        {
                            if (!operands[1].val.IsSquare())
                            {
                                throw new Exception(CompilerMessage.SPECOP_MATPOWER_SQUARE);
                            }

                            MatrisBase<dynamic> res = operands[1].val.Copy();
                            using MatrisBase<dynamic> mat = res.Copy();

                            IMatrisArithmeticService<dynamic> matservice = new MatrisArithmeticService<dynamic>();

                            for (int i = 1; i < operands[0].val; i++)
                            {
                                res = matservice.MatrisMul(res, mat);
                            }

                            operands[0].val = res;
                            operands[0].tknType = TokenType.MATRIS;
                        }
                        else
                        {
                            throw new Exception(CompilerMessage.SPECOP_MATPOWER_BASE);
                        }

                        break;
                    }
                case ".*":
                    {
                        MatrisBase<dynamic> mat1, mat2;
                        mat1 = CheckMatrixAndUpdateVal(operands[0], matDict)
                            ? (MatrisBase<dynamic>)operands[0].val
                            : throw new Exception(CompilerMessage.OP_BETWEEN_(".*", "matrisler"));

                        mat2 = CheckMatrixAndUpdateVal(operands[1], matDict)
                            ? (MatrisBase<dynamic>)operands[1].val
                            : throw new Exception(CompilerMessage.OP_BETWEEN_(".*", "matrisler"));

                        operands[0].val = ((IMatrisArithmeticService<dynamic>)new MatrisArithmeticService<object>()).MatrisMul(mat2, mat1);

                        break;
                    }
                case "./":
                    {
                        MatrisBase<dynamic> mat1, mat2;
                        mat1 = CheckMatrixAndUpdateVal(operands[0], matDict)
                            ? (MatrisBase<dynamic>)operands[0].val
                            : throw new Exception(CompilerMessage.OP_BETWEEN_("./", "matrisler"));

                        mat2 = CheckMatrixAndUpdateVal(operands[1], matDict)
                            ? (MatrisBase<dynamic>)operands[1].val
                            : throw new Exception(CompilerMessage.OP_BETWEEN_("./", "matrisler"));

                        IMatrisArithmeticService<object> matservice = new MatrisArithmeticService<object>();

                        operands[0].val = matservice.MatrisMul(mat2, matservice.Inverse(mat1));

                        break;
                    }
                case "u-":
                    {
                        CheckMatrixAndUpdateVal(operands[0], matDict);

                        operands[0].val = -operands[0].val;

                        break;
                    }
                case "u+":
                    {
                        CheckMatrixAndUpdateVal(operands[0], matDict);

                        break;
                    }
                case "=":
                    {
                        if (operands[1].tknType != TokenType.MATRIS) // LHS should just be a valid name for a matrix
                        {
                            throw new Exception(CompilerMessage.EQ_FORMAT);
                        }
                        else
                        {
                            switch (operands[0].tknType)
                            {
                                case TokenType.NUMBER:  // RHS is scalar
                                    {
                                        operands[0].val = new MatrisBase<dynamic>(1, 1, operands[0].val);
                                        break;
                                    }
                                case TokenType.MATRIS:   // RHS is possibly a matrix
                                    {
                                        operands[0].val = matDict.ContainsKey(operands[0].name)
                                            ? (dynamic)matDict[operands[0].name]
                                            : operands[0].val is MatrisBase<object>
                                                ? operands[0].val
                                                : throw new Exception(CompilerMessage.NOT_SAVED_MATRIX(operands[0].name));
                                        break;
                                    }
                                default:
                                    {
                                        if (!(operands[0].val is MatrisBase<object>))  // If RHS is not even a matrix, throw 
                                        {
                                            throw new Exception(CompilerMessage.EQ_FAILED);
                                        }
                                        break;
                                    }
                            }

                            // Update the matrix table accordingly
                            if (matDict.ContainsKey(operands[1].name))
                            {
                                matDict[operands[1].name] = operands[0].val;
                            }
                            else if (Validations.ValidMatrixName(operands[1].name))
                            {
                                if (matDict.Count < (int)MatrisLimits.forMatrisCount)
                                {
                                    matDict.Add(operands[1].name, operands[0].val);
                                }
                                else
                                {
                                    throw new Exception(CompilerMessage.MAT_LIMIT);
                                }
                            }
                            else // LHS was invalid
                            {
                                if (string.IsNullOrEmpty(operands[1].name))
                                {
                                    throw new Exception(CompilerMessage.EQ_FORMAT);
                                }
                                else
                                {
                                    throw new Exception(CompilerMessage.MAT_NAME_INVALID);
                                }
                            }
                        }
                        break;
                    }
                case ":":
                    {
                        operands[0].info = Validations.ValidMatrixName(operands[1].name)
                            ? operands[1].name
                            : throw new Exception(CompilerMessage.PARAMETER_HINT_INVALID(operands[1].name));
                        break;
                    }
                default:
                    throw new Exception(CompilerMessage.OP_INVALID(op.symbol));
            }

            return operands;
        }

        /// <summary>
        /// Evaluate <paramref name="operands"/> with operator/function <paramref name="op"/>
        /// </summary>
        /// <param name="op">Operator or function token</param>
        /// <param name="operands">Operands to be used</param>
        /// <param name="matDict">Matrix dictionary to be used for matrix references</param>
        /// <returns>A token storing the result of the evaluation</returns>
        private Token EvalOperator(Token op,
                                   List<Token> operands,
                                   Dictionary<string, MatrisBase<dynamic>> matDict)
        {
            switch (op.tknType)
            {
                case TokenType.OPERATOR: // OPERATORS
                    {
                        operands = EvalWithSymbolOperator(op, operands, matDict);
                        break;
                    }

                default:
                    {
                        if (op.argCount > op.paramCount)
                        {
                            throw new Exception(CompilerMessage.FUNC_PARAMCOUNT_EXCESS(op.name, op.paramCount, op.argCount));
                        }

                        object[] param_arg = new object[op.paramCount];

                        object serviceObject = null;
                        MethodInfo method = null;
                        ParameterInfo[] paraminfo = new ParameterInfo[op.paramCount];

                        operands.Reverse();

                        // Set service and method information
                        if (op.tknType == TokenType.DOCS)
                        {
                            throw new Exception(CompilerMessage.DOCS_HELP);
                        }

                        if (operands.Count == 0 && op.argCount != 0)
                        {
                            throw new Exception(CompilerMessage.PARANTHESIS_COUNT_ERROR);
                        }

                        if (!string.IsNullOrEmpty(op.service) && op.service != "FrontService")
                        {
                            Type serviceType = op.service switch
                            {
                                "MatrisArithmeticService" => typeof(MatrisArithmeticService<object>),
                                "SpecialMatricesService" => typeof(SpecialMatricesService),
                                _ => throw new Exception(CompilerMessage.UNKNOWN_SERVICE(op.service))
                            };

                            // Construct service
                            serviceObject = serviceType.GetConstructor(Type.EmptyTypes)
                                                       .Invoke(Array.Empty<object>());

                            // Get the method
                            method = serviceType.GetMethod(op.name);
                            paraminfo = method.GetParameters();
                        }

                        // Parse values from tokens to arguments and check if they are referenced correctly
                        param_arg = CheckAndParseArgumentsAndHints(op, operands, param_arg, paraminfo, matDict);

                        // Replace nulls with default values
                        param_arg = ParseDefaultValues(op.paramCount, param_arg, paraminfo);

                        // Invoke method found with given arguments
                        return InvokeMethods(op, operands, serviceObject, method, param_arg);

                    }
            }

            operands[0].name = "";
            return operands[0];
        }

        /// <summary>
        /// Set given command as docs command with output related to given <paramref name="tkn"/>
        /// </summary>
        /// <param name="cmd">Command to make changes to</param>
        /// <param name="tkn">Token to use as help reference</param>
        /// <param name="matdict">Matrix dictionary to reference to</param>
        /// <returns>Given <paramref name="cmd"/> updated with helpful message</returns>
        private static Command SetDocsCommand(Command cmd,
                                       Token tkn,
                                       Dictionary<string, MatrisBase<dynamic>> matdict)
        {
            switch (tkn.info)
            {
                case "matrix":
                    {
                        if (matdict.ContainsKey(tkn.name))
                        {
                            cmd.STATE = CommandState.SUCCESS;
                            cmd.SetStateMessage(CommandStateMessage.DOCS_MAT_FOUND(tkn.name));
                            cmd.Output = matdict[tkn.name].Details(tkn.name);
                        }
                        else
                        {
                            cmd.STATE = CommandState.ERROR;
                            cmd.SetStateMessage(CommandStateMessage.DOCS_NOT_MAT_FUNC(tkn.name));
                        }
                        break;
                    }
                case "info":    // info about how to use the compiler
                    {
                        cmd.STATE = CommandState.SUCCESS;
                        cmd.SetStateMessage(CommandStateMessage.SUCCESS_COMPILER_DOCS);
                        cmd.Output = CompilerMessage.COMPILER_HELP;
                        break;
                    }
                case "null":    // nothing found
                    {
                        cmd.STATE = CommandState.ERROR;
                        cmd.SetStateMessage(!string.IsNullOrEmpty(tkn.name.Trim())
                            ? CommandStateMessage.DOCS_NOT_MAT_FUNC(tkn.name)
                            : !string.IsNullOrEmpty(tkn.symbol.Trim())
                                ? CommandStateMessage.DOCS_NONE_FOUND(tkn.symbol)
                                : (string)CommandStateMessage.DOCS_NONE_FOUND(tkn.val));

                        break;
                    }
                default:        // given name was a function, and also possibly a matrix
                    {
                        if (matdict.ContainsKey(tkn.name))
                        {
                            cmd.STATE = CommandState.SUCCESS;
                            cmd.SetStateMessage(CommandStateMessage.DOCS_MAT_FUNC_FOUND(tkn.name));
                            cmd.Output = matdict[tkn.name].Details(tkn.name) +
                                "\nKomut: " + tkn.name + "\n" +
                                tkn.info;
                        }
                        else
                        {
                            cmd.STATE = CommandState.SUCCESS;
                            cmd.SetStateMessage(CommandStateMessage.DOCS_FUNC_FOUND(tkn.name));
                            cmd.Output = tkn.info;
                        }
                        break;
                    }
            }
            return cmd;
        }
        #endregion

        #region FrontService Methods
        public void AddToMatrisDict(string name,
                                    MatrisBase<dynamic> matris,
                                    Dictionary<string, MatrisBase<dynamic>> matdict)
        {
            if (matdict == null)
            {
                throw new Exception(CompilerMessage.MAT_DICT_INVALID);
            }

            if (matdict.Count >= (int)MatrisLimits.forMatrisCount)
            {
                throw new Exception(CompilerMessage.MAT_LIMIT);
            }

            if (Validations.ValidMatrixName(name)) // Check again just in case
            {
                matdict.Add(name, matris);
            }
        }

        public bool DeleteFromMatrisDict(string name,
                                         Dictionary<string, MatrisBase<dynamic>> matdict)
        {
            if (matdict == null)
            {
                return false;
            }

            if (matdict.ContainsKey(name))
            {
                matdict[name].Dispose();
                matdict.Remove(name);
                return true;
            }
            return false;
        }

        public Command CreateCommand(string cmd)
        {
            return new Command(cmd);
        }

        public List<CommandLabel> GetBuiltInCommands()
        {
            return builtInCommands;
        }

        public void SetBuiltInCommands(List<CommandLabel> value)
        {
            builtInCommands ??= value;
        }

        public void ReadCommandInformation()
        {
            using StreamReader r = new StreamReader("_builtInCmds.json");
            string json = r.ReadToEnd();
            builtInCommands = JsonConvert.DeserializeObject<List<CommandLabel>>(json);
        }

        public List<CommandLabel> GetCommandLabelList(List<string> filter = null)
        {
            if (filter == null)
            {
                return GetBuiltInCommands();
            }

            List<CommandLabel> filtered = new List<CommandLabel>();
            foreach (CommandLabel lbl in GetBuiltInCommands())
            {
                if (filter.Contains(lbl.Label))
                {
                    filtered.Add(lbl);
                }
            }
            return filtered;
        }

        public void SetMatrixDicts(Dictionary<string, MatrisBase<dynamic>> dict,
                                   Dictionary<string, List<List<object>>> vals,
                                   Dictionary<string, Dictionary<string, dynamic>> opts)
        {
            if (dict == null)
            {
                if (vals != null)
                {
                    vals.Clear();
                }

                if (opts != null)
                {
                    opts.Clear();
                }
                return;
            }

            if (vals == null)
            {
                vals = new Dictionary<string, List<List<object>>>();
            }
            if (opts == null)
            {
                opts = new Dictionary<string, Dictionary<string, dynamic>>();
            }

            foreach (string name in dict.Keys)
            {
                if (vals.ContainsKey(name))
                {
                    vals[name].Clear();
                    vals.Remove(name);
                }
                vals.Add(name, dict[name].GetValues());

                if (opts.ContainsKey(name))
                {
                    opts[name].Clear();
                    opts.Remove(name);
                }

                opts.Add(name, new Dictionary<string, dynamic>
                               {
                                   { "seed",dict[name].Seed },
                                   { "isRandom",dict[name].CreatedFromSeed}
                               }
                        );
            }
        }

        public List<Token> ShuntingYardAlg(List<Token> tkns)
        {
            if (tkns == null)
            {
                return new List<Token>();
            }

            Queue<Token> outputQueue = new Queue<Token>();
            Stack<Token> operatorStack = new Stack<Token>();
            Stack<bool> valtracker = new Stack<bool>();
            Stack<int> argcounter = new Stack<int>();

            int ind = 0;
            while (ind < tkns.Count)
            {
                Token tkn = tkns[ind];
                if (tkn.tknType == TokenType.NUMBER || tkn.tknType == TokenType.MATRIS || tkn.tknType == TokenType.DOCS || tkn.tknType == TokenType.NULL)        // NUMBER | MATRIX | INFORMATION
                {
                    outputQueue.Enqueue(tkn);

                    if (valtracker.Count > 0)
                    {
                        valtracker.Pop();
                        valtracker.Push(true);
                    }
                }

                else if (tkn.tknType == TokenType.FUNCTION)   // FUNCTION
                {
                    operatorStack.Push(tkn);
                    argcounter.Push(0);
                    if (valtracker.Count > 0)
                    {
                        valtracker.Pop();
                        valtracker.Push(true);
                    }
                    valtracker.Push(false);

                }
                else if (tkn.tknType == TokenType.ARGSEPERATOR)     // ARGUMENT SEPERATOR
                {
                    while (operatorStack.Peek().tknType != TokenType.LEFTBRACE)
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                        if (operatorStack.Count == 0)
                        {
                            throw new Exception(CompilerMessage.PARANTHESIS_FORMAT_ERROR);
                        }
                    }
                    if (valtracker.Pop())
                    {
                        int a = argcounter.Pop();
                        a++;
                        argcounter.Push(a);
                    }
                    valtracker.Push(false);
                }

                else if (tkn.tknType == TokenType.OPERATOR)  // OPERATOR
                {
                    while (operatorStack.Count != 0)
                    {
                        Token o2 = operatorStack.Peek();
                        if (o2.tknType != TokenType.OPERATOR)
                        {
                            break;
                        }
                        else if ((tkn.assoc == OperatorAssociativity.LEFT && tkn.priority == o2.priority) || (tkn.priority < o2.priority))
                        {
                            outputQueue.Enqueue(operatorStack.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }
                    operatorStack.Push(tkn);
                }

                else if (tkn.tknType == TokenType.LEFTBRACE)    // LEFT BRACE
                {
                    operatorStack.Push(tkn);
                }
                else if (tkn.tknType == TokenType.RIGHTBRACE)   // RIGHT BRACE
                {
                    while (operatorStack.Peek().tknType != TokenType.LEFTBRACE)
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                        if (operatorStack.Count == 0)
                        {
                            throw new Exception(CompilerMessage.PARANTHESIS_FORMAT_ERROR);
                        }
                    }
                    operatorStack.Pop();

                    if (operatorStack.Count > 0)
                    {
                        if (operatorStack.Peek().tknType == TokenType.FUNCTION)
                        {
                            Token functkn = operatorStack.Pop();
                            int args = argcounter.Pop();
                            if (valtracker.Pop())
                            {
                                args++;
                            }

                            functkn.argCount = args;
                            outputQueue.Enqueue(functkn);
                        }
                    }


                }
                ind++;
            }

            while (operatorStack.Count != 0)
            {
                if ((operatorStack.Peek().tknType == TokenType.LEFTBRACE) || (operatorStack.Peek().tknType == TokenType.RIGHTBRACE))
                {
                    throw new Exception(CompilerMessage.PARANTHESIS_FORMAT_ERROR);
                }

                outputQueue.Enqueue(operatorStack.Pop());
            }

            return new List<Token>(outputQueue.ToArray());
        }

        public List<Token> Tokenize(string exp)
        {
            string[] explist = TokenizeSplit(exp);
            List<Token> tkns = new List<Token>();

            foreach (string e in explist)
            {
                Token tkn = String2Token(e);
                // Decide for unary or binary
                if (e == "-" || e == "+")
                {
                    // Started with - , unary
                    if (tkns.Count == 0)
                    { tkn.SetValues("u" + e, OperatorAssociativity.RIGHT, 200, 1); }
                    // Previous was a left bracet or an operator
                    else if (tkns[^1].tknType == TokenType.LEFTBRACE || tkns[^1].tknType == TokenType.OPERATOR || tkns[^1].tknType == TokenType.ARGSEPERATOR)
                    { tkn.SetValues("u" + e, OperatorAssociativity.RIGHT, 200, 1); }
                }
                tkns.Add(tkn);
            }
            return tkns;

        }

        public CommandInfo TryParseBuiltFunc(string name)
        {
            if (GetBuiltInCommands() == null)
            {
                ReadCommandInformation();
            }
            else if (GetBuiltInCommands().Count == 0)
            {
                ReadCommandInformation();
            }

            foreach (CommandInfo cinfo in from CommandLabel lbl in GetBuiltInCommands()
                                          from CommandInfo cinfo in new List<CommandInfo>(lbl.Functions)
                                          where cinfo.Function == name
                                          select cinfo)
            {
                return cinfo.Copy();
            }

            return null;
        }

        public CommandState EvaluateCommand(Command cmd,
                                            Dictionary<string, MatrisBase<dynamic>> matdict,
                                            List<Command> cmdHistory)
        {
            if (cmd == null)
            {
                return CommandState.ERROR;
            }

            List<Token> tkns = cmd.GetTokens();

            switch (cmd.STATE)
            {
                // Komut ilk defa işlenmekte
                case CommandState.IDLE:
                    {
                        cmd.STATE = CommandState.UNAVAILABLE;

                        // Tek terim verildi
                        if (tkns.Count == 1)
                        {
                            if (Validations.ValidMatrixName(tkns[0].name) && tkns[0].tknType == TokenType.MATRIS)
                            {
                                if (matdict != null)
                                {
                                    if (matdict.ContainsKey(tkns[0].name))
                                    {
                                        cmd.STATE = CommandState.SUCCESS;
                                        cmd.SetStateMessage(CommandStateMessage.SUCCESS_RET_MAT);
                                        cmd.Output = matdict[tkns[0].name].ToString();
                                    }
                                    else // Return null if no matrix found with given name
                                    {
                                        cmd.STATE = CommandState.SUCCESS;
                                        cmd.SetStateMessage(CommandStateMessage.SUCCESS_RET_NULL);
                                        cmd.Output = "null";
                                    }
                                }
                                else
                                {
                                    cmd.STATE = CommandState.SUCCESS;
                                    cmd.SetStateMessage(CommandStateMessage.SUCCESS_RET_NULL);
                                    cmd.Output = "null";
                                }
                                return CommandState.SUCCESS;
                            }
                            else
                            {
                                switch (tkns[0].tknType)
                                {
                                    case TokenType.NUMBER:
                                        {
                                            cmd.STATE = CommandState.SUCCESS;
                                            cmd.SetStateMessage(CommandStateMessage.SUCCESS_RET_NUM);
                                            cmd.Output = tkns[0].val.ToString();
                                            return CommandState.SUCCESS;
                                        }
                                    case TokenType.NULL:
                                        {
                                            cmd.STATE = CommandState.SUCCESS;
                                            cmd.SetStateMessage(CommandStateMessage.SUCCESS_RET_NULL);
                                            cmd.Output = "null";
                                            return CommandState.SUCCESS;
                                        }
                                    case TokenType.DOCS:
                                        {
                                            cmd = SetDocsCommand(cmd, tkns[0], matdict
                                                                               ?? new Dictionary<string, MatrisBase<dynamic>>());
                                            return cmd.STATE;
                                        }
                                    case TokenType.OPERATOR:
                                        {
                                            tkns[0].symbol = (tkns[0].symbol == "u+" || tkns[0].symbol == "u-") ? tkns[0].symbol[1].ToString() : tkns[0].symbol;
                                            cmd.STATE = CommandState.ERROR;
                                            cmd.SetStateMessage(CompilerMessage.OP_CANT_BE_ALONE(tkns[0].symbol));
                                            return CommandState.ERROR;
                                        }
                                    case TokenType.FUNCTION:
                                        {
                                            if (tkns[0].paramCount == 0)
                                            {
                                                break;
                                            }

                                            cmd.STATE = CommandState.WARNING;
                                            cmd.SetStateMessage(CompilerMessage.FUNC_REQUIRES_ARGS(tkns[0].name, tkns[0].paramCount));
                                            return CommandState.WARNING;
                                        }
                                    default:
                                        {
                                            cmd.STATE = CommandState.ERROR;
                                            cmd.SetStateMessage(tkns[0].tknType.ToString() + " tipi terim tek başına kullanılamaz ");
                                            return CommandState.ERROR;
                                        }
                                }
                            }
                        }

                        // More than a single token
                        int ind = 0;
                        Stack<Token> operandStack = new Stack<Token>();
                        try
                        {
                            while (ind < tkns.Count)
                            {
                                Token tkn = tkns[ind];
                                if (tkn.tknType == TokenType.NUMBER || tkn.tknType == TokenType.MATRIS || tkn.tknType == TokenType.NULL)
                                {
                                    operandStack.Push(tkn);
                                }
                                else
                                {
                                    List<Token> operands = new List<Token>();

                                    if (tkn.tknType == TokenType.FUNCTION)
                                    {
                                        if (operandStack.Count < tkn.argCount)
                                        {
                                            throw new Exception(CompilerMessage.ARG_COUNT_ERROR);
                                        }

                                        for (int i = 0; i < tkn.argCount; i++)
                                        {
                                            operands.Add(operandStack.Pop());
                                        }
                                    }
                                    else
                                    {
                                        if (operandStack.Count < tkn.paramCount)
                                        {
                                            throw new Exception(CompilerMessage.ARG_COUNT_ERROR);
                                        }

                                        for (int i = 0; i < tkn.paramCount; i++)
                                        {
                                            operands.Add(operandStack.Pop());
                                        }
                                    }

                                    operandStack.Push(EvalOperator(tkn, operands, matdict));

                                }
                                ind++;
                            }

                            if (operandStack.Count == 1)
                            {
                                if (operandStack.Peek().tknType == TokenType.VOID)
                                {
                                    cmd.Output = CommandStateMessage.SUCCESS_RET_NULL;
                                    cmd.STATE = CommandState.SUCCESS;
                                }
                                else
                                {
                                    cmd.Output = operandStack.Pop().val.ToString();
                                    cmd.STATE = CommandState.SUCCESS;
                                    cmd.SetStateMessage(CommandStateMessage.SUCCESS_RET_NULL);
                                }
                            }
                            else if (operandStack.Count == 0)
                            {
                                cmd.STATE = CommandState.ERROR;

                                cmd.SetStateMessage(tkns.Count == 0 ? CompilerMessage.OP_INVALID(cmd.GetTermsToEvaluate()[0]) : CompilerMessage.ARG_COUNT_ERROR);
                            }
                            else
                            {
                                cmd.STATE = CommandState.ERROR;
                                cmd.SetStateMessage(CompilerMessage.CMD_FORMAT_ERROR);
                            }
                        }
                        catch (Exception err)
                        {
                            cmd.STATE = CommandState.ERROR;
                            cmd.SetStateMessage(err.Message);
                        }

                        break;
                    }
                // Komut işlenmekte veya hatalı
                case CommandState.UNAVAILABLE:
                    {
                        cmd.SetStateMessage(CommandStateMessage.CMD_UNAVAILABLE(cmd.OriginalCommand));
                        break;
                    }
                // Komut zaten işlenmiş
                default:
                    {
                        cmd.SetStateMessage(CommandStateMessage.CMD_COMPILED(cmd.STATE, cmd.GetStateMessage()));
                        break;
                    }

            }

            if (CleanUp_state && cmd.STATE == CommandState.SUCCESS && cmdHistory != null)
            {
                foreach (Command c in cmdHistory)
                {
                    c.Dispose();
                }
                cmdHistory.Clear();

                CleanUp_state = false;
                cmd.SetStateMessage(CommandStateMessage.SUCCESS_CLEANUP);
                if (!cmd.GetNameSettings().ContainsKey("display"))
                {
                    cmd.GetNameSettings().Add("display", "none");
                }
            }

            return cmd.STATE;
        }

        public bool CheckCmdDate(DateTime calldate)
        {
            // First command
            if (calldate == null)
            {
                return true;
            }
            else if ((DateTime.Now - calldate).TotalSeconds < (int)CompilerLimits.forCmdSendRateInSeconds)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void CleanUp()
        {
            CleanUp_state = true;
        }

        public string Help()
        {
            return CompilerMessage.COMPILER_HELP;
        }

        #endregion

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (builtInCommands != null)
                    {
                        foreach (CommandLabel c in builtInCommands)
                        {
                            c.Dispose();
                        }

                        builtInCommands = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~FrontService()
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
