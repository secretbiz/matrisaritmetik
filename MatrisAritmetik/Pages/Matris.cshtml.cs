﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MatrisAritmetik.Pages
{
    /// <summary>
    /// Page for matrix related operations
    /// </summary>
    public class MatrisModel : PageModel
    {
        #region Session Variable Names
        private const string SessionMatrisDict = "_MatDictVals";
        private const string SessionSeedDict = "_MatDictSeed";
        private const string SessionLastCommand = "_LastCmd";
        private const string SessionLastMessage = "_lastMsg";
        private const string SessionOutputHistory = "_outputHistory";
        private const string SessionLastCommandDate = "_lastCmdDate";
        private const string SessionLastOutputDate = "_lastOutDate";
        #endregion

        #region Expected Request Parameter Names
        private const string CommandParam = "cmd";
        private const string MatrisNameParam = "name";
        private const string MatrisValsParam = "vals";
        private const string MatrisDelimParam = "delimiter";
        private const string MatrisNewLineParam = "newline";
        private const string MatrisSpecialFuncParam = "func";
        private const string MatrisSpecialArgsParam = "args";
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
        /// Matrix arithmetic methods
        /// </summary>
        private readonly IMatrisArithmeticService<dynamic> _matrisService;
        /// <summary>
        /// Special matrix creating methods
        /// </summary>
        private readonly ISpecialMatricesService _specialMatricesService;
        #endregion

        #region Page Constructor
        public MatrisModel(ILogger<MatrisModel> logger,
                           IUtilityService<dynamic> utilityService,
                           IMatrisArithmeticService<dynamic> matrisService,
                           IFrontService frontService,
                           ISpecialMatricesService specialMatricesService)
        {
            _logger = logger;
            _utils = utilityService;
            _frontService = frontService;
            _matrisService = matrisService;
            _specialMatricesService = specialMatricesService;
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
        private readonly List<string> SpecialsLabels = new List<string>() { "Özel Matris" };
        /// <summary>
        /// Last time a command was sent
        /// </summary>
        public DateTime LastCmdDate = DateTime.Now;
        /// <summary>
        /// Last time a command was executed and an output was generated
        /// </summary>
        public DateTime LastOutDate;
        #endregion

        #region GET Actions
        /// <summary>
        /// Default GET action
        /// </summary>
        public void OnGet()
        {
            if (_frontService.GetCommandLabelList() == null)
            {
                _frontService.ReadCommandInformation();
            }

            ViewData["komut_optionsdict"] = _frontService.GetCommandLabelList();

            ViewData["special_opiondict"] = _frontService.GetCommandLabelList(SpecialsLabels);

            ViewData["matrix_dict"] = HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict);

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
            Console.WriteLine("OnPost called, resetting session variables...");
        }

        /// <summary>
        /// Create and save a matrix with given name and values from text
        /// </summary>
        public async Task OnPostAddMatrix()
        {
            if (_frontService.CheckCmdDate(HttpContext.Session.Get<DateTime>(SessionLastCommandDate)))
            {
                LastCmdDate = DateTime.Now;

                Dictionary<string, string> reqdict = new Dictionary<string, string>();
                await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, reqdict);

                if (reqdict.ContainsKey(MatrisNameParam)
                    && reqdict.ContainsKey(MatrisValsParam)
                    && reqdict.ContainsKey(MatrisDelimParam)
                    && reqdict.ContainsKey(MatrisNewLineParam)
                   )
                {

                    Dictionary<string, MatrisBase<dynamic>> _dict = HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict);

                    if (!_dict.ContainsKey(reqdict[MatrisNameParam]))
                    {
                        try
                        {
                            Validations.ValidMatrixName(reqdict[MatrisNameParam], throwOnBadName: true);

                            Dictionary<string, List<List<object>>> vals = HttpContext.Session.GetMatVals(SessionMatrisDict);
                            Dictionary<string, Dictionary<string, dynamic>> opts = HttpContext.Session.GetMatOptions(SessionSeedDict);

                            reqdict[MatrisDelimParam] = _utils.FixLiterals(reqdict[MatrisDelimParam]);
                            reqdict[MatrisNewLineParam] = _utils.FixLiterals(reqdict[MatrisNewLineParam]);
                            reqdict[MatrisValsParam] = _utils.FixLiterals(reqdict[MatrisValsParam]);

                            _frontService.AddToMatrisDict
                            (
                                reqdict[MatrisNameParam],
                                new MatrisBase<dynamic>
                                (
                                    _utils.StringTo2DList
                                    (
                                        reqdict[MatrisValsParam],
                                        reqdict[MatrisDelimParam],
                                        reqdict[MatrisNewLineParam]
                                    )
                                ),
                                _dict
                            );

                            _frontService.SetMatrixDicts(_dict, vals, opts);

                            HttpContext.Session.Set(SessionMatrisDict, vals);

                            HttpContext.Session.SetMatOptions(SessionSeedDict, opts);

                            HttpContext.Session.SetLastMsg(
                                                            SessionLastMessage,
                                                            new CommandMessage(CompilerMessage.SAVED_MATRIX(reqdict[MatrisNameParam]), CommandState.SUCCESS)
                                                          );

                            vals.Clear();
                            opts.Clear();
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
                        HttpContext.Session.SetLastMsg(
                                                        SessionLastMessage,
                                                        new CommandMessage(CompilerMessage.MAT_NAME_ALREADY_EXISTS(reqdict[MatrisNameParam]), CommandState.WARNING)
                                                      );
                    }

                    _dict.Clear();
                }
                else
                {
                    HttpContext.Session.SetLastMsg(
                                                    SessionLastMessage,
                                                    new CommandMessage(RequestMessage.REQUEST_MISSING_KEYS("AddMatrix", new string[2] { MatrisNameParam, MatrisValsParam }), CommandState.ERROR)
                                                  );
                }

                HttpContext.Session.Set(SessionLastCommandDate, LastCmdDate);
            }
            else
            {
                HttpContext.Session.SetLastMsg(
                                                SessionLastMessage,
                                                new CommandMessage(RequestMessage.REQUEST_SPAM(HttpContext.Session.Get<DateTime>(SessionLastCommandDate)), CommandState.WARNING)
                                              );
            }

        }

        /// <summary>
        /// Create and save a matrix from given special class with given arguments and name
        /// </summary>
        public async Task OnPostAddMatrixSpecial()
        {
            if (_frontService.CheckCmdDate(HttpContext.Session.Get<DateTime>(SessionLastCommandDate)))
            {
                LastCmdDate = DateTime.Now;

                Dictionary<string, string> reqdict = new Dictionary<string, string>();
                await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, reqdict);

                if (reqdict.ContainsKey(MatrisNameParam) && reqdict.ContainsKey(MatrisSpecialFuncParam) && reqdict.ContainsKey(MatrisSpecialArgsParam))
                {
                    Dictionary<string, MatrisBase<dynamic>> _dict = HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict);

                    if (_dict.ContainsKey(reqdict[MatrisNameParam]))
                    {
                        HttpContext.Session.Set(SessionLastCommandDate, LastCmdDate);
                        HttpContext.Session.SetLastMsg(
                                                        SessionLastMessage,
                                                        new CommandMessage(CompilerMessage.MAT_NAME_ALREADY_EXISTS(reqdict[MatrisNameParam]), CommandState.WARNING)
                                                      );
                        return;
                    }

                    string actualFuncName = reqdict[MatrisSpecialFuncParam][1..reqdict[MatrisSpecialFuncParam].IndexOf("(")];

                    if (actualFuncName == string.Empty)
                    {
                        HttpContext.Session.Set(SessionLastCommandDate, LastCmdDate);
                        HttpContext.Session.SetLastMsg(
                                                        SessionLastMessage,
                                                        new CommandMessage(CompilerMessage.NOT_A_(actualFuncName, "fonksiyon"), CommandState.ERROR)
                                                      );
                        return;
                    }

                    if (_frontService.TryParseBuiltFunc(actualFuncName, out CommandInfo cmdinfo))
                    {
                        try
                        {
                            Dictionary<string, List<List<object>>> vals = HttpContext.Session.Get<Dictionary<string, List<List<object>>>>(SessionMatrisDict);
                            Dictionary<string, Dictionary<string, dynamic>> opts = HttpContext.Session.GetMatOptions(SessionSeedDict);

                            Validations.ValidMatrixName(reqdict[MatrisNameParam], throwOnBadName: true);

                            _frontService.AddToMatrisDict
                            (
                                reqdict[MatrisNameParam],
                                _utils.SpecialStringTo2DList
                                (
                                    reqdict[MatrisSpecialArgsParam],
                                    cmdinfo,
                                    _dict
                                ),
                                _dict
                            );

                            _frontService.SetMatrixDicts(_dict, vals, opts);

                            HttpContext.Session.Set(SessionMatrisDict, vals);

                            HttpContext.Session.SetMatOptions(SessionSeedDict, opts);

                            HttpContext.Session.SetLastMsg(
                                                            SessionLastMessage,
                                                            new CommandMessage(CompilerMessage.SAVED_MATRIX(reqdict[MatrisNameParam]), CommandState.SUCCESS)
                                                          );
                            vals.Clear();
                            opts.Clear();
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

                        _dict.Clear();
                    }
                    else
                    {
                        HttpContext.Session.SetLastMsg(
                                                        SessionLastMessage,
                                                        new CommandMessage(CompilerMessage.NOT_A_(actualFuncName, "fonksiyon"), CommandState.ERROR)
                                                      );
                    }
                }
                else
                {
                    HttpContext.Session.SetLastMsg(
                                                    SessionLastMessage,
                                                    new CommandMessage(RequestMessage.REQUEST_MISSING_KEYS(
                                                        "AddMatrixSpecial",
                                                        new string[3] { MatrisNameParam, MatrisSpecialFuncParam, MatrisSpecialArgsParam }),
                                                        CommandState.ERROR)
                                                  );
                }

                HttpContext.Session.Set(SessionLastCommandDate, LastCmdDate);
            }
            else
            {

                HttpContext.Session.SetLastMsg(
                                                SessionLastMessage,
                                                new CommandMessage(RequestMessage.REQUEST_SPAM(HttpContext.Session.Get<DateTime>(SessionLastCommandDate)), CommandState.WARNING)
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
                LastCmdDate = DateTime.Now;

                Dictionary<string, string> FileData = new Dictionary<string, string>();

                await _utils.ReadFileFromRequest(Request.Body, Encoding.Default, FileData);

                Dictionary<string, MatrisBase<dynamic>> _dict = HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict);

                if (!_dict.ContainsKey(FileData["name"]))
                {
                    try
                    {
                        Validations.ValidMatrixName(FileData["name"], throwOnBadName: true);

                        _frontService.AddToMatrisDict
                        (
                            FileData["name"],
                            new MatrisBase<dynamic>
                            (
                                _utils.StringTo2DList
                                (
                                    FileData["data"],
                                    FileData["delim"],
                                    FileData["newline"]
                                )
                            ),
                            _dict
                        );

                        Dictionary<string, List<List<object>>> vals = HttpContext.Session.Get<Dictionary<string, List<List<object>>>>(SessionMatrisDict);
                        Dictionary<string, Dictionary<string, dynamic>> opts = HttpContext.Session.GetMatOptions(SessionSeedDict);

                        _frontService.SetMatrixDicts(_dict, vals, opts);

                        HttpContext.Session.Set(SessionMatrisDict, vals);

                        HttpContext.Session.SetMatOptions(SessionSeedDict, opts);

                        HttpContext.Session.SetLastMsg(
                                                        SessionLastMessage,
                                                        new CommandMessage(CompilerMessage.SAVED_MATRIX(FileData["name"]), CommandState.SUCCESS)
                                                      );

                        vals.Clear();
                        opts.Clear();
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
                    HttpContext.Session.SetLastMsg(
                                                    SessionLastMessage,
                                                    new CommandMessage(CompilerMessage.MAT_NAME_ALREADY_EXISTS(FileData["name"]), CommandState.WARNING)
                                                  );
                }

                _dict.Clear();
                FileData.Clear();
            }
            else
            {
                HttpContext.Session.SetLastMsg(
                                                SessionLastMessage,
                                                new CommandMessage(RequestMessage.REQUEST_SPAM(HttpContext.Session.Get<DateTime>(SessionLastCommandDate)), CommandState.WARNING)
                                              );
            }

            HttpContext.Session.Set(SessionLastCommandDate, LastCmdDate);
        }

        /// <summary>
        /// Remove a matrix from the table
        /// </summary>
        public async Task OnPostDeleteMatrix()
        {
            Dictionary<string, string> reqdict = new Dictionary<string, string>();

            await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, reqdict);

            if (reqdict.ContainsKey(MatrisNameParam))
            {
                reqdict[MatrisNameParam] = reqdict[MatrisNameParam].Replace("matris_table_delbutton_", "");

                Dictionary<string, MatrisBase<dynamic>> _dict = HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict);
                Dictionary<string, List<List<object>>> vals = HttpContext.Session.GetMatVals(SessionMatrisDict);
                Dictionary<string, Dictionary<string, dynamic>> opts = HttpContext.Session.GetMatOptions(SessionSeedDict);

                if (_frontService.DeleteFromMatrisDict(reqdict[MatrisNameParam], _dict))
                {
                    vals.Remove(reqdict[MatrisNameParam]);
                    opts.Remove(reqdict[MatrisNameParam]);
                }

                // Uncomment this if deleting matrices are made possible through compiler
                //_frontService.SetMatrixDicts(_dict, vals, opts);

                HttpContext.Session.Set(SessionMatrisDict, vals);
                HttpContext.Session.SetMatOptions(SessionSeedDict, opts);

                HttpContext.Session.SetLastMsg(
                                                SessionLastMessage,
                                                new CommandMessage(CompilerMessage.DELETED_MATRIX(reqdict[MatrisNameParam]), CommandState.SUCCESS)
                                              );

                _dict.Clear();
                vals.Clear();
                opts.Clear();
            }

            reqdict.Clear();
        }

        /// <summary>
        /// Create and evaluate a command
        /// </summary>
        public async Task OnPostSendCmd()
        {
            LastCmdDate = DateTime.Now;

            if (_frontService.CheckCmdDate(HttpContext.Session.Get<DateTime>(SessionLastOutputDate)))
            {
                Dictionary<string, string> reqdict = new Dictionary<string, string>();
                await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, reqdict);

                if (reqdict.ContainsKey(CommandParam))
                {
                    if (reqdict[CommandParam].Trim() != "")
                    {
                        List<Command> cmdlis = HttpContext.Session.GetCmdList(SessionOutputHistory);
                        using Command cmd = _frontService.CreateCommand(reqdict[CommandParam]);

                        try
                        {
                            Dictionary<string, MatrisBase<dynamic>> _dict = HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict);
                            Dictionary<string, List<List<object>>> vals = HttpContext.Session.GetMatVals(SessionMatrisDict);
                            Dictionary<string, Dictionary<string, dynamic>> opts = HttpContext.Session.GetMatOptions(SessionSeedDict);


                            cmd.Tokens = _frontService.ShuntingYardAlg(_frontService.Tokenize(cmd.TermsToEvaluate[0]));

                            _frontService.EvaluateCommand(
                                                            cmd,
                                                            _dict,
                                                            cmdlis
                                                         );

                            HttpContext.Session.SetLastMsg(
                                                            SessionLastMessage,
                                                            new CommandMessage(cmd.STATE_MESSAGE, cmd.STATE)
                                                          );

                            _frontService.SetMatrixDicts(_dict, vals, opts);

                            HttpContext.Session.Set(SessionMatrisDict, vals);

                            HttpContext.Session.SetMatOptions(SessionSeedDict, opts);

                            _dict.Clear();
                            vals.Clear();
                            opts.Clear();
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
                        HttpContext.Session.SetLastMsg(
                                                        SessionLastMessage,
                                                        new CommandMessage("", CommandState.IDLE)
                                                      );
                    }
                }
                else
                {
                    HttpContext.Session.SetLastMsg(
                                                    SessionLastMessage,
                                                    new CommandMessage("", CommandState.IDLE)
                                                  );
                }
                LastOutDate = DateTime.Now;
                HttpContext.Session.Set(SessionLastOutputDate, LastOutDate);

                reqdict.Clear();

            }
            else
            {
                HttpContext.Session.SetLastMsg(
                                                SessionLastMessage,
                                                new CommandMessage(RequestMessage.REQUEST_SPAM(HttpContext.Session.Get<DateTime>(SessionLastOutputDate)), CommandState.WARNING)
                                               );
            }

            HttpContext.Session.Set(SessionLastCommandDate, LastCmdDate);
        }

        #endregion

        #region POST Action Partials
        /// <summary>
        /// Matrix table partial view re-rendering
        /// </summary>
        /// <returns>Partial view result of the matrix table</returns>
        public PartialViewResult OnPostUpdateMatrisTable()
        {
            PartialViewResult mpart = Partial("_MatrisTablePartial", HttpContext.Session.GetMatrixDict(SessionMatrisDict, SessionSeedDict));

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
                            "_OutputPanelPartial",
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
