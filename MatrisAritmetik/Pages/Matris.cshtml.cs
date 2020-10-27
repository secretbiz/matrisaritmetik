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
        private readonly IFloatsService<float> _floatArithmetic;
        private readonly IFrontService _frontService;
        private readonly IMatrisArithmeticService<float> _matrisService;
        public MatrisModel(IFloatsService<float> floatArithmetic, IFrontService frontService,IMatrisArithmeticService<float> matrisService)
        {
            _floatArithmetic = floatArithmetic;
            _frontService = frontService;
            _matrisService = matrisService;
        }

        private string[] body;

        private readonly string[] ignoredParams = new string[] { "__RequestVerificationToken" };

        private Dictionary<string, string> decodeDict = new Dictionary<string, string>();

        private void urlDecode(string url)
        {
            body = WebUtility.UrlDecode(url).Split("&");    // body = "param=somevalue&param2=someothervalue"
            decodeDict.Clear();

            string[] pairsplit;
            foreach (var pair in body)
            {
                pairsplit = pair.Split("=");
                if (ignoredParams.Contains(pairsplit[0]))
                    continue;

                decodeDict.Add(pairsplit[0], pairsplit[1]);
            }
        }

        public void OnGet()
        {
            // Sayfa yenileme durumunda kayıtlı matrisleri göster
            if(_frontService.GetMatrisDict().Count != 0)
            {
                TempData["floatdict"] = _frontService.GetMatrisDict();
            }
        }

        public void OnPost()
        {
            
        }

        public async Task OnPostAddMatrix()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.Default))
            {
                string temp = await reader.ReadToEndAsync();
                
                urlDecode(temp);

                if (decodeDict.ContainsKey("name") && decodeDict.ContainsKey("vals"))
                    _frontService.AddToMatrisDict(decodeDict["name"],
                        new MatrisBase<float>(_floatArithmetic.StringTo2DList(decodeDict["vals"])));
            }

        }

        public async Task OnPostDeleteMatrix()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.Default))
            {
                string temp = await reader.ReadToEndAsync();

                urlDecode(temp);

                if (decodeDict.ContainsKey("name"))
                    _frontService.DeleteFromMatrisDict(decodeDict["name"].Replace("matris_table_delbutton_",""));
            }

        }

        public PartialViewResult OnPostUpdateMatrisTable()
        {
            return Partial("_MatrisTablePartial", _frontService.GetMatrisDict());
        }

        public async Task OnPostSendCmd()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.Default))
            {
                string temp = await reader.ReadToEndAsync();

                urlDecode(temp);

                Command debugCmd = _frontService.EvaluateCommand(decodeDict["cmd"]);
            }

        }

    }
}
