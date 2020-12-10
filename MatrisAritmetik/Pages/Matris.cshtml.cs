using System;
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
        #endregion

        #region Expected Request Parameter Names
        private const string MatrisNameParam = "name";
        private const string MatrisValsParam = "vals";
        private const string MatrisSpecialFuncParam = "func";
        private const string MatrisSpecialArgsParam = "args";
        #endregion

        #region ViewData Keys
        private const string LastMessageKey = "LastMessage";
        private const string CommandHistoryKey = "CommandHistory";
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
        /// Saved matrices <see cref="MatrisBase{T}"/> dictionary
        /// </summary>
        public Dictionary<string, MatrisBase<dynamic>> savedMatrices = new Dictionary<string, MatrisBase<dynamic>>();
        /// <summary>
        /// Saved matrices' values dictionary
        /// </summary>
        public Dictionary<string, List<List<dynamic>>> savedMatricesValDict = new Dictionary<string, List<List<dynamic>>>();
        /// <summary>
        /// Saved matrices' options dictionary
        /// </summary>
        public Dictionary<string, Dictionary<string, dynamic>> savedMatricesOptionsDict = new Dictionary<string, Dictionary<string, dynamic>>();
        /// <summary>
        /// List of parameters that should be ignored while reading the request body
        /// </summary>
        private readonly List<string> IgnoredParams = new List<string>() { "__RequestVerificationToken" };
        /// <summary>
        /// List of special labels used for special matrices
        /// </summary>
        private readonly List<string> SpecialsLabels = new List<string>() { "Özel Matris" };
        /// <summary>
        /// Dictionary to store request parameters and values
        /// </summary>
        public Dictionary<string, string> DecodedRequestDict = new Dictionary<string, string>();
        /// <summary>
        /// Last processed command
        /// </summary>
        public Command LastExecutedCommand = new Command("");
        /// <summary>
        /// History of processed commands
        /// </summary>
        public List<Command> CommandHistory = new List<Command>();
        /// <summary>
        /// History of processed commands' outputs
        /// </summary>
        public Dictionary<string, dynamic> OutputHistory = new Dictionary<string, dynamic>();
        /// <summary>
        /// Last message from the last command processed
        /// </summary>
        public CommandMessage LastMessage = new CommandMessage("", CommandState.IDLE);
        #endregion

        #region GET Actions
        /// <summary>
        /// Default GET action
        /// </summary>
        public void OnGet()
        {
            GetSessionVariables();

            if (_frontService.GetCommandLabelList() == null)
            {
                _frontService.ReadCommandInformation();
            }

            ViewData["komut_optionsdict"] = _frontService.GetCommandLabelList();

            ViewData["special_opiondict"] = _frontService.GetCommandLabelList(SpecialsLabels);

            SetSessionVariables();
        }
        #endregion

        #region POST Actions
        /// <summary>
        /// Default post action
        /// </summary>
        public void OnPost()
        {
            Console.WriteLine("OnPost called...");
        }

        /// <summary>
        /// Create and save a matrix with given name and values from text
        /// </summary>
        /// <returns></returns>
        public async Task OnPostAddMatrix()
        {
            GetSessionVariables();

            await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, DecodedRequestDict);

            if (DecodedRequestDict.ContainsKey(MatrisNameParam) && DecodedRequestDict.ContainsKey(MatrisValsParam))
            {
                if (!savedMatrices.ContainsKey(DecodedRequestDict[MatrisNameParam]))
                {
                    try
                    {
                        Validations.ValidMatrixName(DecodedRequestDict[MatrisNameParam], throwOnBadName: true);

                        _frontService.AddToMatrisDict(DecodedRequestDict[MatrisNameParam],
                        new MatrisBase<dynamic>(_utils.StringTo2DList(DecodedRequestDict[MatrisValsParam])),
                        savedMatrices);
                    }
                    catch (Exception err)
                    {
                        LastMessage = err.InnerException != null
                            ? new CommandMessage(err.InnerException.Message, CommandState.ERROR)
                            : new CommandMessage(err.Message, CommandState.ERROR);
                    }
                }
                else
                {
                    LastMessage = new CommandMessage(CompilerMessage.MAT_NAME_ALREADY_EXISTS(DecodedRequestDict[MatrisNameParam]), CommandState.WARNING);
                }
            }
            else
            {
                LastMessage = new CommandMessage(RequestMessage.REQUEST_MISSING_KEYS("AddMatrix", new string[2] { MatrisNameParam, MatrisValsParam }), CommandState.ERROR);
            }

            SetSessionVariables();
        }

        /// <summary>
        /// Create and save a matrix from given special class with given arguments and name
        /// </summary>
        /// <returns></returns>
        public async Task OnPostAddMatrixSpecial()
        {
            GetSessionVariables();

            await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, DecodedRequestDict);

            if (DecodedRequestDict.ContainsKey(MatrisNameParam) && DecodedRequestDict.ContainsKey(MatrisSpecialFuncParam) && DecodedRequestDict.ContainsKey(MatrisSpecialArgsParam))
            {

                if (savedMatrices.ContainsKey(DecodedRequestDict[MatrisNameParam]))
                {
                    LastMessage = new CommandMessage(CompilerMessage.MAT_NAME_ALREADY_EXISTS(DecodedRequestDict[MatrisNameParam]), CommandState.WARNING);
                    SetSessionVariables();
                    return;
                }

                string actualFuncName = DecodedRequestDict[MatrisSpecialFuncParam][1..DecodedRequestDict[MatrisSpecialFuncParam].IndexOf("(")];

                if (actualFuncName == string.Empty)
                {
                    LastMessage = new CommandMessage(CompilerMessage.NOT_A_(actualFuncName, "fonksiyon"), CommandState.ERROR);
                    SetSessionVariables();
                    return;
                }

                if (_frontService.TryParseBuiltFunc(actualFuncName, out CommandInfo cmdinfo))
                {
                    try
                    {
                        Validations.ValidMatrixName(DecodedRequestDict[MatrisNameParam], throwOnBadName: true);

                        _frontService.AddToMatrisDict(DecodedRequestDict[MatrisNameParam],
                            _utils.SpecialStringTo2DList(DecodedRequestDict[MatrisSpecialArgsParam], cmdinfo, savedMatrices),
                            savedMatrices);
                    }
                    catch (Exception err)
                    {
                        LastMessage = err.InnerException != null
                            ? new CommandMessage(err.InnerException.Message, CommandState.ERROR)
                            : new CommandMessage(err.Message, CommandState.ERROR);
                    }
                }
                else
                {
                    LastMessage = new CommandMessage("Fonksiyon adı hatalı", CommandState.ERROR);
                }
            }
            else
            {
                LastMessage = new CommandMessage(RequestMessage.REQUEST_MISSING_KEYS("AddMatrixSpecial", new string[3] { MatrisNameParam, MatrisSpecialFuncParam, MatrisSpecialArgsParam }), CommandState.ERROR);
            }
            SetSessionVariables();
        }

        /// <summary>
        /// Remove a matrix from the table
        /// </summary>
        /// <returns></returns>
        public async Task OnPostDeleteMatrix()
        {
            GetSessionVariables();

            await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, DecodedRequestDict);

            if (DecodedRequestDict.ContainsKey(MatrisNameParam))
            {
                _frontService.DeleteFromMatrisDict(DecodedRequestDict[MatrisNameParam].Replace("matris_table_delbutton_", ""), savedMatrices);
            }

            SetSessionVariables();
        }

        /// <summary>
        /// Create and evaluate a command
        /// </summary>
        /// <returns></returns>
        public async Task OnPostSendCmd()
        {
            GetSessionVariables();

            await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, DecodedRequestDict);

            if (DecodedRequestDict.ContainsKey("cmd"))
            {
                try
                {
                    if (DecodedRequestDict["cmd"].Trim() == "")
                    {
                        return;
                    }

                    LastExecutedCommand = _frontService.CreateCommand(DecodedRequestDict["cmd"]);

                    LastExecutedCommand.Tokens = _frontService.ShuntingYardAlg(_frontService.Tokenize(LastExecutedCommand.TermsToEvaluate[0]));

                    _frontService.EvaluateCommand(LastExecutedCommand, savedMatrices, CommandHistory);

                    LastMessage = new CommandMessage(LastExecutedCommand.STATE_MESSAGE, LastExecutedCommand.STATE);
                }
                catch (Exception err)
                {
                    LastMessage = err.InnerException != null
                        ? new CommandMessage(err.InnerException.Message, CommandState.ERROR)
                        : err.Message == "Stack empty."
                            ? new CommandMessage(CompilerMessage.PARANTHESIS_COUNT_ERROR, CommandState.ERROR)
                            : new CommandMessage(err.Message, CommandState.ERROR);
                }

                if (OutputHistory == null)
                {
                    OutputHistory = new Dictionary<string, dynamic>();
                }

                if (OutputHistory.ContainsKey(CommandHistoryKey))
                {
                    OutputHistory[CommandHistoryKey].Add(LastExecutedCommand);
                }
                else
                {
                    OutputHistory.Add(CommandHistoryKey, new List<Command>() { LastExecutedCommand });
                }

                if (OutputHistory.ContainsKey(LastMessageKey))
                {
                    OutputHistory[LastMessageKey] = LastMessage;
                }
                else
                {
                    OutputHistory.Add(LastMessageKey, LastMessage);
                }
            }

            SetSessionVariables();
        }
        #endregion

        #region POST Action Partials
        /// <summary>
        /// Matrix table partial view re-rendering
        /// </summary>
        /// <returns></returns>
        public PartialViewResult OnPostUpdateMatrisTable()
        {
            GetSessionVariables();

            PartialViewResult mpart = Partial("_MatrisTablePartial", savedMatrices);

            SetSessionVariables();

            return mpart;
        }

        /// <summary>
        /// Command history panel partial view re-rendering
        /// </summary>
        /// <returns></returns>
        public PartialViewResult OnPostUpdateHistoryPanel()
        {
            GetSessionVariables();

            PartialViewResult mpart = Partial("_OutputPanelPartial", OutputHistory);

            SetSessionVariables();

            return mpart;
        }
        #endregion

        #region Session Methods
        /// <summary>
        /// Gets the current session variables and puts them into temporary variables
        /// <para>Must be used at the beginning of the functions used by POST actions</para>
        /// </summary>
        private void GetSessionVariables()
        {
            // Matrix dictionaries
            if (HttpContext.Session.Get<Dictionary<string, List<List<object>>>>(SessionMatrisDict) != null)
            {
                savedMatricesValDict = HttpContext.Session.Get<Dictionary<string, List<List<object>>>>(SessionMatrisDict);
                savedMatricesOptionsDict = HttpContext.Session.GetMatOptions(SessionSeedDict);

                savedMatrices = new Dictionary<string, MatrisBase<object>>();
                foreach (string name in savedMatricesValDict.Keys)
                {
                    savedMatrices.Add(name, new MatrisBase<object>(savedMatricesValDict[name]));
                    savedMatrices[name].Seed = savedMatricesOptionsDict[name]["seed"];
                    savedMatrices[name].CreatedFromSeed = savedMatricesOptionsDict[name]["isRandom"];
                }
            }

            // Last Command
            if (HttpContext.Session.Get<string>(SessionLastCommand) != null)
            {
                LastExecutedCommand = new Command(HttpContext.Session.Get<string>(SessionLastCommand));
            }

            // Last Message
            if (HttpContext.Session.GetLastMsg(SessionLastMessage) != null)
            {
                LastMessage = HttpContext.Session.GetLastMsg(SessionLastMessage);
            }

            // Command and output history
            if (HttpContext.Session.GetCmdList(SessionOutputHistory) != null)
            {
                CommandHistory = HttpContext.Session.GetCmdList(SessionOutputHistory);
            }

            OutputHistory.Add(CommandHistoryKey, CommandHistory);
            OutputHistory.Add(LastMessageKey, LastMessage);
        }

        /// <summary>
        /// Sets the current temporary variables as session variables
        /// <para>Must be used at the end of the functions used by POST actions</para>
        /// </summary>
        private void SetSessionVariables()
        {
            HttpContext.Session.Set<string>(SessionLastCommand, LastExecutedCommand.OriginalCommand);
            LastExecutedCommand = null;

            HttpContext.Session.SetLastMsg(SessionLastMessage, LastMessage);
            LastMessage = null;

            HttpContext.Session.SetCmdList(SessionOutputHistory, CommandHistory);
            CommandHistory = null;

            savedMatricesValDict = new Dictionary<string, List<List<dynamic>>>();
            savedMatricesOptionsDict = new Dictionary<string, Dictionary<string, dynamic>>();
            foreach (string name in savedMatrices.Keys)
            {
                if (!savedMatricesValDict.ContainsKey(name))
                {
                    savedMatricesValDict.Add(name, savedMatrices[name].Values);
                }
                if (!savedMatricesOptionsDict.ContainsKey(name))
                {
                    savedMatricesOptionsDict.Add(name, new Dictionary<string, dynamic> {
                        { "seed",savedMatrices[name].Seed },
                        { "isRandom",savedMatrices[name].CreatedFromSeed} });
                }
            }
            savedMatrices = null;

            HttpContext.Session.Set<Dictionary<string, List<List<dynamic>>>>(SessionMatrisDict, savedMatricesValDict);
            savedMatricesValDict = null;

            HttpContext.Session.SetMatOptions(SessionSeedDict, savedMatricesOptionsDict);
            savedMatricesOptionsDict = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        #endregion

    }
}
