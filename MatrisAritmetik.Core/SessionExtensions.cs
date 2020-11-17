using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MatrisAritmetik.Core.Models;
using Microsoft.AspNetCore.Http;

/* This file is for managing smaller classes, enumerators etc.
 * Created to reduce file amount.
 */
namespace MatrisAritmetik.Core
{
    // Session stuff
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session,
                                  string key,
                                  T value) => session.SetString(key, JsonSerializer.Serialize(value, typeof(T)));
        public static void SetCmdList(this ISession session, string key, List<Command> lis)
        {
            string serialized = "[";
            for (int i = 0; i < lis.Count; i++)
            {
                Command cmd = lis[i];
                Dictionary<string, dynamic> cmdinfo = new Dictionary<string, dynamic>
                {
                    { "org", cmd.OriginalCommand },
                    { "nset", cmd.NameSettings },
                    { "vset", cmd.ValsSettings },
                    { "output", cmd.Output },
                    { "statid", (int)cmd.STATE },
                    { "statmsg", cmd.STATE_MESSAGE }
                };
                serialized += JsonSerializer.Serialize(cmdinfo, typeof(Dictionary<string, dynamic>));
                if (i != lis.Count - 1)
                {
                    serialized += ",";
                }
            }
            serialized += "]";
            session.SetString(key, serialized);
        }
        public static void SetLastMsg(this ISession session,
                                      string key,
                                      CommandMessage msg)
        {
            string serialized = "";
            Dictionary<string, dynamic> cmdinfo = new Dictionary<string, dynamic>
            {
                { "msg", msg.Message },
                { "state", msg.State }
            };
            serialized += JsonSerializer.Serialize(cmdinfo, typeof(Dictionary<string, dynamic>));

            session.SetString(key, serialized);
        }
        public static void SetMatOptions(this ISession session,
                                         string key,
                                         Dictionary<string, Dictionary<string, dynamic>> dict)
        {
            Dictionary<string, Dictionary<string, dynamic>> mats = new Dictionary<string, Dictionary<string, dynamic>>();
            foreach (string mat in dict.Keys)
            {
                mats.Add(mat, new Dictionary<string, dynamic>
                {
                    {"seed",dict[mat]["seed"] },
                    {"isRandom",dict[mat]["isRandom"] }
                });
            }
            string serialized = "";
            serialized += JsonSerializer.Serialize(mats, typeof(Dictionary<string, Dictionary<string, dynamic>>));

            session.SetString(key, value: serialized);
        }

        public static T Get<T>(this ISession session,
                               string key)
        {
            string value = session.GetString(key);
            T t = default;
            return value == null ? t : JsonSerializer.Deserialize<T>(value);
        }
        public static List<Command> GetCmdList(this ISession session,
                                               string key)
        {
            string value = session.GetString(key);
            if (value == null || value == "" || value == "[]")
            {
                return new List<Command>();
            }

            return (from Dictionary<string, dynamic> cmd in JsonSerializer.Deserialize<List<Dictionary<string, dynamic>>>(value)
                    let st = int.Parse(cmd["statid"].ToString())
                    let nset = JsonSerializer.Deserialize<Dictionary<string, string>>(cmd["nset"].ToString())
                    let vset = JsonSerializer.Deserialize<Dictionary<string, string>>(cmd["vset"].ToString())
                    select new Command(org: (string)(cmd["org"].ToString()),
                                       nset: nset,
                                       vset: vset,
                                       stat: st,
                                       statmsg: (string)(cmd["statmsg"].ToString()),
                                       output: (string)(cmd["output"].ToString()))).ToList();
        }
        public static CommandMessage GetLastMsg(this ISession session,
                                                string key)
        {
            string value = session.GetString(key);
            if (value == null || value == "" || value == "{}")
            {
                return new CommandMessage("");
            }

            Dictionary<string, dynamic> msg = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(value);
            return new CommandMessage(msg: msg["msg"].ToString(),
                                      s: (CommandState)int.Parse(msg["state"].ToString()));
        }
        public static Dictionary<string, Dictionary<string, dynamic>> GetMatOptions(this ISession session,
                                                                                    string key)
        {
            string value = session.GetString(key);
            if (value == null || value == "" || value == "{}")
            {
                return new Dictionary<string, Dictionary<string, dynamic>>();
            }

            Dictionary<string, Dictionary<string, dynamic>> opts = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, dynamic>>>(value);
            foreach (string mat in opts.Keys)
            {
                opts[mat]["seed"] = int.Parse(opts[mat]["seed"].ToString());
                opts[mat]["isRandom"] = bool.Parse(opts[mat]["isRandom"].ToString());
            }
            return opts;
        }


    }
}
