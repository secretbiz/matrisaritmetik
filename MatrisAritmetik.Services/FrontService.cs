using System;
using System.Collections.Generic;
using System.Text;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Services;

namespace MatrisAritmetik.Services
{
    public class FrontService: IFrontService
    {
        public Dictionary<string, MatrisBase<float>> FloatMatrisDict = new Dictionary<string, MatrisBase<float>>();

        public void SetMatrisDict(Dictionary<string, MatrisBase<float>> MatrisDict)
        {
            FloatMatrisDict = MatrisDict;
        }

        public Dictionary<string, MatrisBase<float>> GetMatrisDict()
        {
            return FloatMatrisDict;
        }

        public void AddToMatrisDict(string name, MatrisBase<float> matris)
        {   
            if(!FloatMatrisDict.ContainsKey(name))
                FloatMatrisDict.Add(name, matris);
        }

        public void DeleteFromMatrisDict(string name)
        {
            if (FloatMatrisDict.ContainsKey(name))
                FloatMatrisDict.Remove(name);
        }
    }
}
