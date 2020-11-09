using System.Collections.Generic;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core.Services
{
    public interface IFrontService
    {
        void AddToMatrisDict(string name, MatrisBase<dynamic> mat, Dictionary<string, MatrisBase<dynamic>> matdict);

        void DeleteFromMatrisDict(string name, Dictionary<string, MatrisBase<dynamic>> matdict);

        void ReadCommandInformation();

        List<CommandLabel> GetCommandLabelList(List<string> filter = null);

        void AddToCommandLabelList(string label, CommandInfo[] commandInfos);

        void ClearCommandLabel(string label);

        Command CreateCommand(string cmd);

        bool TryParseBuiltFunc(string name, out CommandInfo cmdinfo);

        List<Token> Tokenize(string exp);

        List<Token> ShuntingYardAlg(List<Token> tkns);

        CommandState EvaluateCommand(Command cmd, Dictionary<string, MatrisBase<dynamic>> matdict);
    }
}
