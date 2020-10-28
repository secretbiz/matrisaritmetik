using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core.Services
{
    public interface IUtilityService<T>
    {
        List<List<T>> StringTo2DList(string text, char delimiter = ' ', char newline = '\n', bool removeliterals = true);

        Task ReadAndDecodeRequest(Stream reqbody, Encoding enc, List<string> ignoredparams, Dictionary<string, string> decodedRequestDict);

    }
}
