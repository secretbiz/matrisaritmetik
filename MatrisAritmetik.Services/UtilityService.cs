using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MatrisAritmetik.Core.Models;
using MatrisAritmetik.Core.Services;

namespace MatrisAritmetik.Services
{
    public class UtilityService<T> : IUtilityService<T>
    {
        public List<List<T>> StringTo2DList(string text, char delimiter = ' ', char newline = '\n', bool removeliterals = true)
        {
            string filteredText = text;
            if (removeliterals)
            {
                filteredText = filteredText.Replace('\t', delimiter).Replace('\r', ' ');
            }
            List<List<T>> vals = new List<List<T>>();
            int temp = -1;
            string[] rowsplit;
            List<T> temprow;

            foreach (var row in filteredText.Split(newline))
            {
                temprow = new List<T>();
                rowsplit = row.Split(delimiter);

                if (rowsplit.Length != temp && temp != -1)
                {
                    Console.WriteLine("Bad column size: expected " + temp.ToString() + " got " + rowsplit.Length.ToString());
                    return new List<List<T>>();
                }

                temp = 0;

                foreach (var val in rowsplit)
                {
                    if (float.TryParse(val, out float element)) temprow.Add((dynamic)element);
                    else
                    {
                        Console.WriteLine("Parsing failed: " + val);
                        return new List<List<T>>();
                    }
                    temp += 1;
                }
                vals.Add(temprow);
            }

            return vals;
        }

        public async Task ReadAndDecodeRequest(Stream reqbody, Encoding enc, List<string> ignoredparams, Dictionary<string,string> decodedRequestDict)
        {
            using var reader = new StreamReader(reqbody, enc);
            string url = await reader.ReadToEndAsync();

            string[] body = WebUtility.UrlDecode(url).Split("&");    // body = "param=somevalue&param2=someothervalue"
            string[] pairsplit;

            decodedRequestDict.Clear();

            foreach (var pair in body)
            {
                pairsplit = pair.Split("="); // pairsplit[] = { key , value }

                if (ignoredparams.Contains(pairsplit[0]))
                    continue;

                if (!decodedRequestDict.ContainsKey(pairsplit[0]))
                    decodedRequestDict.Add(pairsplit[0], pairsplit[1]);
            }

        }

    }
}
