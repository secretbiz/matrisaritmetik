using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
                        tkn.SetValues(":", OperatorAssociativity.RIGHT, 2, 2);
                        break;
                    }
                case "=":
                    {
                        tkn.SetValues("=", OperatorAssociativity.RIGHT, 0, 2);
                        break;
                    }
                case "+":
                    {
                        tkn.SetValues("+", OperatorAssociativity.LEFT, 3, 2);
                        break;
                    }
                case "-":
                    {
                        tkn.SetValues("-", OperatorAssociativity.LEFT, 3, 2);
                        break;
                    }
                case "*":
                    {
                        tkn.SetValues("*", OperatorAssociativity.LEFT, 4, 2);
                        break;
                    }
                case "/":
                    {
                        tkn.SetValues("/", OperatorAssociativity.LEFT, 4, 2);
                        break;
                    }
                case "%":
                    {
                        tkn.SetValues("%", OperatorAssociativity.LEFT, 4, 2);
                        break;
                    }
                case "^":
                    {
                        tkn.SetValues("^", OperatorAssociativity.LEFT, 5, 2);
                        break;
                    }
                case ".^":
                    {
                        tkn.SetValues(".^", OperatorAssociativity.LEFT, 5, 2);
                        break;
                    }
                case ".*":
                    {
                        tkn.SetValues(".*", OperatorAssociativity.LEFT, 5, 2);
                        break;
                    }
                case "./":
                    {
                        tkn.SetValues("./", OperatorAssociativity.LEFT, 6, 2);
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
            else if (Constants.Contains(value))
            {
                tkn.name = value;
                tkn.info = "special";
            }
            else if (Validations.ValidMatrixName(value))
            {   // MATRIX
                tkn.name = value;
                tkn.info = "matrix";
            }
            else if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(tkn.info)) // Spammed bunch of question marks or request !Help on a number
            {
                tkn.info = "info";
            }
            else    // NOTHING FOUND
            {
                tkn.name = string.IsNullOrWhiteSpace(value) ? ((object)tkn.val).ToString() : value;
                tkn.val = tkn.name;
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
            if (Constants.Contains(name))
            {
                tkn.val = Constants.Get(name);
                tkn.tknType = tkn.val is None ? TokenType.NULL : TokenType.NUMBER;
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Try to create a function token from if given <paramref name="name"/> is a built-in function name
        /// </summary>
        /// <param name="tkn">Token to apply values to</param>
        /// <param name="name">Function name to search and use</param>
        /// <returns>True if token was set to be a function token</returns>
        private bool SetAsFunctionToken(Token tkn,
                                        string name,
                                        CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            using CommandInfo cmdinfo = TryParseBuiltFunc(name);
            if (cmdinfo != null) // VALID FUNCTION
            {
                Validations.CheckModeAndReturnType(mode, cmdinfo.Returns.ToString());

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
                                          string exp,
                                          CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            if (exp[0] == '!')          // A function or a special value
            {
                exp = exp.Replace("!", "");
                if (!SetAsFunctionToken(tkn, exp, mode))
                {
                    if (!SetAsSpecialToken(tkn, exp))
                    {
                        throw new Exception(CompilerMessage.NOT_A_(exp, "fonksiyon ya da özel bir değer"));
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
        private Token String2Token(string exp,
                                   CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
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

            else if (exp.Length > 1
                     && exp != ".^"
                     && exp != ".*"
                     && exp != "./")
            {
                SetSpecialValueToken(tkn, exp, mode);
            }

            else if (Validations.ValidMatrixName(exp))
            {
                tkn.tknType = TokenType.MATRIS;                   // MATRIX
                tkn.name = exp;
            }

            else
            {                                                       // OPERATOR
                tkn = SetTokenFieldsViaSymbol(tkn, exp);
            }

            return tkn;
        }

        /// <summary>
        /// Set given expression <paramref name="exp"/> as an assignment expression
        /// </summary>
        /// <param name="exp">Expression to use</param>
        /// <returns>Properly split valid assignment expression</returns>
        private static void SetExpressionForAssignOP(ref string exp)
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

                string symbol = expsplits[0][^1].ToString();
                string[] symbols = new string[] { "+", "-", "*", "/", "^", "%" };

                StringBuilder res = new StringBuilder();

                if (Array.IndexOf(symbols, symbol) == -1)
                {
                    if (!Validations.ValidMatrixName(expsplits[0]))
                    {
                        throw new Exception(CompilerMessage.MAT_NAME_INVALID);
                    }
                    res.Append(expsplits[0])
                       .Append(' ')
                       .Append('=')
                       .Append(' ')
                       .Append(expsplits[1]);

                    exp = res.ToString();   // Matris_name = (some_expression)
                }
                else
                {
                    expsplits[0] = expsplits[0][0..^1].Trim();
                    res.Append(expsplits[0])
                       .Append(' ')
                       .Append('=')
                       .Append(' ')
                       .Append(expsplits[0])
                       .Append(' ')
                       .Append(symbol)
                       .Append(' ')
                       .Append('(')
                       .Append(' ')
                       .Append(expsplits[1])
                       .Append(' ')
                       .Append(')');

                    exp = res.ToString();   // Matris_name = (some_expression)
                }

            }
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
                SetExpressionForAssignOP(ref exp);
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
                                    object[] arguments,
                                    Dictionary<string, MatrisBase<dynamic>> matDict,
                                    CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
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
                                return HelpInternal(operands.Count != 0 ? operands[0] : null, matDict, mode);
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
                            "Veri Tablosu" => TokenType.MATRIS,
                            "Matris" => TokenType.MATRIS,
                            "int" => TokenType.NUMBER,
                            "float" => TokenType.NUMBER,
                            "string" => TokenType.STRING,
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
                                                 Dictionary<string, MatrisBase<dynamic>> matDict,
                                                 CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            switch (operand.tknType)
            {
                case TokenType.NULL: arguments[argIndex] = null; break;

                case TokenType.NUMBER: arguments[argIndex] = operand.val; break;

                case TokenType.MATRIS:
                    {
                        if (matDict.ContainsKey(operand.name))
                        {
                            Validations.CheckModeAndMatrixReference(mode, matDict[operand.name]);
                            arguments[argIndex] = matDict[operand.name];
                        }
                        else if (operand.val is MatrisBase<object>)
                        {
                            Validations.CheckModeAndMatrixReference(mode, operand.val);
                            arguments[argIndex] = (object)operand.val;
                        }
                        else if (Validations.ValidMatrixName(operand.name))
                        {
                            operand.tknType = TokenType.STRING;
                            arguments[argIndex] = operand.name;
                        }
                        else
                        {
                            throw new Exception(CompilerMessage.UNKNOWN_VARIABLE(operand.name));
                        }

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
        /// <param name="matDict">Matrix dictionary to reference to if needed</param>
        /// <param name="mode"></param>
        /// <returns>Parsed <paramref name="arguments"/></returns>
        private static object[] CheckAndParseArgumentsAndHints(Token op,
                                                               List<Token> operands,
                                                               object[] arguments,
                                                               ParameterInfo[] paraminfo,
                                                               Dictionary<string, MatrisBase<dynamic>> matDict,
                                                               CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
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

                arguments[pind] = ApplyArgumentValue(op, operands[k], arguments, pind, matDict, mode);

                if (arguments[pind] != null) // Parse given value if not null
                {
                    arguments[pind] = ParseTokenValAsParameterType(op, operands, arguments, k, pind, mode);
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
                                                    int argumentIndex,
                                                    CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
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
                            Validations.CheckModeAndMatrixReference(mode, arguments[argumentIndex]);
                            arguments[argumentIndex] = (MatrisBase<dynamic>)arguments[argumentIndex];
                            break;
                        }
                    case "Veri Tablosu":
                        {
                            Validations.CheckModeAndMatrixReference(mode, arguments[argumentIndex]);
                            arguments[argumentIndex] = (Dataframe)arguments[argumentIndex];
                            break;
                        }
                    case "dinamik":
                        {
                            arguments[argumentIndex] = (dynamic)arguments[argumentIndex];
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
        /// Evaluate expressino where <paramref name="op"/> is <see cref="TokenType.OPERATOR"/> type
        /// </summary>
        /// <param name="op">Operator token, type <see cref="TokenType.OPERATOR"/></param>
        /// <param name="operands">List of operands</param>
        /// <param name="matDict">Matrix dictionary to refer to if necessary</param>
        /// <returns>Operands list after evaluation</returns>
        private static List<Token> EvalWithSymbolOperator(Token op,
                                                          List<Token> operands,
                                                          Dictionary<string, MatrisBase<dynamic>> matDict,
                                                          CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            switch (op.symbol)
            {
                case "+":
                    {
                        CompilerUtils.OPBasic("+", operands[1], operands[0], matDict, mode);
                        break;
                    }
                case "-":
                    {
                        CompilerUtils.OPBasic("-", operands[1], operands[0], matDict, mode);
                        break;
                    }
                case "*":
                    {
                        CompilerUtils.OPBasic("*", operands[1], operands[0], matDict, mode);
                        break;
                    }
                case "/":
                    {
                        CompilerUtils.OPBasic("/", operands[1], operands[0], matDict, mode);
                        break;
                    }
                case "%":
                    {
                        CompilerUtils.OPModulo(operands[1], operands[0], matDict, mode);
                        break;
                    }
                case "^":   // A^3 == A'nın elemanlarının 3. kuvvetleri
                    {
                        CompilerUtils.OPExpo(operands[1], operands[0], matDict, mode);
                        break;
                    }
                case ".^":      // A.^3 == A@A@A  , A kare matris
                    {
                        CompilerUtils.OPMatMulByExpo(operands[1], operands[0], matDict, mode);
                        break;
                    }
                case ".*":
                    {
                        CompilerUtils.OPMatMul(operands[1], operands[0], matDict, mode);
                        break;
                    }
                case "./":
                    {
                        CompilerUtils.OPMatMulWithInverse(operands[1], operands[0], matDict, mode);
                        break;
                    }
                case "u-":
                    {
                        CompilerUtils.CheckMatrixAndUpdateVal(operands[0], matDict, mode);
                        operands[0].val = -operands[0].val;

                        break;
                    }
                case "u+":
                    {
                        CompilerUtils.CheckMatrixAndUpdateVal(operands[0], matDict, mode);
                        break;
                    }
                case "=":
                    {
                        CompilerUtils.OPAssignment(operands[1], operands[0], matDict, mode);
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
                                   Dictionary<string, MatrisBase<dynamic>> matDict,
                                   CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            switch (op.tknType)
            {
                case TokenType.OPERATOR: // OPERATORS
                    {
                        operands = EvalWithSymbolOperator(op, operands, matDict, mode);
                        break;
                    }

                default:  // FUNCTIONS
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
                        if (!string.IsNullOrEmpty(op.service))
                        {
                            Type serviceType = op.service switch
                            {
                                "MatrisArithmeticService" => typeof(MatrisArithmeticService),
                                "SpecialMatricesService" => typeof(SpecialMatricesService),
                                "StatisticsService" => typeof(StatisticsService),
                                "FrontService" => typeof(FrontService),
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
                        param_arg = CheckAndParseArgumentsAndHints(op, operands, param_arg, paraminfo, matDict, mode);

                        // Replace nulls with default values
                        param_arg = CompilerUtils.ParseDefaultValues(op.paramCount, param_arg, paraminfo);

                        // Invoke method found with given arguments
                        return InvokeMethods(op, operands, serviceObject, method, param_arg, matDict, mode);

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
                                              Dictionary<string, MatrisBase<dynamic>> matdict,
                                              CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            switch (tkn.info)
            {
                case "matrix":
                    {
                        if (matdict.ContainsKey(tkn.name))
                        {
                            Validations.CheckModeAndMatrixReference(mode, matdict[tkn.name]);
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
                        cmd.Output = CompilerMessage.COMPILER_HELP(mode);
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
                case "special":
                    {
                        if (matdict.ContainsKey(tkn.name))
                        {
                            Validations.CheckModeAndMatrixReference(mode, matdict[tkn.name]);
                            cmd.STATE = CommandState.SUCCESS;
                            cmd.SetStateMessage(CommandStateMessage.DOCS_MAT_SPECIAL_FOUND(tkn.name));
                            cmd.Output = matdict[tkn.name].Details(tkn.name)
                                         + "\nÖzel değer: "
                                         + tkn.name
                                         + "\n"
                                         + Constants.Description(tkn.name);
                        }
                        else
                        {
                            cmd.STATE = CommandState.SUCCESS;
                            cmd.SetStateMessage(CommandStateMessage.DOCS_SPECIAL_FOUND(tkn.name));
                            cmd.Output = Constants.Description(tkn.name);
                        }
                        break;
                    }
                default:        // given name was a function, and also possibly a matrix
                    {
                        if (matdict.ContainsKey(tkn.name))
                        {
                            Validations.CheckModeAndMatrixReference(mode, matdict[tkn.name]);
                            cmd.STATE = CommandState.SUCCESS;
                            cmd.SetStateMessage(CommandStateMessage.DOCS_MAT_FUNC_FOUND(tkn.name));
                            cmd.Output = matdict[tkn.name].Details(tkn.name)
                                         + "\nKomut: "
                                         + tkn.name
                                         + "\n"
                                         + tkn.info;
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

        /// <summary>
        /// Sets given token's value to documentation of about the subject given in <see cref="Token.info"/>
        /// </summary>
        /// <param name="tkn">Token to use</param>
        /// <param name="matdict">Matrix dictionary to reference to</param>
        /// <returns>Given <paramref name="tkn"/> with updated <see cref="Token.val"/></returns>
        private static void SetDocsAsValue(Token tkn,
                                           Dictionary<string, MatrisBase<dynamic>> matdict,
                                           CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            switch (tkn.info)
            {
                case "matrix":
                    {
                        if (matdict.ContainsKey(tkn.name))
                        {
                            tkn.val = matdict[tkn.name].Details(tkn.name);
                        }
                        break;
                    }
                case "info":    // info about how to use the compiler
                    {
                        tkn.val = CompilerMessage.COMPILER_HELP(mode);
                        break;
                    }
                case "null":    // nothing found, return as is
                    {
                        tkn.val = ((object)tkn.val).ToString();
                        break;
                    }
                case "special":
                    {
                        if (matdict.ContainsKey(tkn.name))
                        {
                            tkn.val = matdict[tkn.name].Details(tkn.name)
                                      + "\nÖzel değer: "
                                      + tkn.name
                                      + "\n"
                                      + Constants.Description(tkn.name);
                        }
                        else
                        {
                            tkn.val = Constants.Description(tkn.name);
                        }
                        break;
                    }
                default:        // given name was a function, and also possibly a matrix
                    {
                        if (matdict.ContainsKey(tkn.name))
                        {
                            tkn.val = matdict[tkn.name].Details(tkn.name)
                                      + "\nKomut: "
                                      + tkn.name
                                      + "\n"
                                      + tkn.info;
                        }
                        else
                        {
                            tkn.val = tkn.info;
                        }
                        break;
                    }
            }
        }


        /// <summary>
        /// Wrapper-like for <see cref="Help(dynamic)"/>
        /// </summary>
        /// <param name="term">Token to use as reference, null to get <see cref="CompilerMessage.COMPILER_HELP"/></param>
        /// <param name="matDict">Matrix dictionary to refer to</param>
        /// <returns>Token with updated docs</returns>
        private Token HelpInternal(Token term,
                                   Dictionary<string, MatrisBase<dynamic>> matDict,
                                   CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            if (term is null || term.tknType == TokenType.NULL)
            {
                return new Token() { tknType = TokenType.DOCS, name = "info", val = CompilerMessage.COMPILER_HELP(mode) };
            }

            SetAsDocsToken(term, term.name);

            SetDocsAsValue(term, matDict, mode);

            return term;
        }

        /// <summary>
        /// Handle single token commands, update <paramref name="cmd"/> command accordingly
        /// </summary>
        /// <param name="cmd">Command to update</param>
        /// <param name="tkn">Token in the command to update</param>
        /// <param name="matdict">Matrix dictionary to refer to</param>
        /// <returns>Commands <see cref="CommandState"/> after evaluation or '<see cref="null"/>' if nothing worked</returns>
        private CommandState? SingleTermCommand(Command cmd,
                                                Token tkn,
                                                Dictionary<string, MatrisBase<dynamic>> matdict,
                                                CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            if (Validations.ValidMatrixName(tkn.name) && tkn.tknType == TokenType.MATRIS)
            {
                if (matdict != null)
                {
                    if (matdict.ContainsKey(tkn.name))
                    {
                        Validations.CheckModeAndMatrixReference(mode, matdict[tkn.name]);
                        cmd.STATE = CommandState.SUCCESS;
                        cmd.SetStateMessage(CommandStateMessage.SUCCESS_RET_MAT);
                        cmd.Output = matdict[tkn.name].ToString();
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
                switch (tkn.tknType)
                {
                    case TokenType.NUMBER:
                        {
                            cmd.STATE = CommandState.SUCCESS;
                            cmd.SetStateMessage(CommandStateMessage.SUCCESS_RET_NUM);
                            cmd.Output = tkn.val.ToString();
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
                            cmd = SetDocsCommand(cmd,
                                                 tkn,
                                                 matdict ?? new Dictionary<string, MatrisBase<dynamic>>(),
                                                 mode);
                            return cmd.STATE;
                        }
                    case TokenType.OPERATOR:
                        {
                            tkn.symbol = (tkn.symbol == "u+" || tkn.symbol == "u-") ? tkn.symbol[1].ToString() : tkn.symbol;
                            cmd.STATE = CommandState.ERROR;
                            cmd.SetStateMessage(CompilerMessage.OP_CANT_BE_ALONE(tkn.symbol));
                            return CommandState.ERROR;
                        }
                    case TokenType.FUNCTION:
                        {
                            using CommandInfo cinfo = TryParseBuiltFunc(tkn.name);
                            if (cinfo.Required_params == 0)
                            {
                                break;
                            }

                            cmd.STATE = CommandState.WARNING;
                            cmd.SetStateMessage(CompilerMessage.FUNC_REQUIRES_ARGS(tkn.name, tkn.paramCount));
                            return CommandState.WARNING;
                        }
                    default:
                        {
                            cmd.STATE = CommandState.ERROR;
                            cmd.SetStateMessage(tkn.tknType.ToString() + " tipi terim tek başına kullanılamaz ");
                            return CommandState.ERROR;
                        }
                }
            }

            return null;
        }

        /// <summary>
        /// Iterate through given <paramref name="tkns"/> and evaluate each of them,
        /// update <paramref name="operandStack"/> accordingly
        /// </summary>
        /// <param name="tkns">List of tokens to evaluate</param>
        /// <param name="operandStack">Operand stack to push arguments and results to</param>
        /// <param name="matdict">Matrix dictionary to refer to</param>
        private void EvaluateTokens(List<Token> tkns,
                                    Stack<Token> operandStack,
                                    Dictionary<string, MatrisBase<dynamic>> matdict,
                                    CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            int ind = 0;
            while (ind < tkns.Count)
            {
                Token tkn = tkns[ind];
                if (tkn.tknType == TokenType.NUMBER
                    || tkn.tknType == TokenType.MATRIS
                    || tkn.tknType == TokenType.NULL)
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

                    operandStack.Push(EvalOperator(tkn, operands, matdict, mode));
                }
                ind++;
            }
        }

        /// <summary>
        /// Evaluate given IDLE command
        /// </summary>
        /// <param name="cmd">Command to evaluate</param>
        /// <param name="tkns">Tokens of the command</param>
        /// <param name="matdict">Matrix dictionary to refer to</param>
        private void EvaluateIdleCommand(Command cmd,
                                         List<Token> tkns,
                                         Dictionary<string, MatrisBase<dynamic>> matdict,
                                         CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            Stack<Token> operandStack = new Stack<Token>();
            try
            {
                EvaluateTokens(tkns, operandStack, matdict, mode);

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

                    cmd.SetStateMessage(tkns.Count == 0
                                        ? CompilerMessage.OP_INVALID(cmd.GetTermsToEvaluate()[0])
                                        : CompilerMessage.ARG_COUNT_ERROR);
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

        public void AddToDfDict(string name,
                                Dataframe df,
                                Dictionary<string, Dataframe> dfdict)
        {
            if (dfdict == null)
            {
                throw new Exception(CompilerMessage.MAT_DICT_INVALID);
            }

            if (dfdict.Count >= (int)DataframeLimits.forDataframeCount)
            {
                throw new Exception(CompilerMessage.DF_LIMIT);
            }

            if (Validations.ValidMatrixName(name)) // Check again just in case
            {
                dfdict.Add(name, df);
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

        public bool DeleteFromDfDict(string name,
                                     Dictionary<string, Dataframe> dfdict)
        {
            if (dfdict == null)
            {
                return false;
            }

            if (dfdict.ContainsKey(name))
            {
                dfdict[name].Dispose();
                dfdict.Remove(name);
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
            builtInCommands = value ?? new List<CommandLabel>();
        }

        public void ReadCommandInformation()
        {
            using StreamReader r = new StreamReader("_builtInCmds.json");
            string json = r.ReadToEnd();
            SetBuiltInCommands(JsonConvert.DeserializeObject<List<CommandLabel>>(json));
        }

        public async Task ReadCommandInformationAsync()
        {
            using StreamReader r = new StreamReader("_builtInCmds.json");
            string json = await r.ReadToEndAsync();
            SetBuiltInCommands(JsonConvert.DeserializeObject<List<CommandLabel>>(json));
        }

        public List<CommandLabel> GetCommandLabelList(List<string> filter = null,
                                                      bool blacklist = false)
        {
            if (GetBuiltInCommands() == null || GetBuiltInCommands().Count == 0)
            {
                ReadCommandInformation();
            }

            if (filter == null)
            {
                return blacklist ? GetBuiltInCommands() : new List<CommandLabel>();
            }

            List<CommandLabel> filtered = new List<CommandLabel>();
            if (blacklist)
            {
                List<CommandLabel> cmds = GetBuiltInCommands();
                for (int i = 0; i < cmds.Count; i++)
                {
                    CommandLabel lbl = cmds[i];
                    if (!filter.Contains(lbl.Label))
                    {
                        for (int j = 0; j < lbl.Functions.Length; j++) // Dispose check. TO-DO: Find out where the dispose error is
                        {
                            CommandInfo cinfo = lbl.Functions[j];
                            if (string.IsNullOrWhiteSpace(cinfo.Function))
                            {
                                ReadCommandInformation();
                                cmds = GetBuiltInCommands();
                                lbl = cmds[i];
                                break;
                            }
                        }
                        filtered.Add(lbl);
                    }
                }
            }
            else
            {
                List<CommandLabel> cmds = GetBuiltInCommands();
                for (int i = 0; i < cmds.Count; i++)
                {
                    CommandLabel lbl = cmds[i];
                    if (filter.Contains(lbl.Label))
                    {
                        for (int j = 0; j < lbl.Functions.Length; j++) // Dispose check. TO-DO: Find out where the dispose error is
                        {
                            CommandInfo cinfo = lbl.Functions[j];
                            if (string.IsNullOrWhiteSpace(cinfo.Function))
                            {
                                ReadCommandInformation();
                                cmds = GetBuiltInCommands();
                                lbl = cmds[i];
                                break;
                            }
                        }
                        filtered.Add(lbl);
                    }
                }
            }
            return filtered;
        }

        public async Task<List<CommandLabel>> GetCommandLabelListAsync(List<string> filter = null,
                                                                       bool blacklist = false)
        {
            if (GetBuiltInCommands() == null || GetBuiltInCommands().Count == 0)
            {
                await ReadCommandInformationAsync();
            }

            if (filter == null)
            {
                return blacklist ? GetBuiltInCommands() : new List<CommandLabel>();
            }

            List<CommandLabel> filtered = new List<CommandLabel>();
            if (blacklist)
            {
                List<CommandLabel> cmds = GetBuiltInCommands();
                for (int i = 0; i < cmds.Count; i++)
                {
                    CommandLabel lbl = cmds[i];
                    if (!filter.Contains(lbl.Label))
                    {
                        for (int j = 0; j < lbl.Functions.Length; j++) // Dispose check. TO-DO: Find out where the dispose error is
                        {
                            CommandInfo cinfo = lbl.Functions[j];
                            if (string.IsNullOrWhiteSpace(cinfo.Function))
                            {
                                ReadCommandInformation();
                                cmds = GetBuiltInCommands();
                                lbl = cmds[i];
                                break;
                            }
                        }
                        filtered.Add(lbl);
                    }
                }
            }
            else
            {
                List<CommandLabel> cmds = GetBuiltInCommands();
                for (int i = 0; i < cmds.Count; i++)
                {
                    CommandLabel lbl = cmds[i];
                    if (filter.Contains(lbl.Label))
                    {
                        for (int j = 0; j < lbl.Functions.Length; j++) // Dispose check. TO-DO: Find out where the dispose error is
                        {
                            CommandInfo cinfo = lbl.Functions[j];
                            if (string.IsNullOrWhiteSpace(cinfo.Function))
                            {
                                ReadCommandInformation();
                                cmds = GetBuiltInCommands();
                                lbl = cmds[i];
                                break;
                            }
                        }
                        filtered.Add(lbl);
                    }
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

        public void SetDfDicts(Dictionary<string, Dataframe> dict,
                               Dictionary<string, List<List<object>>> vals,
                               Dictionary<string, Dictionary<string, List<LabelList>>> lbls,
                               Dictionary<string, Dictionary<string, dynamic>> settings)
        {
            if (dict == null)
            {
                if (vals != null)
                {
                    vals.Clear();
                }

                if (lbls != null)
                {
                    lbls.Clear();
                }

                if (settings != null)
                {
                    settings.Clear();
                }
                return;
            }

            if (vals == null)
            {
                vals = new Dictionary<string, List<List<object>>>();
            }
            if (lbls == null)
            {
                lbls = new Dictionary<string, Dictionary<string, List<LabelList>>>();
            }
            if (settings == null)
            {
                settings = new Dictionary<string, Dictionary<string, dynamic>>();
            }

            foreach (string name in dict.Keys)
            {
                if (vals.ContainsKey(name))
                {
                    vals[name].Clear();
                    vals.Remove(name);
                }

                vals.Add(name, dict[name].GetValues());

                if (lbls.ContainsKey(name))
                {
                    lbls[name].Clear();
                    lbls.Remove(name);
                }

                lbls.Add(name, new Dictionary<string, List<LabelList>>
                            {
                                { "col_labels", dict[name].GetColLabels() },
                                { "row_labels", dict[name].GetRowLabels() }
                            }
                        );

                if (settings.ContainsKey(name))
                {
                    settings[name].Clear();
                    settings.Remove(name);
                }

                settings.Add(name, new Dictionary<string, dynamic>
                                {
                                    { "seed", dict[name].Seed },
                                    { "isRandom", dict[name].CreatedFromSeed },
                                    { "col_settings", dict[name].GetColSettings().GetSeperators() },
                                    { "row_settings", dict[name].GetRowSettings().GetSeperators()  }
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
                if (tkn.tknType == TokenType.NUMBER
                    || tkn.tknType == TokenType.MATRIS
                    || tkn.tknType == TokenType.DOCS
                    || tkn.tknType == TokenType.NULL)     // NUMBER | MATRIX | INFORMATION | NULL
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
                        else if ((tkn.assoc == OperatorAssociativity.LEFT
                                    && tkn.priority == o2.priority)
                                 || (tkn.priority < o2.priority))
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
                if ((operatorStack.Peek().tknType == TokenType.LEFTBRACE)
                    || (operatorStack.Peek().tknType == TokenType.RIGHTBRACE))
                {
                    throw new Exception(CompilerMessage.PARANTHESIS_FORMAT_ERROR);
                }
                outputQueue.Enqueue(operatorStack.Pop());
            }
            return new List<Token>(outputQueue.ToArray());
        }

        public List<Token> Tokenize(string exp,
                                    CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            string[] explist = TokenizeSplit(exp);
            List<Token> tkns = new List<Token>();

            foreach (string e in explist)
            {
                Token tkn = String2Token(e, mode);
                // Decide for unary or binary
                if (e == "-" || e == "+")
                {
                    // Started with - , unary ; TO-DO: Doesn't work well with exponential base
                    if (tkns.Count == 0)
                    { tkn.SetValues("u" + e, OperatorAssociativity.RIGHT, 200, 1); }
                    // Previous was a left bracet or an operator
                    else if (tkns[^1].tknType == TokenType.LEFTBRACE
                             || tkns[^1].tknType == TokenType.OPERATOR
                             || tkns[^1].tknType == TokenType.ARGSEPERATOR)
                    { tkn.SetValues("u" + e, OperatorAssociativity.RIGHT, 200, 1); }
                }
                tkns.Add(tkn);
            }
            return tkns;

        }

        public CommandInfo TryParseBuiltFunc(string name,
                                             List<string> filter = null,
                                             bool blacklist = false)
        {
            if (filter == null && !blacklist)
            {
                blacklist = true;
            }

            foreach (CommandInfo cinfo in from CommandLabel lbl in GetCommandLabelList(filter, blacklist)
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
                                            List<Command> cmdHistory,
                                            CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            if (cmd == null)
            {
                return CommandState.ERROR;
            }
            switch (cmd.STATE)
            {
                case CommandState.IDLE: // Komut ilk defa işlenmekte
                    {
                        cmd.STATE = CommandState.UNAVAILABLE;
                        List<Token> tkns = cmd.GetTokens();

                        if (tkns.Count == 1) // Single token
                        {
                            CommandState state = SingleTermCommand(cmd, tkns[0], matdict, mode) ?? CommandState.UNAVAILABLE;
                            if (state != CommandState.UNAVAILABLE)
                            {   // Check if token needs further evaluation, if not return
                                return state;
                            }
                        }
                        // More than a single token or single token needs evaluation
                        EvaluateIdleCommand(cmd, tkns, matdict, mode);
                        break;
                    }
                case CommandState.UNAVAILABLE: // Komut işlenmekte veya hatalı
                    {
                        cmd.SetStateMessage(CommandStateMessage.CMD_UNAVAILABLE(cmd.OriginalCommand));
                        break;
                    }
                default: // Komut zaten işlenmiş
                    {
                        cmd.SetStateMessage(CommandStateMessage.CMD_COMPILED(cmd.STATE, cmd.GetStateMessage()));
                        break;
                    }
            }
            // Clean up command history
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
            return calldate == null
                || (DateTime.Now - calldate).TotalSeconds >= (int)CompilerLimits.forCmdSendRateInSeconds;
        }

        public void CleanUp()
        {
            CleanUp_state = true;
        }

        public string Help(dynamic term = null)
        {
            return string.Empty;
        }

        #endregion

    }
}
