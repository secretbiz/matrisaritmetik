using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using MatrisAritmetik.Core.Models;
using Microsoft.AspNetCore.Http;

namespace MatrisAritmetik.Core
{
    /// <summary>
    /// Class for setting and getting session variables
    /// </summary>
    public static class SessionExtensions
    {
        #region Variable Setters
        /// <summary>
        /// Default session variable setter
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="value"/> to use while serializing</typeparam>
        /// <param name="session">Current session</param>
        /// <param name="key">Session variable key name</param>
        /// <param name="value">Session variable value</param>
        public static void Set<T>(this ISession session,
                                  string key,
                                  T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value, typeof(T)));
        }

        /// <summary>
        /// Set the command history variable
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Command history key</param>
        /// <param name="lis">Command history list to store</param>
        public static void SetCmdList(this ISession session, string key, List<Command> lis)
        {
            StringBuilder serialized = new StringBuilder();
            serialized.Append("[");
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
                serialized.Append(JsonSerializer.Serialize(cmdinfo, typeof(Dictionary<string, dynamic>)));
                if (i != lis.Count - 1)
                {
                    serialized.Append(",");
                }
            }
            serialized.Append("]");
            session.SetString(key, serialized.ToString());
        }

        /// <summary>
        /// Set the last message variable
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Last message variable key</param>
        /// <param name="msg">Last message</param>
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

        /// <summary>
        /// Set matrix options variable
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Key for matrix options</param>
        /// <param name="dict">Dictionary of matrix names and their options</param>
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
        #endregion

        #region Variable Getters
        /// <summary>
        /// Default session variable getter
        /// </summary>
        /// <typeparam name="T">Expected type of the <paramref name="key"/>'s value to use while deserializing</typeparam>
        /// <param name="session">Current session</param>
        /// <param name="key">Variable key name</param>
        /// <returns>Session variable named <paramref name="key"/> casted as <typeparamref name="T"/></returns>
        public static T Get<T>(this ISession session,
                               string key)
        {
            string value = session.GetString(key);
            T t = default;
            return value == null ? t : JsonSerializer.Deserialize<T>(value);
        }

        /// <summary>
        /// Gets the previous command history
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Command history key name</param>
        /// <returns>Command history from the previous session</returns>
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

        /// <summary>
        /// Gets the last message
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Last message key name</param>
        /// <returns>Last message from the previous session</returns>
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

        /// <summary>
        /// Gets the matrix options
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Matrix options key name</param>
        /// <returns>Dictionary of matrix names and options</returns>
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
        #endregion

    }
}
