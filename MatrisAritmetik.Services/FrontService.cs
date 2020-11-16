using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;
using Newtonsoft.Json;

namespace MatrisAritmetik.Services
{
    public class FrontService : IFrontService
    {
        public bool state = false;
        public void AddToMatrisDict(string name, MatrisBase<dynamic> matris, Dictionary<string, MatrisBase<dynamic>> matdict)
        {
            if (matdict.Count >= (int)MatrisLimits.forMatrisCount)
            {
                return;
            }

            if (Validations.ValidMatrixName(name)) // Check again just in case
            {
                matdict.Add(name, matris);
            }
        }

        public void DeleteFromMatrisDict(string name, Dictionary<string, MatrisBase<dynamic>> matdict)
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

        public bool TknTryParseBuiltFunc(string name, out CommandInfo cmdinfo)
        {
            if (builtInCommands == null)
            {
                ReadCommandInformation();
            }

            foreach (CommandLabel _lbl in builtInCommands)
            {
                foreach (CommandInfo _cmdinfo in _lbl.Functions)
                {
                    if (_cmdinfo.function == name)
                    {
                        cmdinfo = _cmdinfo;
                        return true;
                    }
                }
            }
            cmdinfo = null;
            return false;
        }

        /// <summary>
        ///     Create a token from a string expression
        /// </summary>
        /// Current order of operations:
        ///     OPERATOR    PRIORITY(Higher first)
        ///     ----------------------------------
        ///         u+              200
        ///         u-              200
        ///        FUNC             100
        ///         (               10
        ///         )               10
        ///         ./              6
        ///         .*              5
        ///         .^              5
        ///         ^               5
        ///         *               4
        ///         /               4
        ///         %               4
        ///         +               3
        ///         -               3
        ///         :               2
        ///         ,               1
        ///         =               0
        ///         
        ///
        /// <param name="exp"> String expression to tokenize </param>
        /// <returns></returns>
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

            else if (exp.Length > 1 && exp != ".^" && exp != ".*" && exp != "./")
            {
                if (exp[0] == '!')          // A function
                {
                    exp = exp.Replace("!", "");
                    if (TknTryParseBuiltFunc(exp, out CommandInfo cmdinfo))
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
                    if (TknTryParseBuiltFunc(exp, out CommandInfo cmdinfo))
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
                switch (exp)
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
            }
            return tkn;
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
                if (tkn.tknType == TokenType.NUMBER || tkn.tknType == TokenType.MATRIS || tkn.tknType == TokenType.DOCS)        // NUMBER | MATRIX | INFORMATION
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
                string[] expsplits = exp.Split("=",StringSplitOptions.RemoveEmptyEntries);
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
                    switch(expsplits[0][^1].ToString())
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

                    exp = expsplits[0] + " = " + newPart + "( " + expsplits[1] + " )";   // Matris_name = (some_expression)
                }
            }
            while (exp.Contains("  "))
            {
                exp = exp.Replace("  ", " ");
            }
            return exp.Split(" ");
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
            List<CommandLabel> cmdLabelList = GetCommandLabelList();
            if (cmdLabelList == null)
            {
                ReadCommandInformation();
                cmdLabelList = GetCommandLabelList();
            }

            foreach (CommandLabel lbl in cmdLabelList)
            {
                foreach (CommandInfo _cmdinfo in lbl.Functions)
                {
                    if (_cmdinfo.function == name)
                    {
                        cmdinfo = _cmdinfo;
                        return true;
                    }
                }
            }
            cmdinfo = null;
            return false;
        }

        private bool CheckMatrixAndUpdateVal(Token tkn, Dictionary<string, MatrisBase<dynamic>> matDict)
        {
            if (tkn.tknType == TokenType.MATRIS)
            {
                if (matDict.ContainsKey(tkn.name))
                {
                    tkn.val = matDict[tkn.name];
                }
                else if (!(tkn.val is MatrisBase<object>))
                {
                    throw new Exception(CompilerMessage.NOT_SAVED_MATRIX(tkn.name));
                }

                return true;
            }
            return false;
        }

        private Token EvalOperator(Token op, List<Token> operands, Dictionary<string, MatrisBase<dynamic>> matDict)
        {
            if (op.tknType == TokenType.OPERATOR)    // OPERATORS
            {
                switch (op.symbol)
                {
                    case "+":
                        {
                            CheckMatrixAndUpdateVal(operands[0], matDict);
                            CheckMatrixAndUpdateVal(operands[1], matDict);

                            operands[0].val = operands[1].val + operands[0].val;
                            operands[0].tknType = (operands[1].tknType == TokenType.MATRIS ? TokenType.MATRIS : operands[0].tknType);

                            break;
                        }
                    case "-":
                        {
                            CheckMatrixAndUpdateVal(operands[0], matDict);
                            CheckMatrixAndUpdateVal(operands[1], matDict);

                            operands[0].val = operands[1].val - operands[0].val;
                            operands[0].tknType = (operands[1].tknType == TokenType.MATRIS ? TokenType.MATRIS : operands[0].tknType);

                            break;
                        }
                    case "*":
                        {
                            CheckMatrixAndUpdateVal(operands[0], matDict);
                            CheckMatrixAndUpdateVal(operands[1], matDict);

                            operands[0].val *= operands[1].val;
                            operands[0].tknType = (operands[1].tknType == TokenType.MATRIS ? TokenType.MATRIS : operands[0].tknType);

                            break;
                        }
                    case "/":
                        {
                            CheckMatrixAndUpdateVal(operands[0], matDict);
                            CheckMatrixAndUpdateVal(operands[1], matDict);

                            operands[0].val = operands[1].val / operands[0].val;
                            operands[0].tknType = (operands[1].tknType == TokenType.MATRIS ? TokenType.MATRIS : operands[0].tknType);
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
                                if (CheckMatrixAndUpdateVal(operands[1], matDict))
                                {
                                    operands[0].val = operands[1].val % operands[0].val;
                                }
                                else
                                {
                                    throw new Exception(CompilerMessage.MOD_MAT_THEN_BASE_MAT);
                                }
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
                            else if(operands[0].val == 0)
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
                            if (CheckMatrixAndUpdateVal(operands[0], matDict))
                            {
                                mat1 = operands[0].val;
                            }
                            else
                            {
                                throw new Exception(CompilerMessage.OP_BETWEEN_(".*", "matrisler"));
                            }

                            if (CheckMatrixAndUpdateVal(operands[1], matDict))
                            {
                                mat2 = operands[1].val;
                            }
                            else
                            {
                                throw new Exception(CompilerMessage.OP_BETWEEN_(".*", "matrisler"));
                            }

                            operands[0].val = new MatrisArithmeticService<object>().MatrisMul(mat2, mat1);

                            break;
                        }
                    case "./":
                        {
                            MatrisBase<dynamic> mat1, mat2;
                            if (CheckMatrixAndUpdateVal(operands[0], matDict))
                            {
                                mat1 = operands[0].val;
                            }
                            else
                            {
                                throw new Exception(CompilerMessage.OP_BETWEEN_("./", "matrisler"));
                            }

                            if (CheckMatrixAndUpdateVal(operands[1], matDict))
                            {
                                mat2 = operands[1].val;
                            }
                            else
                            {
                                throw new Exception(CompilerMessage.OP_BETWEEN_("./", "matrisler"));
                            }

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
                            if (operands[0].tknType != TokenType.MATRIS || operands[1].tknType != TokenType.MATRIS)
                            {
                                throw new Exception(CompilerMessage.EQ_FORMAT);
                            }
                            else
                            {
                                if (!(operands[0].val is MatrisBase<object>))
                                {
                                    throw new Exception(CompilerMessage.EQ_FAILED);
                                }

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
                                else
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
                            if(Validations.ValidMatrixName(operands[1].name))   // Use the name function for parameter name validation
                            {
                                operands[0].info = operands[1].name;
                            }
                            else
                            {
                                throw new Exception(CompilerMessage.PARAMETER_HINT_INVALID(operands[1].name));
                            }
                            break;
                        }
                    default:
                        throw new Exception(CompilerMessage.OP_INVALID(op.symbol));
                }
            }
            else                             // FUNCTIONS
            {
                if (op.argCount > op.paramCount)
                {
                    throw new Exception(CompilerMessage.FUNC_PARAMCOUNT_EXCESS(op.name, op.paramCount, op.argCount));
                }

                object[] param_arg = new object[op.paramCount];

                object serviceObject = null;
                MethodInfo method = null;
                ParameterInfo[] paraminfo = null;

                operands.Reverse();

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
                    serviceObject = serviceType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });

                    // Get the method
                    method = serviceType.GetMethod(op.name);
                    paraminfo = method.GetParameters();
                }

                // param:arg dict for checking if parameters are referenced multiple times and storing args
                Dictionary<string, dynamic> param_dict = new Dictionary<string, dynamic>();

                bool hintUsed = false;

                // Check for parameter name hints, fill the param:arg dictionary
                for (int k = 0; k < op.argCount; k++)
                {
                    int pind;
                    if(operands[k].info != null)    // hint was given
                    {
                        hintUsed = true;
                        pind = GetParamIndex(operands[k].info, paraminfo);
                        if (pind == -1)
                        {
                            throw new Exception(CompilerMessage.PARAMETER_NAME_INVALID(operands[k].info));
                        }
                        else
                        {
                            if(param_dict.ContainsKey(paraminfo[pind].Name.ToString()))
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
                    else if(param_dict.ContainsKey(paraminfo[k].Name.ToString()))
                    {
                        throw new Exception(CompilerMessage.MULTIPLE_REFERENCES(paraminfo[k].Name.ToString()));
                    }
                    else
                    {
                        param_dict.Add(paraminfo[k].Name.ToString(), operands[k].val);
                        pind = k;
                    }

                    switch (operands[k].tknType)
                    {
                        case TokenType.NULL: param_arg[pind] = null; break;

                        case TokenType.NUMBER: param_arg[pind] = operands[k].val; break;

                        case TokenType.MATRIS:
                            {
                                if (matDict.ContainsKey(operands[k].name))
                                {
                                    param_arg[pind] = matDict[operands[k].name];
                                }
                                else if ((operands[k].val is MatrisBase<object>))
                                {
                                    param_arg[pind] = operands[k].val;
                                }
                                else
                                {
                                    throw new Exception(CompilerMessage.UNKNOWN_VARIABLE(operands[k].name));
                                }

                                break;
                            }

                        default:
                            {
                                throw new Exception(CompilerMessage.UNKNOWN_ARGUMENT_TYPE(op.name, k + 1));
                            }
                    }

                    if (param_arg[pind] != null) // Parse given value
                    {
                        try
                        {
                            param_arg[pind] = (op.paramTypes[pind]) switch
                            {
                                "int" => Convert.ToInt32(param_arg[pind]),
                                "Matris" => ((MatrisBase<dynamic>)param_arg[pind]),
                                "float" => Convert.ToSingle(param_arg[pind]),
                                _ => throw new Exception(CompilerMessage.UNKNOWN_PARAMETER_TYPE(op.paramTypes[pind])),
                            };
                        }
                        catch (Exception)
                        {
                            throw new Exception(CompilerMessage.ARG_PARSE_ERROR(param_arg[pind].ToString(), op.paramTypes[pind]));
                        }
                    }
                }

                // Replace nulls with default values
                for (int k = 0; k < op.paramCount; k++)
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

                if (op.service == "FrontService")
                {
                    switch (op.name)
                    {
                        case "CleanUp":
                            {
                                CleanUp();
                                operands.Add(new Token() { val = null, tknType = TokenType.VOID });
                                break;
                            }
                        default: throw new Exception(CompilerMessage.UNKNOWN_FRONTSERVICE_FUNC(op.name));
                    }
                }
                else if (op.service != "")
                {
                    try
                    {
                        // Invoke the method
                        // No parameters
                        if (param_arg.Length == 0)
                        {
                            method.Invoke(serviceObject, null);
                            operands.Add(new Token() { val = null, tknType = TokenType.VOID });
                        }
                        else
                        {
                            operands[0].val = (dynamic)method.Invoke(serviceObject, param_arg);
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
            }
            operands[0].name = "";
            return operands[0];
        }

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
                                    case TokenType.DOCS:
                                        {
                                            switch (tkns[0].info)
                                            {
                                                case "matrix":
                                                    {
                                                        if (matdict.ContainsKey(tkns[0].name))
                                                        {
                                                            cmd.STATE = CommandState.SUCCESS;
                                                            cmd.STATE_MESSAGE = CommandStateMessage.DOCS_MAT_FOUND(tkns[0].name);
                                                            cmd.Output = matdict[tkns[0].name].Details(tkns[0].name);
                                                        }
                                                        else
                                                        {
                                                            cmd.STATE = CommandState.ERROR;
                                                            cmd.STATE_MESSAGE = CommandStateMessage.DOCS_NOT_MAT_FUNC(tkns[0].name);
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
                                                        if (tkns[0].name.Trim() != "")
                                                        {
                                                            cmd.STATE_MESSAGE = CommandStateMessage.DOCS_NOT_MAT_FUNC(tkns[0].name);
                                                        }
                                                        else if (tkns[0].symbol.Trim() != "")
                                                        {
                                                            cmd.STATE_MESSAGE = CommandStateMessage.DOCS_NONE_FOUND(tkns[0].symbol);
                                                        }
                                                        else
                                                        {
                                                            cmd.STATE_MESSAGE = CommandStateMessage.DOCS_NONE_FOUND(tkns[0].val);
                                                        }

                                                        break;
                                                    }
                                                default:        // given name was a function, and also possibly a matrix
                                                    {
                                                        if (matdict.ContainsKey(tkns[0].name))
                                                        {
                                                            cmd.STATE = CommandState.SUCCESS;
                                                            cmd.STATE_MESSAGE = CommandStateMessage.DOCS_MAT_FUNC_FOUND(tkns[0].name);
                                                            cmd.Output = matdict[tkns[0].name].Details(tkns[0].name) +
                                                                "\nKomut: " + tkns[0].name + "\n" +
                                                                tkns[0].info;
                                                        }
                                                        else
                                                        {
                                                            cmd.STATE = CommandState.SUCCESS;
                                                            cmd.STATE_MESSAGE = CommandStateMessage.DOCS_FUNC_FOUND(tkns[0].name);
                                                            cmd.Output = tkns[0].info;
                                                        }
                                                        break;
                                                    }
                                            }
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
                                if (tkn.tknType == TokenType.NUMBER || tkn.tknType == TokenType.MATRIS)
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

                                if (tkns.Count == 0)
                                {
                                    cmd.STATE_MESSAGE = CompilerMessage.OP_INVALID(cmd.TermsToEvaluate[0]);
                                }
                                else
                                {
                                    cmd.STATE_MESSAGE = CompilerMessage.ARG_COUNT_ERROR;
                                }
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
            if (state && cmd.STATE == CommandState.SUCCESS)
            {
                cmdHistory.Clear();
                state = false;
                cmd.STATE_MESSAGE = CommandStateMessage.SUCCESS_CLEANUP;
                if(!cmd.NameSettings.ContainsKey("display"))
                {
                    cmd.NameSettings.Add("display", "none");
                }
            }
            return cmd.STATE;
        }

        public void CleanUp()
        {
            state = true;
        }
    }
}
