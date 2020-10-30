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

        public void AddToMatrisDict(string name, MatrisBase<dynamic> matris, Dictionary<string,MatrisBase<dynamic>> matdict)
        {
            if (matdict.Count >= (int)MatrisLimits.forMatrisCount)
                return;

            if (name.Replace(" ", "") == "")
                return;

            Match match = name_regex.Match(name);
            
            if (match.Groups[0].Value == name)
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
    }
}
