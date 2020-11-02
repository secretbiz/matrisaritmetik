using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MatrisAritmetik.Core;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MatrisAritmetik.Core.Services;
using Microsoft.AspNetCore.Authorization;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Pages
{   
    public class MatrisModel : PageModel
    {
        public const string SessionMatrisDict = "_MatDictVals";
        public const string SessionLastCommand = "_LastCmd";
        public const string SessionLastMessage = "_lastMsg";
        public const string SessionOutputHistory = "_outputHistory";

        private readonly IUtilityService<dynamic> _utils;                   // string işleme fonksiyonları
        private readonly IMatrisArithmeticService<dynamic> _matrisService;  // matris fonksiyonları
        private readonly IFrontService _frontService;                     // önyüzde gösterilecek bilgi ile ilgili fonksiyonlar
        private readonly ISpecialMatricesService _specialMatricesService; // özel matris oluşturma fonksiyonları

        // Page constructor
        public MatrisModel(IUtilityService<dynamic> utilityService,
                           IMatrisArithmeticService<dynamic> matrisService,
                           IFrontService frontService,
                           ISpecialMatricesService specialMatricesService)
        {
            _utils = utilityService;
            _frontService = frontService;
            _matrisService = matrisService;
            _specialMatricesService = specialMatricesService;
        }

        // Anlık erişim için değişkenler
        public Dictionary<string, MatrisBase<dynamic>> savedMatrices = new Dictionary<string, MatrisBase<dynamic>>();

        public Dictionary<string, List<List<dynamic>>> savedMatricesValDict = new Dictionary<string, List<List<dynamic>>>();

        private readonly List<string> IgnoredParams = new List<string>() { "__RequestVerificationToken" };

        private readonly List<string> SpecialsLabels = new List<string>() { "Genel", "Rastgele" };

        public Dictionary<string, string> DecodedRequestDict = new Dictionary<string, string>();

        public Command LastExecutedCommand = new Command("");

        public List<Command> CommandHistory = new List<Command>();

        public string LastMessage = "";

        public Dictionary<string, dynamic> OutputHistory = new Dictionary<string, dynamic>();

        // Sayfa kökü GET ile istendiğinde
        public void OnGet()
        {
            GetSessionVariables();

            if (_frontService.GetCommandLabelList() == null)
                _frontService.ReadCommandInformation();

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

            if (DecodedRequestDict.ContainsKey("name") && DecodedRequestDict.ContainsKey("vals"))
            {
                if(!savedMatrices.ContainsKey(DecodedRequestDict["name"]))
                    _frontService.AddToMatrisDict(DecodedRequestDict["name"],
                        new MatrisBase<dynamic>(_utils.StringTo2DList(DecodedRequestDict["vals"])),
                        savedMatrices);

            }

            else if (DecodedRequestDict.ContainsKey("name") && DecodedRequestDict.ContainsKey("func") && DecodedRequestDict.ContainsKey("args"))
            {

                if (savedMatrices.ContainsKey(DecodedRequestDict["name"]))
                    return;

                string actualFuncName = DecodedRequestDict["func"].Substring(1, DecodedRequestDict["func"].IndexOf("(")-1);
                
                if (actualFuncName == string.Empty)
                    return;

                if(_frontService.TryParseBuiltFunc(actualFuncName,out CommandInfo cmdinfo))
                {
                    try
                    {
                        _frontService.AddToMatrisDict(DecodedRequestDict["name"],
                            _utils.SpecialStringTo2DList(DecodedRequestDict["args"], cmdinfo, savedMatrices),
                            savedMatrices);
                    }
                    catch(Exception err)
                    {
                        LastMessage = err.Message;
                    }
                }
                else
                    LastMessage = "Fonksiyon adı hatalı";
            }

            SetSessionVariables();
        }

        // Matris tablosundan matris silme isteği
        public async Task OnPostDeleteMatrix()
        {
            GetSessionVariables();

            await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, DecodedRequestDict);

            if (DecodedRequestDict.ContainsKey("name"))
            {
                _frontService.DeleteFromMatrisDict(DecodedRequestDict["name"].Replace("matris_table_delbutton_", ""), savedMatrices);
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
                    LastExecutedCommand = _frontService.CreateCommand(DecodedRequestDict["cmd"]);

                    LastExecutedCommand.Tokens = _frontService.ShuntingYardAlg(_frontService.Tokenize(LastExecutedCommand.TermsToEvaluate[0]));

                    _frontService.EvaluateCommand(LastExecutedCommand, savedMatrices);

                    LastMessage = LastExecutedCommand.STATE_MESSAGE;
                }
                catch(Exception err)
                {
                    if (err.Message == "Stack empty.")
                        LastMessage = "Format hatası: Parantezler uyuşmalı.";
                    else
                        LastMessage = "Format hatası: " + err.Message;
                }

                if(OutputHistory == null)
                    OutputHistory = new Dictionary<string, dynamic>();

                if(OutputHistory.ContainsKey("CommandHistory"))
                    OutputHistory["CommandHistory"].Add(LastExecutedCommand);
                else
                    OutputHistory.Add("CommandHistory", new List<Command>() { LastExecutedCommand });

                if (OutputHistory.ContainsKey("LastMessage"))
                    OutputHistory["LastMessage"] = LastMessage;
                else
                    OutputHistory.Add("LastMessage", LastMessage);
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

                // TO-DO: Eliminate this process
                savedMatrices = new Dictionary<string, MatrisBase<object>>();
                foreach(string name in savedMatricesValDict.Keys)
                {
                    savedMatrices.Add(name, new MatrisBase<object>(savedMatricesValDict[name]));
                }
                //
            }

            // Last Command
            if (HttpContext.Session.Get<string>(SessionLastCommand) != null)
            {
                LastExecutedCommand = new Command(HttpContext.Session.Get<string>(SessionLastCommand));
            }

            // Last Message
            if (HttpContext.Session.Get<string>(SessionLastMessage) != null)
            {
                LastMessage = HttpContext.Session.Get<string>(SessionLastMessage);
            }

            // Command and output history
            if (HttpContext.Session.GetCmdList(SessionOutputHistory) != null)
            {
                CommandHistory = HttpContext.Session.GetCmdList(SessionOutputHistory);
            }

            OutputHistory.Add("CommandHistory", CommandHistory);
            OutputHistory.Add("LastMessage", LastMessage);
        }

        // Session değişkenlerini güncelle, istek işleme fonksiyonlarının sonunda kullanılmalı
        private void SetSessionVariables()
        {
            HttpContext.Session.Set<string>(SessionLastCommand, LastExecutedCommand.OriginalCommand);

            HttpContext.Session.Set<string>(SessionLastMessage, LastMessage);

            HttpContext.Session.SetCmdList(SessionOutputHistory,CommandHistory);

            // TO-DO: Eliminate this process
            savedMatricesValDict = new Dictionary<string, List<List<dynamic>>>();
            foreach (string name in savedMatrices.Keys)
            {
                savedMatricesValDict.Add(name, savedMatrices[name].values);
            }
            //

            HttpContext.Session.Set<Dictionary<string, List<List<dynamic>>>>(SessionMatrisDict, savedMatricesValDict);
        }


    }
}
