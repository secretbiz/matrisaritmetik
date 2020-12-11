using System;
using System.Collections.Generic;
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
        private bool CleanUp_state = false;
        #endregion

        #region Private Methods

        /// <summary>
        /// Set basic fields of a <see cref="Token"/> <paramref name="tkn"/> by checking its <paramref name="symbol"/>
        /// </summary>
        /// <param name="tkn"><see cref="Token"/> to use</param>
        /// <param name="symbol">Symbol/operator to use</param>
        /// <returns>Given token with values set</returns>
        private Token SetTokenFieldsViaSymbol(Token tkn, string symbol)
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
                if (exp[0] == '!')          // A function
                {
                    exp = exp.Replace("!", "");
                    if (exp == "null")
                    {
                        tkn.val = new None();
                        tkn.tknType = TokenType.NULL;
                    }
                    else if (TryParseBuiltFunc(exp, out CommandInfo cmdinfo))
                    {
                        tkn.paramCount = cmdinfo.param_names.Length;
                        tkn.service = cmdinfo.service;
                        tkn.tknType = TokenType.FUNCTION;       // VALID FUNCTION
                        tkn.name = exp;
                        tkn.returns = cmdinfo.returns;
                        tkn.paramTypes = new List<string>(cmdinfo.param_types);
                        tkn.priority = 100;
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.NOT_A_(exp, "fonksiyon"));
                    }
                }
                else if (Validations.ValidMatrixName(exp))          // MATRIX
                {
                    tkn.tknType = TokenType.MATRIS;
                    tkn.name = exp;
                }
                else if (exp[0] == '?')                 // Information about following expression
                {
                    exp = exp.Replace("?", "").Trim();
                    tkn.tknType = TokenType.DOCS;
                    if (TryParseBuiltFunc(exp, out CommandInfo cmdinfo))
                    {   // VALID FUNCTION
                        tkn.name = exp;
                        tkn.info = cmdinfo.Info();
                    }
                    else if (Validations.ValidMatrixName(exp))
                    {   // MATRIX
                        tkn.name = exp;
                        tkn.info = "matrix";
                    }
                    else if (exp == "") // Spammed bunch of question marks
                    {
                        tkn.info = "info";
                    }
                    else    // NOTHING FOUND
                    {
                        tkn.val = exp;
                        tkn.info = "null";
                    }
                }
                else
                {
                    tkn.val = new None();
                    tkn.tknType = TokenType.NULL;       // SHOULDN'T ENT UP HERE
                }
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
        private string[] TokenizeSplit(string exp)
        {
            exp = exp.
                Replace("+", " + ").
                Replace("+ =", "+=").
                Replace("-", " - ").
                Replace("- =", "-=").
                Replace("*", " * ").
                Replace("* =", "*=").
                Replace(". *", ".*").
                Replace("/", " / ").
                Replace("/ =", "/=").
                Replace(". /", "./").
                Replace("(", " ( ").
                Replace(")", " ) ").
                Replace(",", " , ").
                Replace("%", " % ").
                Replace("% =", "%=").
                Replace("^", " ^ ").
                Replace("^ =", "^=").
                Replace(". ^", ".^").
                Replace(".*", " .* ").
                Replace(".^", " .^ ").
                Replace("./", " ./ ").
                Replace("=", " = ").
                Replace(":", " : ").
                Trim();

            if (exp.Contains("=")) // Matris_name = some_expression
            {
                string[] expsplits = exp.Split("=", StringSplitOptions.RemoveEmptyEntries);
                if (expsplits.Length != 2)
                {
                    throw new Exception(CompilerMessage.EQ_MULTIPLE_USE);
                }
                else
                {
                    expsplits[0] = expsplits[0].Trim();
                    expsplits[1] = expsplits[1].Trim();
                    if (expsplits[0] == "" || expsplits[1] == "")
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
        private int GetParamIndex(string name, ParameterInfo[] paramarr)
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
        private bool CheckMatrixAndUpdateVal(Token tkn, Dictionary<string, MatrisBase<dynamic>> matDict)
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
        private Token InvokeMethods(Token op, List<Token> operands, object serviceObject, MethodInfo method, object[] arguments)
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
                    if (op.service != "")
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

                            throw err;
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
        private object ApplyArgumentValue(Token op,
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
        private object[] CheckAndParseArgumentsAndHints(Token op,
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
        private object ParseTokenValAsParameterType(Token op,
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
                                    arguments[argumentIndex] = Convert.ToInt32((string)((MatrisBase<dynamic>)arguments[argumentIndex])[0, 0].ToString());
                                }
                            }
                            else
                            {
                                arguments[argumentIndex] = Convert.ToInt32(arguments[argumentIndex]);
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
                                    arguments[argumentIndex] = Convert.ToSingle((string)((MatrisBase<dynamic>)arguments[argumentIndex])[0, 0].ToString());
                                }
                            }
                            else
                            {
                                arguments[argumentIndex] = Convert.ToSingle(arguments[argumentIndex]);
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
        private object[] ParseDefaultValues(int parameterCount,
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
                                arguments[k] = Convert.ToInt32(parameterInfo[k].DefaultValue);
                                break;
                            }
                        case "System.Single":
                            {
                                arguments[k] = Convert.ToSingle(parameterInfo[k].DefaultValue);
                                break;
                            }
                        case "System.Double":
                            {
                                arguments[k] = Convert.ToDouble(parameterInfo[k].DefaultValue);
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
        /// Evaluate expressino where <paramref name="op"/> is <see cref="TokenType.OPERATOR"/> type
        /// </summary>
        /// <param name="op">Operator token, type <see cref="TokenType.OPERATOR"/></param>
        /// <param name="operands">List of operands</param>
        /// <param name="matDict">Matrix dictionary to refer to if necessary</param>
        /// <returns>Operands list after evaluation</returns>
        private List<Token> EvalWithSymbolOperator(Token op,
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
                            throw new Exception(CompilerMessage.EXPO_NOT_NUMBER);
                        }

                        if (CheckMatrixAndUpdateVal(operands[1], matDict))  // base matrix
                        {
                            operands[0].val = ((MatrisBase<object>)operands[1].val).Power((dynamic)operands[0].val);
                            operands[0].tknType = TokenType.MATRIS;
                        }
                        else // base is number
                        {
                            operands[0].val = MatrisBase<object>.PowerMethod(double.Parse(operands[1].val.ToString()), double.Parse(operands[0].val.ToString())); // Use method inside matrisbase for now to calculate power
                            operands[0].tknType = TokenType.NUMBER;
                        }

                        break;
                    }
                case ".^":      // A.^3 == A@A@A  , A kare matris
                    {
                        if (operands[0].tknType != TokenType.NUMBER)
                        {
                            throw new Exception(CompilerMessage.EXPO_NOT_NUMBER);
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
                            MatrisBase<dynamic> mat = res.Copy();

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
                                if (operands[1].name == "")
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

                        if (op.service != "" && op.service != "FrontService")
                        {
                            Type serviceType = op.service switch
                            {
                                "MatrisArithmeticService" => typeof(MatrisArithmeticService<object>),
                                "SpecialMatricesService" => typeof(SpecialMatricesService),
                                _ => throw new Exception(CompilerMessage.UNKNOWN_SERVICE(op.service))
                            };

                            // Construct service
                            serviceObject = serviceType.GetConstructor(Type.EmptyTypes)
                                                       .Invoke(new object[] { });

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
        private Command SetDocsCommand(Command cmd,
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
                            cmd.STATE_MESSAGE = CommandStateMessage.DOCS_MAT_FOUND(tkn.name);
                            cmd.Output = matdict[tkn.name].Details(tkn.name);
                        }
                        else
                        {
                            cmd.STATE = CommandState.ERROR;
                            cmd.STATE_MESSAGE = CommandStateMessage.DOCS_NOT_MAT_FUNC(tkn.name);
                        }
                        break;
                    }
                case "info":    // info about how to use the compiler
                    {
                        cmd.STATE = CommandState.SUCCESS;
                        cmd.STATE_MESSAGE = CommandStateMessage.SUCCESS_COMPILER_DOCS;
                        cmd.Output = CompilerMessage.COMPILER_HELP;
                        break;
                    }
                case "null":    // nothing found
                    {
                        cmd.STATE = CommandState.ERROR;
                        cmd.STATE_MESSAGE = tkn.name.Trim() != ""
                            ? CommandStateMessage.DOCS_NOT_MAT_FUNC(tkn.name)
                            : tkn.symbol.Trim() != ""
                                ? CommandStateMessage.DOCS_NONE_FOUND(tkn.symbol)
                                : (string)CommandStateMessage.DOCS_NONE_FOUND(tkn.val);

                        break;
                    }
                default:        // given name was a function, and also possibly a matrix
                    {
                        if (matdict.ContainsKey(tkn.name))
                        {
                            cmd.STATE = CommandState.SUCCESS;
                            cmd.STATE_MESSAGE = CommandStateMessage.DOCS_MAT_FUNC_FOUND(tkn.name);
                            cmd.Output = matdict[tkn.name].Details(tkn.name) +
                                "\nKomut: " + tkn.name + "\n" +
                                tkn.info;
                        }
                        else
                        {
                            cmd.STATE = CommandState.SUCCESS;
                            cmd.STATE_MESSAGE = CommandStateMessage.DOCS_FUNC_FOUND(tkn.name);
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
            if (matdict.Count >= (int)MatrisLimits.forMatrisCount)
            {
                throw new Exception(CompilerMessage.MAT_LIMIT);
            }

            if (Validations.ValidMatrixName(name)) // Check again just in case
            {
                matdict.Add(name, matris);
            }
        }

        public void DeleteFromMatrisDict(string name,
                                         Dictionary<string, MatrisBase<dynamic>> matdict)
        {
            if (matdict.ContainsKey(name))
            {
                matdict[name].Values = null;
                matdict.Remove(name);
            }
        }

        public Command CreateCommand(string cmd)
        {
            return new Command(cmd);
        }

        public List<CommandLabel> builtInCommands;

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
                return builtInCommands;
            }

            List<CommandLabel> filtered = new List<CommandLabel>();
            foreach (CommandLabel lbl in builtInCommands)
            {
                if (filter.Contains(lbl.Label))
                {
                    filtered.Add(lbl);
                }
            }
            return filtered;
        }

        public void AddToCommandLabelList(string label, CommandInfo[] commandInfos)
        {
            int labelIndex = builtInCommands.FindIndex(0, cmdlbl => cmdlbl.Label == label);
            if (labelIndex == -1)
            {
                builtInCommands.Append(new CommandLabel() { Functions = commandInfos, Label = label });
            }
            else
            {   // TO-DO : Check aliasing
                builtInCommands[labelIndex].Functions.Concat(commandInfos);
            }
        }

        public void ClearCommandLabel(string label)
        {
            int labelIndex = builtInCommands.FindIndex(0, cmdlbl => cmdlbl.Label == label);
            if (labelIndex != -1)
            {
                builtInCommands[labelIndex] = new CommandLabel() { Label = label };
            }
        }

        public List<Token> ShuntingYardAlg(List<Token> tkns)
        {
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

        public bool TryParseBuiltFunc(string name, out CommandInfo cmdinfo)
        {
            if (builtInCommands == null)
            {
                ReadCommandInformation();
            }

            foreach (CommandInfo _cmdinfo in from CommandLabel _lbl in builtInCommands
                                             from CommandInfo _cmdinfo in _lbl.Functions
                                             where _cmdinfo.function == name
                                             select _cmdinfo)
            {
                cmdinfo = _cmdinfo;
                return true;
            }

            cmdinfo = null;
            return false;
        }

        public CommandState EvaluateCommand(Command cmd, Dictionary<string, MatrisBase<dynamic>> matdict, List<Command> cmdHistory)
        {
            List<Token> tkns = cmd.Tokens;

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
                                if (matdict.ContainsKey(tkns[0].name))
                                {
                                    cmd.STATE = CommandState.SUCCESS;
                                    cmd.STATE_MESSAGE = CommandStateMessage.SUCCESS_RET_MAT;
                                    cmd.Output = matdict[tkns[0].name].ToString();
                                }
                                else
                                {
                                    cmd.STATE = CommandState.SUCCESS;
                                    cmd.STATE_MESSAGE = CommandStateMessage.SUCCESS_RET_NULL;
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
                                            cmd.STATE_MESSAGE = CommandStateMessage.SUCCESS_RET_NUM;
                                            cmd.Output = tkns[0].val.ToString();
                                            return CommandState.SUCCESS;
                                        }
                                    case TokenType.NULL:
                                        {
                                            cmd.STATE = CommandState.SUCCESS;
                                            cmd.STATE_MESSAGE = CommandStateMessage.SUCCESS_RET_NULL;
                                            cmd.Output = "null";
                                            return CommandState.SUCCESS;
                                        }
                                    case TokenType.DOCS:
                                        {
                                            cmd = SetDocsCommand(cmd, tkns[0], matdict);
                                            return cmd.STATE;
                                        }
                                    case TokenType.OPERATOR:
                                        {
                                            tkns[0].symbol = (tkns[0].symbol == "u+" || tkns[0].symbol == "u-") ? tkns[0].symbol[1].ToString() : tkns[0].symbol;
                                            cmd.STATE = CommandState.ERROR;
                                            cmd.STATE_MESSAGE = CompilerMessage.OP_CANT_BE_ALONE(tkns[0].symbol);
                                            return CommandState.ERROR;
                                        }
                                    case TokenType.FUNCTION:
                                        {
                                            if (tkns[0].paramCount == 0)
                                            {
                                                break;
                                            }

                                            cmd.STATE = CommandState.WARNING;
                                            cmd.STATE_MESSAGE = CompilerMessage.FUNC_REQUIRES_ARGS(tkns[0].name, tkns[0].paramCount);
                                            return CommandState.WARNING;
                                        }
                                    default:
                                        {
                                            cmd.STATE = CommandState.ERROR;
                                            cmd.STATE_MESSAGE = tkns[0].tknType.ToString() + " tipi terim tek başına kullanılamaz ";
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
                                    cmd.STATE_MESSAGE = CommandStateMessage.SUCCESS_RET_NULL;
                                }
                            }
                            else if (operandStack.Count == 0)
                            {
                                cmd.STATE = CommandState.ERROR;

                                cmd.STATE_MESSAGE = tkns.Count == 0 ? CompilerMessage.OP_INVALID(cmd.TermsToEvaluate[0]) : CompilerMessage.ARG_COUNT_ERROR;
                            }
                            else
                            {
                                cmd.STATE = CommandState.ERROR;
                                cmd.STATE_MESSAGE = CompilerMessage.CMD_FORMAT_ERROR;
                            }
                        }
                        catch (Exception err)
                        {
                            cmd.STATE = CommandState.ERROR;
                            cmd.STATE_MESSAGE = err.Message;
                        }

                        break;
                    }
                // Komut işlenmekte veya hatalı
                case CommandState.UNAVAILABLE:
                    {
                        cmd.STATE_MESSAGE = CommandStateMessage.CMD_UNAVAILABLE(cmd.OriginalCommand);
                        break;
                    }
                // Komut zaten işlenmiş
                default:
                    {
                        cmd.STATE_MESSAGE = CommandStateMessage.CMD_COMPILED(cmd.STATE, cmd.STATE_MESSAGE);
                        break;
                    }

            }

            if (CleanUp_state && cmd.STATE == CommandState.SUCCESS)
            {
                cmdHistory.Clear();
                CleanUp_state = false;
                cmd.STATE_MESSAGE = CommandStateMessage.SUCCESS_CLEANUP;
                if (!cmd.NameSettings.ContainsKey("display"))
                {
                    cmd.NameSettings.Add("display", "none");
                }
            }

            return cmd.STATE;
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
    }
}
