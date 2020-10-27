using System;
using System.Collections.Generic;
using System.Text;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core.Services
{
    public interface IFrontService
    {
        void SetMatrisDict(Dictionary<string, MatrisBase<float>> MatrisDict);

        Dictionary<string, MatrisBase<float>> GetMatrisDict();

        void AddToMatrisDict(string name, MatrisBase<float> MatrisDict);

        void DeleteFromMatrisDict(string name);

        Command EvaluateCommand(string cmd);
    }
}
