using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using MatrisAritmetik.Core.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MatrisAritmetik.Core
{

    /// <summary>
    /// Override handling of doubles
    /// </summary>
    public class DoubleConverter : JsonConverter<double?>
    {
        public override double? ReadJson(JsonReader reader, Type objectType, [AllowNull] double? existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            string val = reader.ReadAsString();
            return val switch
            {
                "NaN" => double.NaN,
                "Infinity" => double.PositiveInfinity,
                "-Infinity" => double.NegativeInfinity,
                _ => double.TryParse(val, out double d) ? d : existingValue ?? double.NaN,
            };
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] double? value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value is null || !value.HasValue)
            {
                writer.WriteNull();
            }
            else if (double.IsNaN(value.Value))
            {
                writer.WriteValue("NaN");
            }
            else if (double.IsPositiveInfinity(value.Value))
            {
                writer.WriteValue("Infinity");
            }
            else if (double.IsNegativeInfinity(value.Value))
            {
                writer.WriteValue("-Infinity");
            }
            else
            {
                writer.WriteValue(value);
            }
        }
    }

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
            if (typeof(T) == typeof(Dictionary<string, List<List<dynamic>>>))
            {
                session.SetString(key, JsonConvert.SerializeObject(value, new DoubleConverter()));
                foreach (List<List<dynamic>> l in ((Dictionary<string, List<List<dynamic>>>)(dynamic)value).Values)
                {
                    foreach (List<dynamic> k in l)
                    {
                        k.Clear();
                    }
                    l.Clear();
                }
                ((Dictionary<string, List<List<dynamic>>>)(dynamic)value).Clear();
            }
            else
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }
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
            serialized.Append('[');
            for (int i = 0; i < lis.Count; i++)
            {
                Command cmd = lis[i];
                Dictionary<string, dynamic> cmdinfo = new Dictionary<string, dynamic>
                {
                    { "org", cmd.OriginalCommand },
                    { "nset", cmd.GetNameSettings() },
                    { "vset", cmd.GetValsSettings() },
                    { "output", cmd.Output },
                    { "statid", (int)cmd.STATE },
                    { "statmsg", cmd.GetStateMessage() }
                };
                serialized.Append(JsonSerializer.Serialize(cmdinfo, typeof(Dictionary<string, dynamic>)));
                if (i != lis.Count - 1)
                {
                    serialized.Append(',');
                }
            }
            serialized.Append(']');
            session.SetString(key, serialized.ToString());

            foreach (Command c in lis)
            {
                c.Dispose();
            }
            lis.Clear();
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

            msg.Dispose();
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
                    {"seed", dict[mat]["seed"] },
                    {"isRandom", dict[mat]["isRandom"] }
                });
            }
            string serialized = "";
            serialized += JsonSerializer.Serialize(mats, typeof(Dictionary<string, Dictionary<string, dynamic>>));

            session.SetString(key, value: serialized);
        }

        /// <summary>
        /// Set matrix options variable
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Key for matrix options</param>
        /// <param name="dict">Dictionary of matrix names and their options</param>
        public static void SetDfLabels(this ISession session,
                                       string key,
                                       Dictionary<string, Dictionary<string, List<LabelList>>> dict)
        {
            Dictionary<string, Dictionary<string, List<LabelList>>> mats = new Dictionary<string, Dictionary<string, List<LabelList>>>();
            foreach (string mat in dict.Keys)
            {
                mats.Add(mat, new Dictionary<string, List<LabelList>>
                {
                    {"col_labels", dict[mat]["col_labels"] },
                    {"row_labels", dict[mat]["row_labels"] }
                });
            }
            string serialized = "";
            serialized += JsonConvert.SerializeObject(mats);

            session.SetString(key, value: serialized);
        }

        /// <summary>
        /// Set dataframe settings variable
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Key for matrix options</param>
        /// <param name="dict">Dictionary of dataframe names and their options</param>
        public static void SetDfSettings(this ISession session,
                                         string key,
                                         Dictionary<string, Dictionary<string, dynamic>> dict)
        {
            Dictionary<string, Dictionary<string, dynamic>> mats = new Dictionary<string, Dictionary<string, dynamic>>();
            foreach (string mat in dict.Keys)
            {
                mats.Add(mat, new Dictionary<string, dynamic>
                {
                    {"seed", dict[mat]["seed"] },
                    {"isRandom", dict[mat]["isRandom"] },
                    {"row_settings", dict[mat]["row_settings"] },
                    {"col_settings", dict[mat]["col_settings"] }
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
            if (value == null || string.IsNullOrEmpty(value) || value == "[]")
            {
                return new List<Command>();
            }

            return (from Dictionary<string, dynamic> cmd in JsonSerializer.Deserialize<List<Dictionary<string, dynamic>>>(value)
                    let st = int.Parse(cmd["statid"].ToString())
                    let nset = JsonSerializer.Deserialize<Dictionary<string, string>>(cmd["nset"].ToString())
                    let vset = JsonSerializer.Deserialize<Dictionary<string, string>>(cmd["vset"].ToString())
                    select new Command(org: (string)cmd["org"].ToString(),
                                       nset: nset,
                                       vset: vset,
                                       stat: st,
                                       statmsg: (string)cmd["statmsg"].ToString(),
                                       output: (string)cmd["output"].ToString())).ToList();
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
            if (value == null || string.IsNullOrEmpty(value) || value == "{}")
            {
                return new CommandMessage("");
            }

            Dictionary<string, dynamic> msg = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(value);
            return new CommandMessage(msg: msg["msg"].ToString(),
                                      s: (CommandState)int.Parse(msg["state"].ToString()));
        }
        /// <summary>
        /// Return matrix values
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Key name</param>
        /// <returns>Values of matrices</returns>
        public static Dictionary<string, List<List<object>>> GetMatVals(this ISession session,
                                                                        string key)
        {
            string value = session.GetString(key);
            if (value == null || string.IsNullOrEmpty(value) || value == "{}")
            {
                return new Dictionary<string, List<List<object>>>();
            }

            Dictionary<string, List<List<object>>> dict = new Dictionary<string, List<List<object>>>();

            foreach (KeyValuePair<string, List<List<double>>> pair in JsonConvert.DeserializeObject<Dictionary<string, List<List<double>>>>(value))
            {
                List<List<object>> objlis = new List<List<object>>();
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    objlis.Add(new List<object>());
                    for (int j = 0; j < pair.Value[i].Count; j++)
                    {
                        objlis[i].Add(pair.Value[i][j]);
                    }
                }
                dict.Add(pair.Key, objlis);
            }
            return dict;
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
            if (value == null || string.IsNullOrEmpty(value) || value == "{}")
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

        /// <summary>
        /// Get the proper matrix dictionary for the session
        /// </summary>
        /// <param name="session">Session to use</param>
        /// <param name="matdictkey">Matrix values key name</param>
        /// <param name="optdictkey">Matrix options key name</param>
        /// <returns>Matrices in a dictionary</returns>
        public static Dictionary<string, MatrisBase<dynamic>> GetMatrixDict(this ISession session,
                                                                            string matdictkey,
                                                                            string optdictkey)
        {
            Dictionary<string, MatrisBase<dynamic>> _dict = new Dictionary<string, MatrisBase<dynamic>>();

            Dictionary<string, List<List<object>>> vals = session.GetMatVals(matdictkey);
            vals ??= new Dictionary<string, List<List<object>>>();

            Dictionary<string, Dictionary<string, dynamic>> opts = session.GetMatOptions(optdictkey);
            opts ??= new Dictionary<string, Dictionary<string, dynamic>>();

            foreach (string name in vals.Keys)
            {
                _dict.Add(name, new MatrisBase<object>(vals[name]));
                _dict[name].Seed = opts[name]["seed"];
                _dict[name].CreatedFromSeed = opts[name]["isRandom"];
            }

            vals.Clear();
            opts.Clear();
            return _dict;
        }

        /// <summary>
        /// Gets the matrix options
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Matrix options key name</param>
        /// <returns>Dictionary of matrix names and options</returns>
        public static Dictionary<string, Dictionary<string, List<LabelList>>> GetDfLabels(this ISession session,
                                                                                          string key)
        {
            string value = session.GetString(key);
            if (value == null || string.IsNullOrEmpty(value) || value == "{}")
            {
                return new Dictionary<string, Dictionary<string, List<LabelList>>>();
            }

            Dictionary<string, Dictionary<string, List<LabelList>>> opts = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<LabelList>>>>(value);
            foreach (string mat in opts.Keys)
            {
                opts[mat]["col_labels"] = opts[mat]["col_labels"];
                opts[mat]["row_labels"] = opts[mat]["row_labels"];
            }
            return opts;
        }

        /// <summary>
        /// Gets the dataframe settings
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Dataframe options key name</param>
        /// <returns>Dictionary of dataframe names and options</returns>
        public static Dictionary<string, Dictionary<string, dynamic>> GetDfSettings(this ISession session,
                                                                                    string key)
        {
            string value = session.GetString(key);
            if (value == null || string.IsNullOrEmpty(value) || value == "{}")
            {
                return new Dictionary<string, Dictionary<string, dynamic>>();
            }

            Dictionary<string, Dictionary<string, dynamic>> opts = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, dynamic>>>(value);
            foreach (string mat in opts.Keys)
            {
                opts[mat]["seed"] = int.Parse(opts[mat]["seed"].ToString());
                opts[mat]["isRandom"] = bool.Parse(opts[mat]["isRandom"].ToString());
                opts[mat]["row_settings"] = new DataframeRowSettings(JsonSerializer.Deserialize<List<string>>(opts[mat]["row_settings"].ToString()));
                opts[mat]["col_settings"] = new DataframeColSettings(JsonSerializer.Deserialize<List<string>>(opts[mat]["col_settings"].ToString()));
            }
            return opts;
        }

        /// <summary>
        /// Get the proper matrix dictionary for the session
        /// </summary>
        /// <param name="session">Session to use</param>
        /// <param name="dfdictkey">Matrix values key name</param>
        /// <param name="optdictkey">Matrix options key name</param>
        /// <returns>Matrices in a dictionary</returns>
        public static Dictionary<string, Dataframe> GetDfDict(this ISession session,
                                                              string dfdictkey,
                                                              string labelskey,
                                                              string settingskey)
        {
            Dictionary<string, Dataframe> _dict = new Dictionary<string, Dataframe>();

            Dictionary<string, List<List<object>>> vals = session.GetMatVals(dfdictkey);
            vals ??= new Dictionary<string, List<List<object>>>();

            Dictionary<string, Dictionary<string, List<LabelList>>> labels = session.GetDfLabels(labelskey);
            labels ??= new Dictionary<string, Dictionary<string, List<LabelList>>>();

            Dictionary<string, Dictionary<string, dynamic>> settings = session.GetDfSettings(settingskey);
            settings ??= new Dictionary<string, Dictionary<string, dynamic>>();

            Dataframe df;
            foreach (string name in vals.Keys)
            {
                df = new Dataframe(
                                       vals[name],
                                       rowLabels: labels[name]["row_labels"],
                                       colLabels: labels[name]["col_labels"],
                                       rowSettings: settings[name]["row_settings"],
                                       colSettings: settings[name]["col_settings"]
                                  );
                _dict.Add(name, df);
            }

            vals.Clear();
            labels.Clear();
            return _dict;
        }

        #endregion

    }
}
