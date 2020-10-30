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

        public Command LastExecutedCommand;

        public string LastMessage;

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


                List<CommandLabel> cmdLabelList = _frontService.GetCommandLabelList();
                if (cmdLabelList == null)
                {
                    _frontService.ReadCommandInformation();
                    cmdLabelList = _frontService.GetCommandLabelList();
                }

                bool found = false;

                foreach(CommandLabel lbl in cmdLabelList)
                {
                    if (found)
                        break;
                    foreach (CommandInfo cmdinfo in lbl.Functions)
                    {
                        if (cmdinfo.function == actualFuncName)
                        {
                            try
                            {
                                _frontService.AddToMatrisDict(DecodedRequestDict["name"],
                                    _utils.SpecialStringTo2DList(DecodedRequestDict["args"], cmdinfo, savedMatrices),
                                    savedMatrices);
                                found = true;
                                break;
                            }
                            catch(Exception err)
                            {
                                LastMessage = err.Message;
                            }
                        }
                    }
                }

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
                LastExecutedCommand = _frontService.CreateCommand(DecodedRequestDict["cmd"]);
            }

            SetSessionVariables();
        }

        // Session değişkenlerini al, istek işleme fonksiyonlarının başında kullanılmalı
        private void GetSessionVariables()
        {
            // Matrix dictionaries
            if (HttpContext.Session.Get<Dictionary<string, List<List<dynamic>>>>(SessionMatrisDict) != null)
            {
                savedMatricesValDict = HttpContext.Session.Get<Dictionary<string, List<List<dynamic>>>>(SessionMatrisDict);

                // TO-DO: Eliminate this process
                savedMatrices = new Dictionary<string, MatrisBase<dynamic>>();
                foreach(string name in savedMatricesValDict.Keys)
                {
                    savedMatrices.Add(name, new MatrisBase<dynamic>(savedMatricesValDict[name]));
                }
                //
            }

            // Last Command
            if (HttpContext.Session.Get<Command>(SessionLastCommand) != null)
            {
                LastExecutedCommand = HttpContext.Session.Get<Command>(SessionLastCommand);
            }

            // Last Message
            if (HttpContext.Session.Get<string>(SessionLastMessage) != null)
            {
                LastMessage = HttpContext.Session.Get<string>(SessionLastMessage);
            }
        }

        // Session değişkenlerini güncelle, istek işleme fonksiyonlarının sonunda kullanılmalı
        private void SetSessionVariables()
        {
            HttpContext.Session.Set<Command>(SessionLastCommand, LastExecutedCommand);

            HttpContext.Session.Set<string>(SessionLastMessage, LastMessage);

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
