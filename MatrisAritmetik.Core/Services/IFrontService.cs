using System;
using System.Collections.Generic;
using System.Text;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core.Services
{
    public interface IFrontService
    {
        void AddToMatrisDict(string name, MatrisBase<float> mat, Dictionary<string, MatrisBase<float>> matdict);

        void DeleteFromMatrisDict(string name, Dictionary<string, MatrisBase<float>> matdict);

        void ReadCommandInformation();

        CommandLabel[] GetCommandLabelList();

        void AddToCommandLabelList(string label, CommandInfo[] commandInfos);

        void ClearCommandLabel(string label);

        Command CreateCommand(string cmd);
    }
}
