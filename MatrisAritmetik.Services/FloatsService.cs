using System;
using System.Collections.Generic;
using MatrisAritmetik.Core.Services;

namespace MatrisAritmetik.Services
{
    public class FloatsService<T> : IFloatsService<T>
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
            float element;
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
                    if (float.TryParse(val, out element)) temprow.Add((dynamic)element);
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
    }
}
