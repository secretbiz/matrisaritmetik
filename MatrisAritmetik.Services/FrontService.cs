using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private Regex name_regex = new Regex(@"^[^0-9\s]\w*|[0-9]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public void AddToMatrisDict(string name, MatrisBase<float> matris, Dictionary<string,MatrisBase<float>> matdict)
        {
            if (name.Replace(" ", "") == "")
                return;

            Match match = name_regex.Match(name);
            
            if (match.Groups[0].Value == name && !matdict.ContainsKey(name))
                matdict.Add(name, matris);
        }

        public void DeleteFromMatrisDict(string name, Dictionary<string, MatrisBase<float>> matdict)
        {
            if (matdict.ContainsKey(name))
                matdict.Remove(name);
        }

        public Command CreateCommand(string cmd)
        {
            return new Command(cmd);
        }

        public CommandLabel[] builtInCommands;

        public void ReadCommandInformation()
        {
            using StreamReader r = new StreamReader("_builtInCmds.json");
            string json = r.ReadToEnd();
            builtInCommands = JsonConvert.DeserializeObject<CommandLabel[]>(json);
        }

        public CommandLabel[] GetCommandLabelList()
        {
            return builtInCommands;
        }

        public void AddToCommandLabelList(string label, CommandInfo[] commandInfos)
        {
            int labelIndex = Array.FindIndex(builtInCommands, cmdlbl => cmdlbl.Label == label);
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
            int labelIndex = Array.FindIndex(builtInCommands, cmdlbl => cmdlbl.Label == label);
            if (labelIndex != -1)
            {
                builtInCommands[labelIndex] = new CommandLabel() { Label = label };
            }
        }
    }
}
