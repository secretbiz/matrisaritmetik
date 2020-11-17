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
    public class MatrisModel : PageModel
    {
        private const string SessionMatrisDict = "_MatDictVals";
        private const string SessionSeedDict = "_MatDictSeed";
        private const string SessionLastCommand = "_LastCmd";
        private const string SessionLastMessage = "_lastMsg";
        private const string SessionOutputHistory = "_outputHistory";

        private const string MatrisNameParam = "name";
        private const string MatrisValsParam = "vals";
        private const string MatrisSpecialFuncParam = "func";
        private const string MatrisSpecialArgsParam = "args";

        private const string LastMessageKey = "LastMessage";
        private const string CommandHistoryKey = "CommandHistory";

        private readonly ILogger<MatrisModel> _logger;
        private readonly IUtilityService<dynamic> _utils;                   // string işleme fonksiyonları
        private readonly IMatrisArithmeticService<dynamic> _matrisService;  // matris fonksiyonları
        private readonly IFrontService _frontService;                     // önyüzde gösterilecek bilgi ile ilgili fonksiyonlar
        private readonly ISpecialMatricesService _specialMatricesService; // özel matris oluşturma fonksiyonları

        // Page constructor
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

        // Anlık erişim için değişkenler
        public Dictionary<string, MatrisBase<dynamic>> savedMatrices = new Dictionary<string, MatrisBase<dynamic>>();

        public Dictionary<string, List<List<dynamic>>> savedMatricesValDict = new Dictionary<string, List<List<dynamic>>>();

        public Dictionary<string, Dictionary<string, dynamic>> savedMatricesOptionsDict = new Dictionary<string, Dictionary<string, dynamic>>();

        private readonly List<string> IgnoredParams = new List<string>() { "__RequestVerificationToken" };

        private readonly List<string> SpecialsLabels = new List<string>() { "Özel Matris" };

        public Dictionary<string, string> DecodedRequestDict = new Dictionary<string, string>();

        public Command LastExecutedCommand = new Command("");

        public List<Command> CommandHistory = new List<Command>();

        public CommandMessage LastMessage = new CommandMessage("", CommandState.IDLE);

        public Dictionary<string, dynamic> OutputHistory = new Dictionary<string, dynamic>();


        // Sayfa kökü GET ile istendiğinde
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

        // Sayfadaki bir form'dan POST isteği gönderildiğinde
        // Sayfayı refreshler
        public void OnPost()
        {
            string debug = ""; debug += "test"; // Buraya düşmemeli
        }

        // Matris tablosuna matris ekleme isteği
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
                        if (err.InnerException != null)
                        {
                            LastMessage = new CommandMessage(err.InnerException.Message, CommandState.ERROR);
                        }
                        else
                        {
                            LastMessage = new CommandMessage(err.Message, CommandState.ERROR);
                        }
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

        // Matris tablosuna özel matris ekleme isteği
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
                        if (err.InnerException != null)
                        {
                            LastMessage = new CommandMessage(err.InnerException.Message, CommandState.ERROR);
                        }
                        else
                        {
                            LastMessage = new CommandMessage(err.Message, CommandState.ERROR);
                        }
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

        // Matris tablosundan matris silme isteği
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

        // Matris tablosunu güncelleme, rerenderlama
        public PartialViewResult OnPostUpdateMatrisTable()
        {
            GetSessionVariables();

            PartialViewResult mpart = Partial("_MatrisTablePartial", savedMatrices);

            SetSessionVariables();

            return mpart;
        }

        // Komut gönderme isteği
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
                    if (err.InnerException != null)
                    {
                        LastMessage = new CommandMessage(err.InnerException.Message, CommandState.ERROR);
                    }
                    else if (err.Message == "Stack empty.")
                    {
                        LastMessage = new CommandMessage(CompilerMessage.PARANTHESIS_COUNT_ERROR, CommandState.ERROR);
                    }
                    else
                    {
                        LastMessage = new CommandMessage(err.Message, CommandState.ERROR);
                    }
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

        // Komut panelini güncelleme, rerenderlama
        public PartialViewResult OnPostUpdateHistoryPanel()
        {
            GetSessionVariables();

            PartialViewResult mpart = Partial("_OutputPanelPartial", OutputHistory);

            SetSessionVariables();

            return mpart;
        }

        // Session değişkenlerini al, istek işleme fonksiyonlarının başında kullanılmalı
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

        // Session değişkenlerini güncelle, istek işleme fonksiyonlarının sonunda kullanılmalı
        private void SetSessionVariables()
        {
            HttpContext.Session.Set<string>(SessionLastCommand, LastExecutedCommand.OriginalCommand);
            LastExecutedCommand = null;

            HttpContext.Session.SetLastMsg(SessionLastMessage, LastMessage);
            LastMessage = null;

            HttpContext.Session.SetCmdList(SessionOutputHistory, CommandHistory);
            CommandHistory = null;

            savedMatricesValDict = new Dictionary<string, List<List<dynamic>>>();
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


    }
}
