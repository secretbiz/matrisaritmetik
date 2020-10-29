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

        private readonly IUtilityService<float> _utils;
        private readonly IFrontService _frontService;
        private readonly IMatrisArithmeticService<float> _matrisService;

        public MatrisModel(IUtilityService<float> utilityService, IFrontService frontService,IMatrisArithmeticService<float> matrisService)
        {
            _utils = utilityService;
            _frontService = frontService;
            _matrisService = matrisService;
        }

        public Dictionary<string, MatrisBase<float>> savedMatrices = new Dictionary<string, MatrisBase<float>>();

        public Dictionary<string, List<List<float>>> savedMatricesValDict = new Dictionary<string, List<List<float>>>();

        private readonly List<string> IgnoredParams = new List<string>() { "__RequestVerificationToken" };

        public Dictionary<string, string> DecodedRequestDict = new Dictionary<string, string>();

        public Command LastExecutedCommand;

        public void OnGet()
        {
            GetSessionVariables();

            if (_frontService.GetCommandLabelList() == null)
                _frontService.ReadCommandInformation();

            ViewData["komut_optionsdict"] = _frontService.GetCommandLabelList();

            SetSessionVariables();
        }

        public void OnPost()
        {
            string debug = "";
        }

        public async Task OnPostAddMatrix()
        {
            GetSessionVariables();
            await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, DecodedRequestDict);

            if (DecodedRequestDict.ContainsKey("name") && DecodedRequestDict.ContainsKey("vals"))
            {
                _frontService.AddToMatrisDict(DecodedRequestDict["name"],
                    new MatrisBase<float>(_utils.StringTo2DList(DecodedRequestDict["vals"])),
                    savedMatrices);

            }

            SetSessionVariables();
        }

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

        public PartialViewResult OnPostUpdateMatrisTable()
        {
            GetSessionVariables();

            PartialViewResult mpart = Partial("_MatrisTablePartial", savedMatrices);

            SetSessionVariables();

            return mpart;
        }

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

        private void GetSessionVariables()
        {
            // Matrix dictionaries
            if (HttpContext.Session.Get<Dictionary<string, List<List<float>>>>(SessionMatrisDict) != null)
            {
                savedMatricesValDict = HttpContext.Session.Get<Dictionary<string, List<List<float>>>>(SessionMatrisDict);

                // TO-DO: Eliminate this process
                savedMatrices = new Dictionary<string, MatrisBase<float>>();
                foreach(string name in savedMatricesValDict.Keys)
                {
                    savedMatrices.Add(name, new MatrisBase<float>(savedMatricesValDict[name]));
                }
                //
            }

            // Last Command
            if (HttpContext.Session.Get<Command>(SessionLastCommand) != null)
            {
                LastExecutedCommand = HttpContext.Session.Get<Command>(SessionLastCommand);
            }
        }

        private void SetSessionVariables()
        {
            HttpContext.Session.Set<Command>(SessionLastCommand, LastExecutedCommand);

            // TO-DO: Eliminate this process
            savedMatricesValDict = new Dictionary<string, List<List<float>>>();
            foreach (string name in savedMatrices.Keys)
            {
                savedMatricesValDict.Add(name, savedMatrices[name].values);
            }
            //

            HttpContext.Session.Set<Dictionary<string, List<List<float>>>>(SessionMatrisDict, savedMatricesValDict);
        }
    }
}
