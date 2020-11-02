using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using MatrisAritmetik.Core.Models;
using Microsoft.AspNetCore.Http;

/* This file is for managing smaller classes, enumerators etc.
 * Created to reduce file amount.
 */
namespace MatrisAritmetik.Core
{
    // ENUM CLASSES
    // Command's states
    public enum CommandState { IDLE, UNAVAILABLE, SUCCESS, WARNING, ERROR };

    // For limiting matrix creation
    public enum MatrisLimits { forRows = 128, forCols = 128, forSize = 128*128, forMatrisCount = 8, forName = 64};

    // Token types
    public enum TokenType { NULL, NUMBER, MATRIS, FUNCTION, ARGSEPERATOR, OPERATOR, LEFTBRACE, RIGHTBRACE };

    // Operator order
    public enum OperatorAssociativity { LEFT, RIGHT };

    // Session stuff
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            string serialized = JsonSerializer.Serialize(value,typeof(T));
            session.SetString(key, serialized);
        }
        public static void SetCmdList(this ISession session, string key, List<Command> lis)
        {
            string serialized = "[";
            for(int i = 0; i < lis.Count; i++)
            {
                Command cmd = lis[i];
                Dictionary<string, dynamic> cmdinfo = new Dictionary<string, dynamic>();
                cmdinfo.Add("org", cmd.OriginalCommand);
                cmdinfo.Add("nset", cmd.NameSettings);
                cmdinfo.Add("vset", cmd.ValsSettings);
                cmdinfo.Add("output", cmd.Output);
                cmdinfo.Add("statid", (int)cmd.STATE);
                cmdinfo.Add("statmsg", cmd.STATE_MESSAGE);
                serialized += JsonSerializer.Serialize(cmdinfo, typeof(Dictionary<string, dynamic>));
                if (i != lis.Count - 1)
                    serialized += ",";
            }
            serialized += "]";
            session.SetString(key, serialized);
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
        public static List<Command> GetCmdList(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null || value == "" || value == "[]")
                return new List<Command>();

            List<Command> cmds = new List<Command>();
            foreach(Dictionary<string, dynamic> cmd in JsonSerializer.Deserialize<List<Dictionary<string,dynamic>>>(value))
            {
                int st = int.Parse(cmd["statid"].ToString());
                
                Dictionary<string, string> nset = JsonSerializer.Deserialize<Dictionary<string, string>>(cmd["nset"].ToString());
                Dictionary<string, string> vset = JsonSerializer.Deserialize<Dictionary<string, string>>(cmd["vset"].ToString());
                
                cmds.Add(new Command((string)(cmd["org"].ToString()),
                                     nset,
                                     vset,
                                     st,
                                     (string)(cmd["statmsg"].ToString()),
                                     (string)(cmd["output"].ToString()))); 
            }
            return cmds;
        }
    }

    public static class Validations
    {
        public static bool ValidMatrixName(string name)
        {
            Regex name_regex = new Regex(@"^\w*|[0-9]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (name.Replace(" ", "") == "")
                return false;

            if (name.Length > (int)MatrisLimits.forName)
                return false;

            return !("0123456789".Contains(name[0])) &&
                   (name_regex.Match(name).Groups[0].Value == name);
        }

    }
}
