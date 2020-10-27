using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;

namespace MatrisAritmetik.Services
{
    public class FrontService: IFrontService
    {
        public Dictionary<string, MatrisBase<float>> FloatMatrisDict = new Dictionary<string, MatrisBase<float>>();

        private Regex name_regex = new Regex(@"^[^0-9\s]\w*|[0-9]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
            Match match = name_regex.Match(name);
            
            if (match.Groups[0].Value == name && !FloatMatrisDict.ContainsKey(name))
                FloatMatrisDict.Add(name, matris);
        }

        public void DeleteFromMatrisDict(string name)
        {
            if (FloatMatrisDict.ContainsKey(name))
                FloatMatrisDict.Remove(name);
        }

        public Command EvaluateCommand(string cmd)
        {
            return new Command(cmd);
        }
    }
}
