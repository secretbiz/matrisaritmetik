using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Models.Core;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MatrisAritmetik.Core
{
    public class SpecialConverter : JsonConverter
    {
        private readonly Type[] _types;

        public SpecialConverter(params Type[] types)
        {
            _types = types;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

            if (value is null || value is None)
            {
                writer.WriteValue(SessionExtensions.CustomNull);
            }
            else if (float.TryParse(value.ToString(), out float res))
            {
                if (float.IsNaN(res))
                {
                    writer.WriteValue(SessionExtensions.CustomNan);
                }
                else if (float.IsPositiveInfinity(res))
                {
                    writer.WriteValue(SessionExtensions.CustomInf);
                }
                else if (float.IsNegativeInfinity(res))
                {
                    writer.WriteValue(SessionExtensions.CustomNinf);
                }
                else
                {
                    writer.WriteValue(res);
                }
            }
            else
            {
                writer.WriteValue(value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // This is not used
            string val = reader.ReadAsString() ?? "!null";
            return val switch
            {
                SessionExtensions.CustomNull => new None(),
                SessionExtensions.CustomNan => float.NaN,
                SessionExtensions.CustomInf => float.PositiveInfinity,
                SessionExtensions.CustomNinf => float.NegativeInfinity,
                _ => float.TryParse(val, out float d)
                    ? d
                    : float.TryParse(existingValue == null
                                ? string.Empty
                                : existingValue.ToString(), out float ex)
                        ? ex
                        : float.NaN
            };
        }

        public override bool CanRead => false;

        public override bool CanConvert(Type objectType)
        {
            return _types.Any(t => t == objectType);
        }
    }

    /// <summary>
    /// Class for setting and getting session variables
    /// </summary>
    public static class SessionExtensions
    {
        #region Custom values used in json write and read
        public const string CustomNull = "!!null";
        public const string CustomNan = "!!NaN";
        public const string CustomInf = "!!Infinify";
        public const string CustomNinf = "!!-Infinify";
        #endregion

        #region Static Public Methods
        public static object DecideFloat(string val, bool writeStrIfInvalid = false)
        {
            val = string.IsNullOrWhiteSpace(val) ? CustomNull : val;
            if (writeStrIfInvalid)
            {
                switch (val)
                {
                    case CustomNull:
                        return new None();
                    case CustomNan:
                        return float.NaN;
                    case CustomInf:
                        return float.PositiveInfinity;
                    case CustomNinf:
                        return float.NegativeInfinity;
                    default:
                        {
                            if (float.TryParse(val, out float d))
                            {
                                return d;
                            }
                            return val;
                        };
                }
            }

            return val switch
            {
                CustomNull => new None(),
                CustomNan => float.NaN,
                CustomInf => float.PositiveInfinity,
                CustomNinf => float.NegativeInfinity,
                _ => float.TryParse(val, out float d)
                    ? d
                    : float.NaN
            };
        }
        #endregion


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
                session.SetString(key, JsonConvert.SerializeObject(value, new SpecialConverter(typeof(float), typeof(None), typeof(object))));
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
                serialized.Append(JsonConvert.SerializeObject(cmdinfo));
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
            serialized += JsonConvert.SerializeObject(cmdinfo);

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
            serialized += JsonConvert.SerializeObject(mats);

            session.SetString(key, value: serialized);
        }

        /// <summary>
        /// Set dataframe labels variable
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Key for dataframe labels</param>
        /// <param name="dict">Dictionary of dataframe names and their labels with keys "col_labels" and "row_labels"</param>
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
        /// <param name="key">Key for dataframe settings</param>
        /// <param name="dict">Dictionary of dataframe names and their settings</param>
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
            serialized += JsonConvert.SerializeObject(mats);

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
            return value == null ? t : JsonConvert.DeserializeObject<T>(value);
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
            return value == null || string.IsNullOrEmpty(value) || value == "[]"
                ? new List<Command>()
                : (from Dictionary<string, dynamic> cmd in JsonConvert.DeserializeObject<List<Dictionary<string, dynamic>>>(value)
                   let st = int.Parse(cmd["statid"].ToString())
                   let nset = JsonConvert.DeserializeObject<Dictionary<string, string>>(cmd["nset"].ToString())
                   let vset = JsonConvert.DeserializeObject<Dictionary<string, string>>(cmd["vset"].ToString())
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

            Dictionary<string, dynamic> msg = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(value);
            return new CommandMessage(msg: msg["msg"].ToString(),
                                      s: (CommandState)int.Parse(msg["state"].ToString()));
        }
        /// <summary>
        /// Return matrix values
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Key name</param>
        /// <param name="allowNonNumber"> Wheter to allow non-numbers, when 'false' and value is non-number, it gets replaced with <see cref="float.NaN"/></param>
        /// <returns>Values of matrices</returns>
        public static Dictionary<string, List<List<object>>> GetMatVals(this ISession session,
                                                                        string key,
                                                                        bool allowNonNumber = false)
        {
            string value = session.GetString(key);
            if (value == null || string.IsNullOrEmpty(value) || value == "{}")
            {
                return new Dictionary<string, List<List<object>>>();
            }

            Dictionary<string, List<List<object>>> dict = new Dictionary<string, List<List<object>>>();

            if (allowNonNumber)
            {
                foreach (KeyValuePair<string, List<List<object>>> pair in JsonConvert.DeserializeObject<Dictionary<string, List<List<object>>>>(value, new SpecialConverter(typeof(float))))
                {
                    List<List<object>> objlis = new List<List<object>>();
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        objlis.Add(new List<object>());
                        for (int j = 0; j < pair.Value[i].Count; j++)
                        {
                            objlis[i].Add(DecideFloat(pair.Value[i][j].ToString(), true));
                        }
                    }
                    dict.Add(pair.Key, objlis);
                }
            }
            else
            {
                foreach (KeyValuePair<string, List<List<object>>> pair in JsonConvert.DeserializeObject<Dictionary<string, List<List<object>>>>(value, new SpecialConverter(typeof(float))))
                {
                    List<List<object>> objlis = new List<List<object>>();
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        objlis.Add(new List<object>());
                        for (int j = 0; j < pair.Value[i].Count; j++)
                        {
                            objlis[i].Add(DecideFloat(pair.Value[i][j].ToString()));
                        }
                    }
                    dict.Add(pair.Key, objlis);
                }
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

            Dictionary<string, Dictionary<string, dynamic>> opts = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(value);
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
        /// Gets the dataframe labels
        /// </summary>
        /// <param name="session">Current session</param>
        /// <param name="key">Dataframe labels key name</param>
        /// <returns>Dictionary of dataframe names and labels in a dictionary with keys "col_labels" and "row_labels"</returns>
        public static Dictionary<string, Dictionary<string, List<LabelList>>> GetDfLabels(this ISession session,
                                                                                          string key)
        {
            string value = session.GetString(key);
            return value == null || string.IsNullOrEmpty(value) || value == "{}"
                ? new Dictionary<string, Dictionary<string, List<LabelList>>>()
                : JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<LabelList>>>>(value);
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

            Dictionary<string, Dictionary<string, dynamic>> opts = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(value);
            foreach (string mat in opts.Keys)
            {
                opts[mat]["seed"] = int.Parse(opts[mat]["seed"].ToString());
                opts[mat]["isRandom"] = bool.Parse(opts[mat]["isRandom"].ToString());
                opts[mat]["row_settings"] = new DataframeRowSettings(JsonConvert.DeserializeObject<List<string>>(opts[mat]["row_settings"].ToString()));
                opts[mat]["col_settings"] = new DataframeColSettings(JsonConvert.DeserializeObject<List<string>>(opts[mat]["col_settings"].ToString()));
            }
            return opts;
        }

        /// <summary>
        /// Get the proper dataframe dictionary for the session
        /// </summary>
        /// <param name="session">Session to use</param>
        /// <param name="dfvalskey">Dataframe values key name</param>
        /// <param name="labelskey">Dataframe labels key name</param>
        /// <param name="settingskey">Dataframe settings key name</param>
        /// <returns>Dataframes in a dictionary</returns>
        public static Dictionary<string, Dataframe> GetDfDict(this ISession session,
                                                              string dfvalskey,
                                                              string labelskey,
                                                              string settingskey)
        {
            Dictionary<string, Dataframe> _dict = new Dictionary<string, Dataframe>();

            Dictionary<string, List<List<object>>> vals = session.GetMatVals(dfvalskey, true);
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
