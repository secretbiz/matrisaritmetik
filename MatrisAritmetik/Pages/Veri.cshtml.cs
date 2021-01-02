using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MatrisAritmetik.Pages
{
    public class VeriModel : PageModel
    {
        #region Session Variable Names
        private const string SessionMatrisDict = "_MatDictVals";
        private const string SessionSeedDict = "_MatDictSeed";
        private const string SessionDfDict = "_dfDictVals";
        private const string SessionDfSettings = "_dfSettings";
        private const string SessionDfLabels = "_dfLabels";
        private const string SessionLastMessage = "_dflastMsg";
        private const string SessionOutputHistory = "_dfoutputHistory";
        private const string SessionLastCommandDate = "_dflastCmdDate";
        private const string SessionLastOutputDate = "_dflastOutDate";
        #endregion

        #region Expected Request Parameter Names
        private const string CommandParam = "cmd";
        private const string DfNameParam = "name";
        private const string DfValsParam = "vals";
        private const string DfDelimParam = "delimiter";
        private const string DfNewLineParam = "newline";
        private const string DfSpecialFuncParam = "func";
        private const string DfSpecialArgsParam = "args";
        private const string DfColIndexParam = "cols";
        #endregion

        #region ViewData Keys
        public const string LastMessageKey = "LastMessage";
        public const string CommandHistoryKey = "CommandHistory";
        #endregion

        #region Service Interface Instances
        private readonly ILogger<MatrisModel> _logger;
        /// <summary>
        /// Front-end related methods
        /// </summary>
        private readonly IFrontService _frontService;
        /// <summary>
        /// String manipulation methods
        /// </summary>
        private readonly IUtilityService<dynamic> _utils;
        /// <summary>
        /// Matrix operations
        /// </summary>
        private readonly IMatrisArithmeticService<object> _matService;
        /// <summary>
        /// Statistics
        /// </summary>
        private readonly IStatisticsService _statService;
        #endregion

        #region Page Constructor
        public VeriModel(ILogger<MatrisModel> logger,
                         IUtilityService<dynamic> utilityService,
                         IFrontService frontService,
                         IMatrisArithmeticService<object> matService,
                         IStatisticsService stattService)
        {
            _logger = logger;
            _utils = utilityService;
            _frontService = frontService;
            _matService = matService;
            _statService = stattService;
        }
        #endregion

        #region Temporary Variables 
        /// <summary>
        /// List of parameters that should be ignored while reading the request body
        /// </summary>
        private readonly List<string> IgnoredParams = new List<string>() { "__RequestVerificationToken", "RequestVerificationToken" };
        /// <summary>
        /// List of special labels used for special matrices
        /// </summary>
        private readonly List<string> SpecialsLabels = new List<string>() { "Özel Veri Tablosu" };
        #endregion

        public List<CommandLabel> SpecialFuncTab => _frontService.GetCommandLabelList(SpecialsLabels);

        #region GET Actions
        /// <summary>
        /// Default GET action
        /// </summary>
        public async Task OnGetAsync()
        {
            ViewData["komut_optionsdict"] = await _frontService.GetCommandLabelListAsync(null, true);

            ViewData["special_optiondict"] = await _frontService.GetCommandLabelListAsync(SpecialsLabels);

            ViewData["matrix_dict"] = HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict);

            ViewData["df_dict"] = HttpContext.Session.GetDfDict(SessionDfDict, SessionDfLabels, SessionDfSettings);

            ViewData["output_history"] = new Dictionary<string, dynamic>()
            {
                {LastMessageKey, HttpContext.Session.GetLastMsg(SessionLastMessage) },
                {CommandHistoryKey, HttpContext.Session.GetCmdList(SessionOutputHistory) }
            };
        }
        #endregion

        #region POST Actions
        /// <summary>
        /// Default post action
        /// <para> DO NOT USE </para>
        /// </summary>
        public void OnPost()
        {
            using CommandMessage msg = new CommandMessage(string.Empty, CommandState.SUCCESS);
            HttpContext.Session.SetLastMsg(
                                            SessionLastMessage,
                                            msg
                                          );
        }

        /// <summary>
        /// Create and save a dataframe with given name and values from text
        /// </summary>
        public async Task OnPostAddDataframe()
        {
            if (_frontService.CheckCmdDate(HttpContext.Session.Get<DateTime>(SessionLastCommandDate)))
            {
                DateTime LastCmdDate = DateTime.Now;

                Dictionary<string, string> reqdict = new Dictionary<string, string>();
                await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, reqdict).ConfigureAwait(false);

                if (reqdict.ContainsKey(DfNameParam)
                    && reqdict.ContainsKey(DfValsParam)
                    && reqdict.ContainsKey(DfDelimParam)
                    && reqdict.ContainsKey(DfNewLineParam)
                   )
                {
                    Dictionary<string, Dataframe> _dict = HttpContext.Session.GetDfDict(SessionDfDict, SessionDfLabels, SessionDfSettings);

                    if (!HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict).ContainsKey(reqdict[DfNameParam]))
                    {
                        if (!_dict.ContainsKey(reqdict[DfNameParam]))
                        {
                            try
                            {
                                Validations.ValidMatrixName(reqdict[DfNameParam], throwOnBadName: true);

                                reqdict[DfDelimParam] = _utils.FixLiterals(reqdict[DfDelimParam]);
                                reqdict[DfNewLineParam] = _utils.FixLiterals(reqdict[DfNewLineParam]);
                                reqdict[DfValsParam] = _utils.FixLiterals(reqdict[DfValsParam]);

                                List<string> optlist = _utils.GetOptionList(reqdict);

                                using Dataframe df = new Dataframe
                                                                (
                                                                    _utils.StringTo2DList
                                                                    (
                                                                        reqdict[DfValsParam],
                                                                        reqdict[DfDelimParam],
                                                                        reqdict[DfNewLineParam],
                                                                        true,
                                                                        (int)DataframeLimits.forRows,
                                                                        (int)DataframeLimits.forCols,
                                                                        true,
                                                                        optlist
                                                                    ),
                                                                    optlist
                                                                );
                                _frontService.AddToDfDict
                                (
                                    reqdict[DfNameParam],
                                    df,
                                    _dict
                                );

                                Dictionary<string, List<List<object>>> vals = HttpContext.Session.GetMatVals(SessionDfDict, true);
                                Dictionary<string, Dictionary<string, dynamic>> settings = HttpContext.Session.GetDfSettings(SessionDfSettings);
                                Dictionary<string, Dictionary<string, List<LabelList>>> labels = HttpContext.Session.GetDfLabels(SessionDfLabels);

                                _frontService.SetDfDicts(_dict, vals, labels, settings);

                                HttpContext.Session.Set(SessionDfDict, vals);
                                HttpContext.Session.SetDfLabels(SessionDfLabels, labels);
                                HttpContext.Session.SetDfSettings(SessionDfSettings, settings);

                                using CommandMessage msg = new CommandMessage(CompilerMessage.SAVED_DF(reqdict[DfNameParam]), CommandState.SUCCESS);
                                HttpContext.Session.SetLastMsg(
                                                                SessionLastMessage,
                                                                msg
                                                              );

                                _utils.DisposeDfDicts(null, labels);
                                vals.Clear();
                                labels.Clear();
                                settings.Clear();
                            }
                            catch (Exception err)
                            {
                                using CommandMessage msg = err.InnerException != null
                                                    ? new CommandMessage(err.InnerException.Message, CommandState.ERROR)
                                                    : new CommandMessage(err.Message, CommandState.ERROR);

                                HttpContext.Session.SetLastMsg(
                                                                SessionLastMessage,
                                                                msg
                                                              );
                            }
                        }
                        else
                        {
                            using CommandMessage msg = new CommandMessage(CompilerMessage.DF_NAME_ALREADY_EXISTS(reqdict[DfNameParam]), CommandState.WARNING);
                            HttpContext.Session.SetLastMsg(
                                                            SessionLastMessage,
                                                            msg
                                                          );
                        }
                    }
                    else
                    {
                        using CommandMessage msg = new CommandMessage(CompilerMessage.MAT_NAME_ALREADY_EXISTS(reqdict[DfNameParam]), CommandState.WARNING);
                        HttpContext.Session.SetLastMsg(
                                                        SessionLastMessage,
                                                        msg
                                                      );
                    }

                    _utils.DisposeDfDicts(_dict, null);
                    _dict.Clear();
                }
                else
                {
                    using CommandMessage msg = new CommandMessage(RequestMessage.REQUEST_MISSING_KEYS("AddDataframe", new string[2] { DfNameParam, DfValsParam }), CommandState.ERROR);
                    HttpContext.Session.SetLastMsg(
                                                    SessionLastMessage,
                                                    msg
                                                  );
                }

                HttpContext.Session.Set(SessionLastCommandDate, LastCmdDate);
            }
            else
            {
                using CommandMessage msg = new CommandMessage(RequestMessage.REQUEST_SPAM(HttpContext.Session.Get<DateTime>(SessionLastCommandDate)), CommandState.WARNING);
                HttpContext.Session.SetLastMsg(
                                                SessionLastMessage,
                                                msg
                                              );
            }

        }


        /// <summary>
        /// Create and save a dataframe from given special class with given arguments and name
        /// </summary>
        public async Task OnPostAddDataframeSpecial()
        {
            if (_frontService.CheckCmdDate(HttpContext.Session.Get<DateTime>(SessionLastCommandDate)))
            {
                DateTime LastCmdDate = DateTime.Now;

                Dictionary<string, string> reqdict = new Dictionary<string, string>();
                await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, reqdict).ConfigureAwait(false);

                if (reqdict.ContainsKey(DfNameParam) && reqdict.ContainsKey(DfSpecialFuncParam) && reqdict.ContainsKey(DfSpecialArgsParam))
                {

                    Dictionary<string, Dataframe> _dict = HttpContext.Session.GetDfDict(SessionDfDict, SessionDfLabels, SessionDfSettings);

                    if (!_dict.ContainsKey(reqdict[DfNameParam]))
                    {
                        if (HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict).ContainsKey(reqdict[DfNameParam]))
                        {
                            HttpContext.Session.Set(SessionLastCommandDate, LastCmdDate);
                            using CommandMessage msg = new CommandMessage(CompilerMessage.MAT_NAME_ALREADY_EXISTS(reqdict[DfNameParam]), CommandState.WARNING);
                            HttpContext.Session.SetLastMsg(
                                                            SessionLastMessage,
                                                            msg
                                                          );
                            _utils.DisposeDfDicts(_dict, null);
                            return;
                        }
                    }
                    else
                    {
                        HttpContext.Session.Set(SessionLastCommandDate, LastCmdDate);
                        using CommandMessage msg = new CommandMessage(CompilerMessage.DF_NAME_ALREADY_EXISTS(reqdict[DfNameParam]), CommandState.WARNING);
                        HttpContext.Session.SetLastMsg(
                                                        SessionLastMessage,
                                                        msg
                                                      );
                        _utils.DisposeDfDicts(_dict, null);
                        return;
                    }
                    string actualFuncName = reqdict[DfSpecialFuncParam][1..reqdict[DfSpecialFuncParam].IndexOf("(", StringComparison.CurrentCulture)];

                    if (string.IsNullOrEmpty(actualFuncName))
                    {
                        HttpContext.Session.Set(SessionLastCommandDate, LastCmdDate);
                        using CommandMessage msg = new CommandMessage(CompilerMessage.NOT_A_(actualFuncName, "fonksiyon"), CommandState.ERROR);
                        HttpContext.Session.SetLastMsg(
                                                        SessionLastMessage,
                                                        msg
                                                      );
                        _utils.DisposeDfDicts(_dict, null);
                        return;
                    }

                    using CommandInfo cmdinfo = _frontService.TryParseBuiltFunc(actualFuncName, SpecialsLabels);
                    if (cmdinfo != null)
                    {
                        try
                        {
                            Validations.ValidMatrixName(reqdict[DfNameParam], throwOnBadName: true);


                            List<string> optlist = _utils.GetOptionList(reqdict);

                            using Dataframe df = _utils.SpecialStringToDataframe
                                                 (
                                                     reqdict[DfSpecialArgsParam],
                                                     cmdinfo,
                                                     _dict,
                                                     options: optlist,
                                                     mode: CompilerDictionaryMode.All
                                                 );

                            _frontService.AddToDfDict
                            (
                                reqdict[DfNameParam],
                                df,
                                _dict
                            );

                            Dictionary<string, List<List<object>>> vals = HttpContext.Session.GetMatVals(SessionDfDict, true);
                            Dictionary<string, Dictionary<string, List<LabelList>>> labels = HttpContext.Session.GetDfLabels(SessionDfLabels);
                            Dictionary<string, Dictionary<string, dynamic>> settings = HttpContext.Session.GetDfSettings(SessionDfSettings);

                            _frontService.SetDfDicts(_dict, vals, labels, settings);

                            HttpContext.Session.Set(SessionDfDict, vals);
                            HttpContext.Session.SetDfLabels(SessionDfLabels, labels);
                            HttpContext.Session.SetDfSettings(SessionDfSettings, settings);

                            using CommandMessage msg = new CommandMessage(CompilerMessage.SAVED_DF(reqdict[DfNameParam]), CommandState.SUCCESS);
                            HttpContext.Session.SetLastMsg(
                                                            SessionLastMessage,
                                                            msg
                                                          );

                            _utils.DisposeDfDicts(null, labels);
                            vals.Clear();
                            labels.Clear();
                            settings.Clear();

                        }
                        catch (Exception err)
                        {
                            using CommandMessage msg = err.InnerException != null
                                                ? new CommandMessage(err.InnerException.Message, CommandState.ERROR)
                                                : new CommandMessage(err.Message, CommandState.ERROR);

                            HttpContext.Session.SetLastMsg(
                                                            SessionLastMessage,
                                                            msg
                                                          );
                        }

                        _utils.DisposeDfDicts(_dict, null);

                        _dict.Clear();
                    }
                    else
                    {
                        using CommandMessage msg = new CommandMessage(CompilerMessage.NOT_A_(actualFuncName, "fonksiyon"), CommandState.ERROR);
                        HttpContext.Session.SetLastMsg(
                                                        SessionLastMessage,
                                                        msg
                                                      );
                    }
                }
                else
                {
                    using CommandMessage msg = new CommandMessage(RequestMessage.REQUEST_MISSING_KEYS(
                                                                                    "AddDataframeSpecial",
                                                                                    new string[3] { DfNameParam, DfSpecialFuncParam, DfSpecialArgsParam }),
                                                                                    CommandState.ERROR);
                    HttpContext.Session.SetLastMsg(
                                                    SessionLastMessage,
                                                    msg
                                                  );
                }

                HttpContext.Session.Set(SessionLastCommandDate, LastCmdDate);
            }
            else
            {

                using CommandMessage msg = new CommandMessage(RequestMessage.REQUEST_SPAM(HttpContext.Session.Get<DateTime>(SessionLastCommandDate)), CommandState.WARNING);
                HttpContext.Session.SetLastMsg(
                                                SessionLastMessage,
                                                msg
                                              );
            }

        }


        /// <summary>
        /// Try to read the uploaded file 
        /// </summary>
        public async Task OnPostUploadFile()
        {
            if (_frontService.CheckCmdDate(HttpContext.Session.Get<DateTime>(SessionLastCommandDate)))
            {
                DateTime LastCmdDate = DateTime.Now;

                Dictionary<string, string> FileData = new Dictionary<string, string>();

                await _utils.ReadFileFromRequest(Request.Body, Encoding.Default, FileData).ConfigureAwait(false);

                Dictionary<string, Dataframe> _dict = HttpContext.Session.GetDfDict(SessionDfDict, SessionDfLabels, SessionDfSettings);

                if (!HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict).ContainsKey(FileData["name"]))
                {
                    if (!_dict.ContainsKey(FileData["name"]))
                    {
                        try
                        {
                            Validations.ValidMatrixName(FileData["name"], throwOnBadName: true);

                            List<string> optlist = _utils.GetOptionList(FileData);

                            using Dataframe mat = new Dataframe
                                                            (
                                                                _utils.StringTo2DList
                                                                (
                                                                    FileData["data"],
                                                                    FileData["delim"],
                                                                    FileData["newline"],
                                                                    true,
                                                                    (int)DataframeLimits.forRows,
                                                                    (int)DataframeLimits.forCols,
                                                                    true,
                                                                    optlist
                                                                ),
                                                                optlist
                                                            );
                            _frontService.AddToDfDict
                            (
                                FileData["name"],
                                mat,
                                _dict
                            );


                            Dictionary<string, List<List<object>>> vals = HttpContext.Session.GetMatVals(SessionDfDict, true);
                            Dictionary<string, Dictionary<string, List<LabelList>>> labels = HttpContext.Session.GetDfLabels(SessionDfLabels);
                            Dictionary<string, Dictionary<string, dynamic>> settings = HttpContext.Session.GetDfSettings(SessionDfSettings);

                            _frontService.SetDfDicts(_dict, vals, labels, settings);

                            HttpContext.Session.Set(SessionDfDict, vals);
                            HttpContext.Session.SetDfLabels(SessionDfLabels, labels);
                            HttpContext.Session.SetDfSettings(SessionDfSettings, settings);

                            using CommandMessage msg = new CommandMessage(CompilerMessage.SAVED_DF(FileData["name"]), CommandState.SUCCESS);
                            HttpContext.Session.SetLastMsg(
                                                            SessionLastMessage,
                                                            msg
                                                          );

                            _utils.DisposeDfDicts(null, labels);

                            vals.Clear();
                            labels.Clear();
                            settings.Clear();
                        }
                        catch (Exception err)
                        {
                            using CommandMessage msg = err.InnerException != null
                                                ? new CommandMessage(err.InnerException.Message, CommandState.ERROR)
                                                : new CommandMessage(err.Message, CommandState.ERROR);

                            HttpContext.Session.SetLastMsg(
                                                            SessionLastMessage,
                                                            msg
                                                          );
                        }
                    }
                    else
                    {
                        using CommandMessage msg = new CommandMessage(CompilerMessage.DF_NAME_ALREADY_EXISTS(FileData["name"]), CommandState.WARNING);
                        HttpContext.Session.SetLastMsg(
                                                        SessionLastMessage,
                                                        msg
                                                      );
                    }
                }
                else
                {
                    using CommandMessage msg = new CommandMessage(CompilerMessage.MAT_NAME_ALREADY_EXISTS(FileData["name"]), CommandState.WARNING);
                    HttpContext.Session.SetLastMsg(
                                                    SessionLastMessage,
                                                    msg
                                                  );
                }

                _utils.DisposeDfDicts(_dict, null);
                _dict.Clear();
                FileData.Clear();

                HttpContext.Session.Set(SessionLastCommandDate, LastCmdDate);
            }
            else
            {
                using CommandMessage msg = new CommandMessage(RequestMessage.REQUEST_SPAM(HttpContext.Session.Get<DateTime>(SessionLastCommandDate)), CommandState.WARNING);
                HttpContext.Session.SetLastMsg(
                                                SessionLastMessage,
                                                msg
                                              );
            }

        }

        /// <summary>
        /// Remove a matrix from the table
        /// </summary>
        public async Task OnPostDeleteMatrix()
        {
            Dictionary<string, string> reqdict = new Dictionary<string, string>();

            await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, reqdict).ConfigureAwait(false);

            if (reqdict.ContainsKey(DfNameParam))
            {
                reqdict[DfNameParam] = reqdict[DfNameParam].Replace("matris_table_delbutton_", "");

                Dictionary<string, MatrisBase<dynamic>> _dict = HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict);
                Dictionary<string, List<List<object>>> vals = HttpContext.Session.GetMatVals(SessionMatrisDict, true);
                Dictionary<string, Dictionary<string, dynamic>> opts = HttpContext.Session.GetMatOptions(SessionSeedDict);

                if (_frontService.DeleteFromMatrisDict(reqdict[DfNameParam], _dict))
                {
                    vals.Remove(reqdict[DfNameParam]);
                    opts.Remove(reqdict[DfNameParam]);
                }

                _frontService.SetMatrixDicts(_dict, vals, opts);

                HttpContext.Session.Set(SessionMatrisDict, vals);
                HttpContext.Session.SetMatOptions(SessionSeedDict, opts);

                using CommandMessage msg = new CommandMessage(CompilerMessage.DELETED_MATRIX(reqdict[DfNameParam]), CommandState.SUCCESS);
                HttpContext.Session.SetLastMsg(
                                                SessionLastMessage,
                                                msg
                                              );

                _dict.Clear();
                vals.Clear();
                opts.Clear();
            }

            reqdict.Clear();
        }

        /// <summary>
        /// Remove a dataframe from the table
        /// </summary>
        public async Task OnPostDeleteDataframe()
        {
            Dictionary<string, string> reqdict = new Dictionary<string, string>();

            await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, reqdict).ConfigureAwait(false);

            if (reqdict.ContainsKey(DfNameParam))
            {
                reqdict[DfNameParam] = reqdict[DfNameParam].Replace("veri_table_delbutton_", "");

                Dictionary<string, Dataframe> _dict = HttpContext.Session.GetDfDict(SessionDfDict, SessionDfLabels, SessionDfSettings);
                Dictionary<string, List<List<object>>> vals = HttpContext.Session.GetMatVals(SessionDfDict, true);
                Dictionary<string, Dictionary<string, List<LabelList>>> labels = HttpContext.Session.GetDfLabels(SessionDfLabels);
                Dictionary<string, Dictionary<string, dynamic>> settings = HttpContext.Session.GetDfSettings(SessionDfSettings);

                if (_frontService.DeleteFromDfDict(reqdict[DfNameParam], _dict))
                {
                    vals.Remove(reqdict[DfNameParam]);
                    labels.Remove(reqdict[DfNameParam]);
                    settings.Remove(reqdict[DfNameParam]);
                }

                _frontService.SetDfDicts(_dict, vals, labels, settings);

                HttpContext.Session.Set(SessionDfDict, vals);
                HttpContext.Session.SetDfLabels(SessionDfLabels, labels);
                HttpContext.Session.SetDfSettings(SessionDfSettings, settings);

                _utils.DisposeDfDicts(_dict, labels);

                using CommandMessage msg = new CommandMessage(CompilerMessage.DELETED_DF(reqdict[DfNameParam]), CommandState.SUCCESS);
                HttpContext.Session.SetLastMsg(
                                                SessionLastMessage,
                                                msg
                                              );

                _dict.Clear();
                vals.Clear();
                labels.Clear();
                settings.Clear();
            }

            reqdict.Clear();
        }

        /// <summary>
        /// Create and evaluate a command
        /// </summary>
        public async Task OnPostSendCmd()
        {
            DateTime LastCmdDate = DateTime.Now;

            if (_frontService.CheckCmdDate(HttpContext.Session.Get<DateTime>(SessionLastOutputDate)))
            {
                Dictionary<string, string> reqdict = new Dictionary<string, string>();
                await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, reqdict).ConfigureAwait(false);

                if (reqdict.ContainsKey(CommandParam))
                {
                    if (!string.IsNullOrEmpty(reqdict[CommandParam].Trim()))
                    {
                        List<Command> cmdlis = HttpContext.Session.GetCmdList(SessionOutputHistory);
                        using Command cmd = _frontService.CreateCommand(reqdict[CommandParam]);

                        try
                        {
                            Dictionary<string, MatrisBase<dynamic>> _dict = HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict);
                            Dictionary<string, MatrisBase<dynamic>> tempdict = HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict);
                            Dictionary<string, List<List<object>>> vals = HttpContext.Session.GetMatVals(SessionMatrisDict);
                            Dictionary<string, Dictionary<string, dynamic>> opts = HttpContext.Session.GetMatOptions(SessionSeedDict);

                            Dictionary<string, Dataframe> dfdict = HttpContext.Session.GetDfDict(SessionDfDict, SessionDfLabels, SessionDfSettings);
                            Dictionary<string, List<List<object>>> dfvals = HttpContext.Session.GetMatVals(SessionDfDict, true);
                            Dictionary<string, Dictionary<string, List<LabelList>>> dflabels = HttpContext.Session.GetDfLabels(SessionDfLabels);
                            Dictionary<string, Dictionary<string, dynamic>> dfsettings = HttpContext.Session.GetDfSettings(SessionDfSettings);

                            foreach (KeyValuePair<string, Dataframe> pair in dfdict)
                            {
                                tempdict.Add(pair.Key, pair.Value);
                            }

                            cmd.SetTokens(_frontService.ShuntingYardAlg(_frontService.Tokenize(cmd.GetTermsToEvaluate()[0],
                                                                                               CompilerDictionaryMode.All)));

                            _frontService.EvaluateCommand(cmd,
                                                          tempdict,
                                                          cmdlis,
                                                          CompilerDictionaryMode.All);

                            using CommandMessage msg = new CommandMessage(cmd.GetStateMessage(), cmd.STATE);
                            HttpContext.Session.SetLastMsg(SessionLastMessage,
                                                           msg);

                            foreach (string key in tempdict.Keys)
                            {
                                if (tempdict[key] is Dataframe df)
                                {
                                    if (dfdict.ContainsKey(key))
                                    {
                                        dfdict[key] = df;
                                    }
                                    else
                                    {
                                        dfdict.Add(key, df);
                                    }

                                    if (_dict.ContainsKey(key))
                                    {
                                        _dict.Remove(key);
                                        vals.Remove(key);
                                        opts.Remove(key);
                                    }
                                }
                                else
                                {
                                    if (_dict.ContainsKey(key))
                                    {
                                        _dict[key] = tempdict[key];
                                    }
                                    else
                                    {
                                        _dict.Add(key, tempdict[key]);
                                    }

                                    if (dfdict.ContainsKey(key))
                                    {
                                        dfdict.Remove(key);
                                        dfvals.Remove(key);
                                        dflabels.Remove(key);
                                        dfsettings.Remove(key);
                                    }
                                }
                            }

                            _frontService.SetMatrixDicts(_dict, vals, opts);
                            HttpContext.Session.Set(SessionMatrisDict, vals);
                            HttpContext.Session.SetMatOptions(SessionSeedDict, opts);

                            _frontService.SetDfDicts(dfdict, dfvals, dflabels, dfsettings);
                            HttpContext.Session.Set(SessionDfDict, dfvals);
                            HttpContext.Session.SetDfLabels(SessionDfLabels, dflabels);
                            HttpContext.Session.SetDfSettings(SessionDfSettings, dfsettings);

                            _utils.DisposeDfDicts(null, dflabels);

                            foreach (MatrisBase<dynamic> d in tempdict.Values)
                            {
                                d.Dispose();
                            }

                            tempdict.Clear();
                            _dict.Clear();
                            vals.Clear();
                            opts.Clear();
                            dfdict.Clear();
                            dfvals.Clear();
                            dflabels.Clear();
                            dfsettings.Clear();
                        }
                        catch (Exception err)
                        {
                            using CommandMessage msg = err.InnerException != null
                                                     ? new CommandMessage(err.InnerException.Message, CommandState.ERROR)
                                                     : err.Message == "Stack empty."
                                                         ? new CommandMessage(CompilerMessage.PARANTHESIS_COUNT_ERROR, CommandState.ERROR)
                                                         : new CommandMessage(err.Message, CommandState.ERROR);

                            HttpContext.Session.SetLastMsg(
                                                            SessionLastMessage,
                                                            msg
                                                          );
                        }

                        cmdlis.Add(cmd);
                        HttpContext.Session.SetCmdList(SessionOutputHistory, cmdlis);
                        cmdlis.Clear();
                    }
                    else
                    {
                        using CommandMessage msg = new CommandMessage("", CommandState.IDLE);
                        HttpContext.Session.SetLastMsg(
                                                        SessionLastMessage,
                                                        msg
                                                      );
                    }
                }
                else
                {
                    using CommandMessage msg = new CommandMessage(RequestMessage.REQUEST_MISSING_KEYS("Komut yollama", reqdict[CommandParam]), CommandState.ERROR);
                    HttpContext.Session.SetLastMsg(
                                                    SessionLastMessage,
                                                    msg
                                                  );
                }

                HttpContext.Session.Set(SessionLastOutputDate, DateTime.Now);

                reqdict.Clear();

            }
            else
            {
                using CommandMessage msg = new CommandMessage(RequestMessage.REQUEST_SPAM(HttpContext.Session.Get<DateTime>(SessionLastOutputDate)), CommandState.WARNING);
                HttpContext.Session.SetLastMsg(
                                                SessionLastMessage,
                                                msg
                                               );
            }

            HttpContext.Session.Set(SessionLastCommandDate, LastCmdDate);
        }

        /// <summary>
        /// Create a description matrix with given request
        /// </summary>
        /// <returns>An <see cref="OkObjectResult"/> with description dataframe of the requested dataframe</returns>
        public async Task<OkObjectResult> OnPostCreateDescription()
        {
            Dictionary<string, string> reqdict = new Dictionary<string, string>();
            await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, reqdict).ConfigureAwait(false);
            if (reqdict.ContainsKey(DfNameParam)
                && reqdict.ContainsKey(DfColIndexParam))
            {
                List<object> colinds = new List<object>(reqdict[DfColIndexParam].Split(",", StringSplitOptions.RemoveEmptyEntries));
                if (colinds.Count == 0)
                {
                    using CommandMessage msg = new CommandMessage(RequestMessage.REQUEST_PARAM_INVALID("Sütun isimleri", reqdict[DfColIndexParam]), CommandState.WARNING);
                    HttpContext.Session.SetLastMsg(
                                                    SessionLastMessage,
                                                    msg
                                                  );
                    return new OkObjectResult(new Dictionary<string, dynamic>()
                    {
                        {"null", new List<object>() },
                        {"__Keys", new List<string>() }
                    });
                }

                try
                {
                    Dictionary<string, dynamic> response = new Dictionary<string, dynamic>();
                    Dictionary<string, Dataframe> _dict = HttpContext.Session.GetDfDict(SessionDfDict, SessionDfLabels, SessionDfSettings);

                    // CREATE TEMPORARY DATAFRAME FROM COLUMN INDICES
                    for (int i = 0; i < colinds.Count; i++)
                    {
                        colinds[i] = int.Parse((string)colinds[i]);
                    }
                    List<LabelList> newcollbls = new List<LabelList>() { new LabelList() };
                    List<Label> labels = _dict[reqdict[DfNameParam]].GetColLabels() == null
                                        ? new LabelList(_dict[reqdict[DfNameParam]].Col).Labels
                                        : _dict[reqdict[DfNameParam]].GetColLabels()[0].Labels;

                    MatrisBase<object> sub = _dict[reqdict[DfNameParam]].ColMat((int)colinds[0], 1);
                    newcollbls[0].Labels.Add(labels[(int)colinds[0] - 1].Copy());
                    for (int j = 1; j < colinds.Count; j++)
                    {
                        newcollbls[0].Labels.Add(labels[(int)colinds[j] - 1].Copy());
                        sub = _matService.Concatenate(sub, _dict[reqdict[DfNameParam]].ColMat((int)colinds[j], 1), 1);
                    }
                    sub = new Dataframe(sub.GetValues(), colLabels: newcollbls);

                    // DESCRIPTION
                    using Dataframe desc = (Dataframe)_matService.Replace(
                        _matService.Replace(
                            _matService.Replace(
                                _statService.Describe(sub), float.NaN, null),
                                float.PositiveInfinity,
                                "inf"),
                        float.NegativeInfinity,
                        "-inf");

                    List<string> keys = new List<string>();
                    for (int c = 0; c < desc.Col; c++)
                    {
                        response.Add(desc.GetColLabels()[0].Labels[c].Value, desc.ColList(c, 0));
                    }
                    for (int r = 0; r < desc.Row; r++)
                    {
                        keys.Add(desc.GetRowLabels()[0].Labels[r].Value);
                    }
                    response.Add("__Keys", keys);

                    // DISPOSE
                    _utils.DisposeDfDicts(_dict, null);
                    sub.Dispose();
                    reqdict.Clear();

                    return new OkObjectResult(response);
                }
                catch (Exception err)
                {
                    using CommandMessage msg = new CommandMessage(err.Message, CommandState.ERROR);
                    HttpContext.Session.SetLastMsg(
                                                    SessionLastMessage,
                                                    msg
                                                  );
                    return new OkObjectResult(new Dictionary<string, dynamic>()
                    {
                        {"null", new List<object>() },
                        {"__Keys", new List<string>() }
                    });
                }
            }
            else
            {
                using CommandMessage msg = new CommandMessage(RequestMessage.REQUEST_MISSING_KEYS("Komut yollama", reqdict[CommandParam]), CommandState.ERROR);
                HttpContext.Session.SetLastMsg(
                                                SessionLastMessage,
                                                msg
                                              );

                return new OkObjectResult(new Dictionary<string, dynamic>()
                    {
                        {"null", new List<object>() },
                        {"__Keys", new List<string>() }
                    });
            }
        }
        #endregion

        #region POST Action Partials
        /// <summary>
        /// Dataframe table partial view re-rendering
        /// </summary>
        /// <returns>Partial view result of the matrix table</returns>
        public PartialViewResult OnPostUpdateDataframeTable()
        {
            PartialViewResult mpart = Partial("_VeriTablePartial", HttpContext.Session.GetDfDict(SessionDfDict, SessionDfLabels, SessionDfSettings));

            return mpart;
        }

        /// <summary>
        /// Matrix table partial view re-rendering
        /// </summary>
        /// <returns>Partial view result of the matrix table</returns>
        public PartialViewResult OnPostUpdateMatrisTable()
        {
            PartialViewResult mpart = Partial("_MatrisTablePartial", HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict));
            mpart.ViewData["headcls"] = "df_matris_table df_matris_table_head";
            mpart.ViewData["headtdcls"] = "df_saved_mattable_title";
            mpart.ViewData["bodyclass"] = "df_matris_table";
            return mpart;
        }

        /// <summary>
        /// Command history panel partial view re-rendering
        /// </summary>
        /// <returns>Partial view result of the command and output history panel</returns>
        public PartialViewResult OnPostUpdateHistoryPanel()
        {
            PartialViewResult mpart =
                Partial(
                            "_dfOutputPanelPartial",
                            new Dictionary<string, dynamic>()
                            {
                                { CommandHistoryKey , HttpContext.Session.GetCmdList(SessionOutputHistory) },
                                { LastMessageKey , HttpContext.Session.GetLastMsg(SessionLastMessage) }
                            }
                        );

            return mpart;
        }

        #endregion

    }
}
