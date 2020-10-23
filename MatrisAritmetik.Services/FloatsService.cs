using System;
using System.Collections.Generic;
using MatrisAritmetik.Core.Services;

namespace MatrisAritmetik.Services
{
    public class FloatsService : IFloatsService
    {
        public List<List<float>> StringTo2DList(string text, char delimiter = ' ', char newline = '\n', bool removeliterals = true)
        {
            string filteredText = text;
            if (removeliterals)
            {
                filteredText = filteredText.Replace('\t', delimiter).Replace('\r', ' ');
            }
            List<List<float>> vals = new List<List<float>>();
            int temp = -1;
            float element;
            string[] rowsplit;
            List<float> temprow;

            foreach (var row in filteredText.Split(newline))
            {
                temprow = new List<float>();
                rowsplit = row.Split(delimiter);

                if (rowsplit.Length != temp && temp != -1)
                {
                    Console.WriteLine("Bad column size: expected " + temp.ToString() + " got " + rowsplit.Length.ToString());
                    return new List<List<float>>();
                }

                temp = 0;

                foreach (var val in rowsplit)
                {
                    if (float.TryParse(val, out element)) temprow.Add(element);
                    else
                    {
                        Console.WriteLine("Parsing failed: " + val);
                        return new List<List<float>>();
                    }
                    temp += 1;
                }
                vals.Add(temprow);
            }

            return vals;
        }
    }
}
