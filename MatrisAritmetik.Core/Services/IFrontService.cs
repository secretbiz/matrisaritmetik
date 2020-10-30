using System;
using System.Collections.Generic;
using System.Text;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core.Services
{
    public interface IFrontService
    {
        void AddToMatrisDict(string name, MatrisBase<dynamic> mat, Dictionary<string, MatrisBase<dynamic>> matdict);

        void DeleteFromMatrisDict(string name, Dictionary<string, MatrisBase<dynamic>> matdict);

        void ReadCommandInformation();

        List<CommandLabel> GetCommandLabelList(List<string> filter=null);

        void AddToCommandLabelList(string label, CommandInfo[] commandInfos);

        void ClearCommandLabel(string label);

        Command CreateCommand(string cmd);
    }
}
