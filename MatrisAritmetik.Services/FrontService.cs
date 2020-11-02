using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;
using Newtonsoft.Json;

namespace MatrisAritmetik.Services
{
    public class FrontService: IFrontService
    {
        public void AddToMatrisDict(string name, MatrisBase<dynamic> matris, Dictionary<string,MatrisBase<dynamic>> matdict)
        {
            if (matdict.Count >= (int)MatrisLimits.forMatrisCount)
                return;

            if (Validations.ValidMatrixName(name))
                matdict.Add(name, matris);
        }

        public void DeleteFromMatrisDict(string name, Dictionary<string, MatrisBase<dynamic>> matdict)
        {   
            if (matdict.ContainsKey(name))
            {
                matdict[name].values = null;
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
                return builtInCommands;

            List<CommandLabel> filtered = new List<CommandLabel>();
            foreach(CommandLabel lbl in builtInCommands)
            {
                if (filter.Contains(lbl.Label))
                    filtered.Add(lbl);
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

        private bool TknTryParseBuiltFunc(string name, out CommandInfo cmdinfo)
        {
            if (builtInCommands == null)
                ReadCommandInformation();

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

        private Token String2Token(string exp)
        {
            Token tkn = new Token();

            if (double.TryParse(exp, out double val))
            {
                tkn.tknType = TokenType.NUMBER;                     // NUMBER
                tkn.val = val;
            }

            else if (exp == "(")
                tkn.tknType = TokenType.LEFTBRACE;                  // LEFT BRACE

            else if (exp == ")")
                tkn.tknType = TokenType.RIGHTBRACE;                 // RIGHT BRACE

            else if (exp.Length > 1 && exp != ".^" && exp != ".*")
            {
                if (exp[0] == '!')
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
                    }
                    else
                    {
                        tkn.tknType = TokenType.NULL;       // BAD FUNCTION NAME
                    }
                }
                else if (Validations.ValidMatrixName(exp))
                {
                    tkn.tknType = TokenType.MATRIS;                // MATRIX
                    tkn.name = exp;
                }
                else
                    tkn.tknType = TokenType.NULL;       // SHOULDN'T ENT UP HERE
            }
            else if (Validations.ValidMatrixName(exp))
            {
                tkn.tknType = TokenType.MATRIS;                   // MATRIX
                tkn.name = exp;
            }

            else if (exp == ",")
                tkn.tknType = TokenType.ARGSEPERATOR;               // ARGUMENT SEPERATOR

            else
            {                                                       // OPERATOR
                switch (exp)
                {
                    case "=":
                        {
                            tkn.tknType = TokenType.OPERATOR;
                            tkn.symbol = "=";
                            tkn.priority = 2;
                            tkn.assoc = OperatorAssociativity.RIGHT;
                            tkn.paramCount = 2;
                            break;
                        }
                    case "+":
                        {
                            tkn.tknType = TokenType.OPERATOR;
                            tkn.symbol = "+";
                            tkn.priority = 5;
                            tkn.assoc = OperatorAssociativity.LEFT;
                            tkn.paramCount = 2;
                            break;
                        }
                    case "-":
                        {
                            tkn.tknType = TokenType.OPERATOR;
                            tkn.symbol = "-";
                            tkn.priority = 5;
                            tkn.assoc = OperatorAssociativity.LEFT;
                            tkn.paramCount = 2;
                            break;
                        }
                    case "*":
                        {
                            tkn.tknType = TokenType.OPERATOR;
                            tkn.symbol = "*";
                            tkn.priority = 10;
                            tkn.assoc = OperatorAssociativity.LEFT;
                            tkn.paramCount = 2;
                            break;
                        }
                    case "/":
                        {
                            tkn.tknType = TokenType.OPERATOR;
                            tkn.symbol = "/";
                            tkn.priority = 10;
                            tkn.assoc = OperatorAssociativity.LEFT;
                            tkn.paramCount = 2;
                            break;
                        }
                    case "^":
                        {
                            tkn.tknType = TokenType.OPERATOR;
                            tkn.symbol = "^";
                            tkn.priority = 15;
                            tkn.assoc = OperatorAssociativity.LEFT;
                            tkn.paramCount = 2;
                            break;
                        }
                    case ".^":
                        {
                            tkn.tknType = TokenType.OPERATOR;
                            tkn.symbol = ".^";
                            tkn.priority = 15;
                            tkn.assoc = OperatorAssociativity.LEFT;
                            tkn.paramCount = 2;
                            break;
                        }
                    case ".*":
                        {
                            tkn.tknType = TokenType.OPERATOR;
                            tkn.symbol = ".*";
                            tkn.priority = 10;
                            tkn.assoc = OperatorAssociativity.LEFT;
                            tkn.paramCount = 2;
                            break;
                        }
                    case "./":
                        {
                            tkn.tknType = TokenType.OPERATOR;
                            tkn.symbol = "./";
                            tkn.priority = 20;
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
            int ind = 0;
            while (ind < tkns.Count)
            {
                Token tkn = tkns[ind];
                if (tkn.tknType == TokenType.NUMBER || tkn.tknType == TokenType.MATRIS)        // NUMBER OR MATRIX
                    outputQueue.Enqueue(tkn);

                else if (tkn.tknType == TokenType.FUNCTION)   // FUNCTION
                    operatorStack.Push(tkn);

                else if (tkn.tknType == TokenType.ARGSEPERATOR)     // ARGUMENT SEPERATOR
                {
                    while (operatorStack.Peek().tknType != TokenType.LEFTBRACE)
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                        if (operatorStack.Count == 0)
                            throw new Exception("Parantez formatı hatalı.");
                    }
                }

                else if (tkn.tknType == TokenType.OPERATOR)  // OPERATOR
                {
                    while (operatorStack.Count != 0)
                    {
                        Token o2 = operatorStack.Peek();
                        if (o2.tknType != TokenType.OPERATOR)
                            break;
                        else if ((tkn.assoc == OperatorAssociativity.LEFT && tkn.priority == o2.priority) || (tkn.priority < o2.priority))
                            outputQueue.Enqueue(operatorStack.Pop());
                        else
                            break;
                    }
                    operatorStack.Push(tkn);
                }

                else if (tkn.tknType == TokenType.LEFTBRACE)    // LEFT BRACE
                    operatorStack.Push(tkn);

                else if (tkn.tknType == TokenType.RIGHTBRACE)   // RIGHT BRACE
                {
                    while (operatorStack.Peek().tknType != TokenType.LEFTBRACE)
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                        if (operatorStack.Count == 0)
                            throw new Exception("Parantez formatı hatalı.");
                    }
                    operatorStack.Pop();

                    if (operatorStack.Peek().tknType == TokenType.FUNCTION)
                        outputQueue.Enqueue(operatorStack.Pop());


                }
                ind++;
            }

            while (operatorStack.Count != 0)
            {
                if ((operatorStack.Peek().tknType == TokenType.LEFTBRACE) || (operatorStack.Peek().tknType == TokenType.RIGHTBRACE))
                    throw new Exception("Parantez formatı hatalı.");

                outputQueue.Enqueue(operatorStack.Pop());
            }

            return new List<Token>(outputQueue.ToArray());
        }

        private string[] TokenizeSplit(string exp)
        {
            exp = exp.Replace("+", " + ").
                Replace("-", " - ").
                Replace("*", " * ").
                Replace(". *", ".*").
                Replace("/", " / ").
                Replace(". /", "./").
                Replace("(", " ( ").
                Replace(")", " ) ").
                Replace(",", " , ").
                Replace("^", " ^ ").
                Replace(". ^", ".^").
                Replace(".*", " .* ").
                Replace(".^", " .^ ").
                Replace("=", " = ").
                Trim();

            if (exp.Contains(" = ")) // Matris_name = some_expression
            {
                string[] expsplits = exp.Split("=");
                if (expsplits.Length != 2)
                    throw new Exception("Atama operatörü birden fazla kez kullanılamaz");
                else
                {
                    if (!Validations.ValidMatrixName(expsplits[0].Trim()))
                        throw new Exception("'" + expsplits[0].Trim() + "' uygun bir matris ismi değil.");
                    else
                        exp = expsplits[0].Trim() + " = ( " + expsplits[1].Trim() + " )";   // Matris_name = (some_expression)
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
                    { tkn.SetValues("u" + e, OperatorAssociativity.RIGHT, 20, 1); }
                    // Previous was a left bracet or an operator
                    else if (tkns[tkns.Count - 1].tknType == TokenType.LEFTBRACE || tkns[tkns.Count - 1].tknType == TokenType.OPERATOR)
                    { tkn.SetValues("u" + e, OperatorAssociativity.RIGHT, 20, 1); }
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

        private Token EvalOperator(Token op, List<Token> operands, Dictionary<string,MatrisBase<dynamic>> matDict)
        {
            if(op.tknType == TokenType.OPERATOR)    // OPERATORS
            {
                switch (op.symbol)
                {
                    case "+":
                        {
                            if (operands[0].tknType == TokenType.MATRIS)
                            {
                                if (matDict.ContainsKey(operands[0].name))
                                    operands[0].val = matDict[operands[0].name];
                                else if (!(operands[0].val is MatrisBase<object>))
                                    throw new Exception("'" + operands[0].name + "' adlı bir matris bulunamadı");
                                
                            }

                            if (operands[1].tknType == TokenType.MATRIS)
                            {
                                if (matDict.ContainsKey(operands[1].name))
                                    operands[1].val = matDict[operands[1].name];
                                else if (!(operands[1].val is MatrisBase<object>))
                                    throw new Exception("'" + operands[1].name + "' adlı bir matris bulunamadı");
                                
                            }
                            
                            operands[0].val = operands[1].val + operands[0].val;
                            operands[0].tknType = (operands[1].tknType == TokenType.MATRIS ? TokenType.MATRIS : operands[0].tknType); 

                            break;
                        }
                    case "-":
                        {
                            if (operands[0].tknType == TokenType.MATRIS)
                            {
                                if (matDict.ContainsKey(operands[0].name))
                                    operands[0].val = matDict[operands[0].name];
                                else if (!(operands[0].val is MatrisBase<object>))
                                    throw new Exception("'" + operands[0].name + "' adlı bir matris bulunamadı");
                            }

                            if (operands[1].tknType == TokenType.MATRIS)
                            {
                                if (matDict.ContainsKey(operands[1].name))
                                    operands[1].val = matDict[operands[1].name];
                                else if (!(operands[1].val is MatrisBase<object>))
                                    throw new Exception("'" + operands[1].name + "' adlı bir matris bulunamadı");
                            }

                            operands[0].val = operands[1].val - operands[0].val;
                            operands[0].tknType = (operands[1].tknType == TokenType.MATRIS ? TokenType.MATRIS : operands[0].tknType);
                            break;
                        }
                    case "*":
                        {
                            if (operands[0].tknType == TokenType.MATRIS)
                            {
                                if (matDict.ContainsKey(operands[0].name))
                                    operands[0].val = matDict[operands[0].name];
                                else if(!(operands[0].val is MatrisBase<object>))
                                    throw new Exception("'" + operands[0].name + "' adlı bir matris bulunamadı");
                            }

                            if (operands[1].tknType == TokenType.MATRIS)
                            {
                                if (matDict.ContainsKey(operands[1].name))
                                    operands[1].val = matDict[operands[1].name];
                                else if (!(operands[1].val is MatrisBase<object>))
                                    throw new Exception("'" + operands[1].name + "' adlı bir matris bulunamadı");

                                operands[0].tknType = TokenType.MATRIS;
                            }

                            operands[0].val *= operands[1].val;
                            operands[0].tknType = (operands[1].tknType == TokenType.MATRIS ? TokenType.MATRIS : operands[0].tknType);

                            break;
                        }
                    case "/":
                        {
                            if (operands[0].tknType == TokenType.MATRIS)
                            {
                                if(matDict.ContainsKey(operands[0].name))
                                    operands[0].val = matDict[operands[0].name];
                                else if (!(operands[0].val is MatrisBase<object>))
                                    throw new Exception("'" + operands[0].name + "' adlı bir matris bulunamadı");
                            }

                            if (operands[1].tknType == TokenType.MATRIS)
                            {
                                if (matDict.ContainsKey(operands[1].name))
                                    operands[1].val = matDict[operands[1].name];
                                else if (!(operands[1].val is MatrisBase<object>))
                                    throw new Exception("'" + operands[1].name + "' adlı bir matris bulunamadı");

                                operands[0].tknType = TokenType.MATRIS;
                            }

                            operands[0].val = operands[1].val / operands[0].val;
                            operands[0].tknType = (operands[1].tknType == TokenType.MATRIS ? TokenType.MATRIS : operands[0].tknType);
                            break;
                        }
                    case "^":   // A^3 == A'nın elemanlarının 3. kuvvetleri
                        {
                            if (operands[0].tknType != TokenType.NUMBER)
                                throw new Exception("Üssel kısım sayı olmalı");

                            if(operands[1].tknType == TokenType.MATRIS)
                            {
                                if(matDict.ContainsKey(operands[1].name))
                                {
                                   operands[0].val = matDict[operands[1].name].Power((int)operands[0].val);
                                }
                                else if (operands[1].val is MatrisBase<object>) // Inner matrix, not named
                                {
                                    operands[0].val = operands[1].val.Power((int)operands[0].val);
                                }
                                else
                                    throw new Exception("'" + operands[0].name + "' adlı bir matris bulunamadı");

                                operands[0].tknType = TokenType.MATRIS;
                            }
                            else
                            {
                                dynamic val = operands[1].val;
                                for (int i = 0; i < operands[0].val - 1; i++)
                                    val *= operands[1].val;

                                operands[0].val = val;
                                operands[0].tknType = (operands[1].tknType == TokenType.MATRIS ? TokenType.MATRIS : operands[0].tknType);
                            }

                            break;
                        }
                    case ".^":      // A.^3 == A@A@A  , A kare matris
                        {
                            if (operands[0].tknType != TokenType.NUMBER)
                                throw new Exception("Üssel kısım sayı olmalı");

                            if (operands[1].tknType == TokenType.MATRIS)
                            {
                                if (matDict.ContainsKey(operands[1].name))
                                {
                                    if (!matDict[operands[1].name].IsSquare())
                                        throw new Exception("Sadece kare matrisler .^ operatörünü kullanabilir ");

                                    MatrisBase<float> res = new MatrisBase<float>(MatrisBase<dynamic>.FloatListParse(matDict[operands[1].name]));
                                    MatrisBase<float> mat = res.Copy();

                                    IMatrisArithmeticService<float> matservice = new MatrisArithmeticService<float>();

                                    for (int i = 1; i < operands[0].val; i++)
                                        res = matservice.MatrisMul(res, mat);

                                    operands[0].val = res;
                                    operands[0].tknType = TokenType.MATRIS;
                                }
                                else if (operands[1].val is MatrisBase<object>) // Inner mat
                                {
                                    MatrisBase<float> res = new MatrisBase<float>(MatrisBase<dynamic>.FloatListParse(operands[1].val));
                                    MatrisBase<float> mat = res.Copy();

                                    IMatrisArithmeticService<float> matservice = new MatrisArithmeticService<float>();

                                    for (int i = 1; i < operands[0].val; i++)
                                        res = matservice.MatrisMul(res, mat);

                                    operands[0].val = res;
                                    operands[0].tknType = TokenType.MATRIS;
                                }
                                else
                                    throw new Exception("'" + operands[1].name + "' adlı bir matris bulunamadı");
                            }
                            else
                                throw new Exception(" .^ işlemi taban olarak matris gerektirir");

                            break;
                        }
                    case ".*":
                        {
                            MatrisBase<float> mat1, mat2;
                            if (matDict.ContainsKey(operands[0].name))
                            { mat1 = new MatrisBase<float>(MatrisBase<dynamic>.FloatListParse(matDict[operands[0].name])); }
                            else if((operands[0].val is MatrisBase<object>))
                            { mat1 = new MatrisBase<float>(MatrisBase<dynamic>.FloatListParse(operands[0].val)); }
                            else
                                throw new Exception("'" + operands[0].name + "' adlı bir matris bulunamadı");

                            if (matDict.ContainsKey(operands[1].name))
                            { mat2 = new MatrisBase<float>(MatrisBase<dynamic>.FloatListParse(matDict[operands[1].name])); }
                            else if ((operands[1].val is MatrisBase<object>))
                            { mat2 = new MatrisBase<float>(MatrisBase<dynamic>.FloatListParse(operands[1].val)); }
                            else
                                throw new Exception("'" + operands[0].name + "' adlı bir matris bulunamadı");

                            operands[0].val = new MatrisArithmeticService<float>().MatrisMul(mat2, mat1);

                            break;
                        }
                    case "u-":
                        {
                            if (operands[0].tknType == TokenType.MATRIS)
                            {
                                if (matDict.ContainsKey(operands[0].name))
                                    operands[0].val = matDict[operands[0].name]; 
                                else if(!(operands[0].val is MatrisBase<object>))
                                    throw new Exception("'" + operands[0].name + "' adlı bir matris bulunamadı");
                                
                            }

                            operands[0].val = -operands[0].val;
                            
                            break;
                        }
                    case "u+":
                        {
                            if (operands[0].tknType == TokenType.MATRIS)
                            {
                                if (matDict.ContainsKey(operands[0].name))
                                    operands[0].val = matDict[operands[0].name];
                                else if (!(operands[0].val is MatrisBase<object>))
                                    throw new Exception("'" + operands[0].name + "' adlı bir matris bulunamadı");
                            }
                            break;
                        }
                    case "=":
                        {
                            if (operands[0].tknType != TokenType.MATRIS || operands[1].tknType != TokenType.MATRIS)
                                throw new Exception("Atama işlemi sadece 'matris = matris' formatında olabilir");
                            else
                            {
                                if (matDict.ContainsKey(operands[1].name))
                                    matDict[operands[1].name] = operands[0].val;
                                else if (Validations.ValidMatrixName(operands[1].name))
                                    matDict.Add(operands[1].name, operands[0].val); // Ignore matris limit for now
                                else
                                {
                                    if (operands[1].name == "")
                                        throw new Exception("Format hatası!");
                                    else
                                        throw new Exception("Matris ismi " + operands[1].name + " olamaz.");
                                }
                            }
                            break;
                        }
                    default:
                        throw new Exception("Hatalı işlem operatörü");
                }
            }
            else                             // FUNCTIONS
            {
                //operands.Reverse();
                object[] param_arg = new object[op.paramCount];

                operands.Reverse();

                // Put values in order
                for (int k = 0; k < op.paramCount; k++)
                {
                    switch(operands[k].tknType)
                    {
                        case TokenType.NULL: param_arg[k] = null;break;

                        case TokenType.NUMBER: param_arg[k] = operands[k].val;break;

                        case TokenType.MATRIS:
                            {
                                if (matDict.ContainsKey(operands[k].name))
                                    param_arg[k] = matDict[operands[k].name];
                                else if((operands[k].val is MatrisBase<object>))
                                    param_arg[k] = operands[k].val;
                                else
                                    throw new Exception("'" + operands[k].name + "' adlı bir matris bulunamadı");
                                break;
                            }
                        default:
                            {
                                throw new Exception("Parametre türü tanımlanamadı");
                            }
                    }

                    switch(op.paramTypes[k])
                    {
                        case "int":
                            {
                                try
                                {
                                    param_arg[k] = ((int)param_arg[k]); break;
                                }
                                catch (Exception)
                                {
                                    param_arg[k] = Convert.ToInt32(param_arg[k]);break;
                                }
                            }

                        case "Matris": param_arg[k] = ((MatrisBase<dynamic>)param_arg[k]); break;
                        
                        case "float": param_arg[k] = ((MatrisBase<dynamic>)param_arg[k]); break;

                        default: throw new Exception("Fonksiyon parametre türü tanımlanamadı:" + op.paramTypes[k]);
                    }

                }
                if(op.service != "")
                {
                    object serviceObject;
                    Type serviceType;
                    // Get service type 
                    switch (op.service)
                    {
                        case "MatrisArithmeticService":
                            serviceType = typeof(MatrisArithmeticService<object>);break;
                        case "SpecialMatricesService":
                            serviceType = typeof(SpecialMatricesService); break;
                        default:
                            throw new Exception("Bilinmeyen servis ismi: " + op.service);
                    }

                    // Construct service
                    serviceObject = serviceType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });

                    // Get and Invoke the method
                    MethodInfo method = serviceType.GetMethod(op.name);
                    operands[0].val = (dynamic)method.Invoke(serviceObject, param_arg);

                    switch(op.returns)
                    {
                        case "Matris":operands[0].tknType = TokenType.MATRIS;break;
                        case "int": operands[0].tknType = TokenType.NUMBER; break;
                        case "float": operands[0].tknType = TokenType.NUMBER; break;
                        default: operands[0].tknType = TokenType.NULL; break;
                    }
                }
            }
            return operands[0];
        }

        public CommandState EvaluateCommand(Command cmd, Dictionary<string, MatrisBase<dynamic>> matdict)
        {
            List<Token> tkns = cmd.Tokens;

            switch (cmd.STATE)
            {
                // Komut ilk defa işlenmekte
                case CommandState.IDLE:
                    {
                        cmd.STATE = CommandState.UNAVAILABLE;
                        int ind = 0;
                        Stack<Token> operandStack = new Stack<Token>();
                        try
                        {
                            while (ind < tkns.Count)
                            {
                                Token tkn = tkns[ind];
                                if (tkn.tknType == TokenType.NUMBER || tkn.tknType == TokenType.MATRIS)
                                    operandStack.Push(tkn);

                                else
                                {
                                    if (operandStack.Count < tkn.paramCount)
                                        throw new Exception("Gerekli parameterlere değer verilmeli.");

                                    List<Token> operands = new List<Token>();
                                    for (int i = 0; i < tkn.paramCount; i++)
                                        operands.Add(operandStack.Pop());

                                    operandStack.Push(EvalOperator(tkn, operands, matdict));
                                }
                                ind++;
                            }

                            if (operandStack.Count == 1)
                            {
                                cmd.Output = operandStack.Pop().val.ToString();
                                cmd.STATE = CommandState.SUCCESS;
                                cmd.STATE_MESSAGE = "";
                            }
                            else
                            {
                                cmd.STATE = CommandState.ERROR;
                                cmd.STATE_MESSAGE = "Argüman sayısı hatalı.";
                            }
                        }
                        catch(Exception err)
                        {
                            cmd.STATE = CommandState.ERROR;
                            cmd.STATE_MESSAGE = err.Message;
                        }

                        break;
                    }
                // Komut işlenmekte veya hatalı
                case CommandState.UNAVAILABLE:
                    {
                        cmd.STATE_MESSAGE = "Komut işleme hatası-> \nOriginal:" + cmd.OriginalCommand + "\nCleaned:" + cmd.CleanedCommand;
                        break;
                    }
                // Komut zaten işlenmiş
                default:
                    {
                        cmd.STATE_MESSAGE = "Komut zaten işlenmiş. Durum: " + cmd.STATE + " Extra message: " + cmd.STATE_MESSAGE;
                        break;
                    }

            }

            return cmd.STATE;
        }
    }
}
