using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrisAritmetik.Core.Services
{
    public interface IFloatsService
    {
        List<List<float>> StringTo2DList(string text, char delimiter = ' ', char newline = '\n', bool removeliterals = true);
        
    }
}
