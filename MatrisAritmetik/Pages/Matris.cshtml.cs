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
        private readonly IUtilityService<float> _utils;
        private readonly IFrontService _frontService;
        private readonly IMatrisArithmeticService<float> _matrisService;
        public MatrisModel(IUtilityService<float> utilityService, IFrontService frontService,IMatrisArithmeticService<float> matrisService)
        {
            _utils = utilityService;
            _frontService = frontService;
            _matrisService = matrisService;
        }

        private readonly List<string> IgnoredParams = new List<string>() { "__RequestVerificationToken" };

        public Dictionary<string, string> DecodedRequestDict = new Dictionary<string, string>();

        public Command LastExecutedCommand;

        public void OnGet()
        {
            // Sayfa yenileme durumunda kayıtlı matrisleri göster
            if(_frontService.GetMatrisDict().Count != 0)
            {
                TempData["floatdict"] = _frontService.GetMatrisDict();
            }

            // Hazır komut bilgilerini al
            if(_frontService.GetCommandLabelList() == null)
                _frontService.ReadCommandInformation();

            TempData["komut_optionsdict"] = _frontService.GetCommandLabelList();
        }

        public void OnPost()
        {
            
        }

        public async Task OnPostAddMatrix()
        {
            await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, DecodedRequestDict);

            if (DecodedRequestDict.ContainsKey("name") && DecodedRequestDict.ContainsKey("vals"))
                _frontService.AddToMatrisDict(DecodedRequestDict["name"],
                    new MatrisBase<float>(_utils.StringTo2DList(DecodedRequestDict["vals"])));

        }

        public async Task OnPostDeleteMatrix()
        {
            await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, DecodedRequestDict);

            if (DecodedRequestDict.ContainsKey("name"))
                _frontService.DeleteFromMatrisDict(DecodedRequestDict["name"].Replace("matris_table_delbutton_",""));

        }

        public PartialViewResult OnPostUpdateMatrisTable()
        {
            return Partial("_MatrisTablePartial", _frontService.GetMatrisDict());
        }

        public async Task OnPostSendCmd()
        {
            await _utils.ReadAndDecodeRequest(Request.Body, Encoding.Default, IgnoredParams, DecodedRequestDict);

            if (DecodedRequestDict.ContainsKey("cmd"))
                LastExecutedCommand = _frontService.CreateCommand(DecodedRequestDict["cmd"]);

        }

    }
}
