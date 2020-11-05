using System.Collections.Generic;

namespace MatrisAritmetik.Core.Models
{
    // Represent a token
    public class Token
    {
        // Type of token
        public TokenType tknType = TokenType.NULL;

        public void SetValues(string _symbol,OperatorAssociativity _assoc, int _priority, int _paramCount)
        {
            symbol = _symbol;
            assoc = _assoc;
            priority = _priority;
            paramCount = _paramCount;
        }
        // Number
        public dynamic val = 0.0;

        // Operator
        public string symbol = " ";
        public OperatorAssociativity assoc = OperatorAssociativity.LEFT;   // Order
        public int priority = 0;       
        public int paramCount = 0;     
        public int argCount = 0;

        // Matrix or function
        public string name = " ";
        public List<string> paramTypes = new List<string>(); 
        public string service = "";
        public string returns = "";

        // In case ? is used
        public string info = null;

        public Token() { }

        public Token(TokenType tknType,
                     dynamic val,
                     string symbol,
                     OperatorAssociativity assoc,
                     int priority,
                     int paramCount,
                     string name,
                     List<string> paramTypes,
                     string service,
                     string returns)
        {
            this.tknType = tknType;
            this.val = val;
            this.symbol = symbol;
            this.assoc = assoc;
            this.priority = priority;
            this.paramCount = paramCount;
            this.name = name;
            this.paramTypes = paramTypes;
            this.service = service;
            this.returns = returns;
        }

        // For easier debugging
        public override string ToString() => tknType switch
        {
            TokenType.ARGSEPERATOR => "ARGSEPERATOR",
            TokenType.FUNCTION => "FUNC(" + service + ")" + name + " " + paramCount.ToString() + " params",
            TokenType.MATRIS => "MAT " + name + " " + val.ToString(),
            TokenType.NUMBER => "NUM " + val.ToString(),
            TokenType.OPERATOR => "OP '" + symbol + "'",
            TokenType.NULL => "NULL",
            TokenType.LEFTBRACE => "LEFTBR",
            TokenType.RIGHTBRACE => "RIGHTBR",
            _ => tknType.ToString(),
        };
    }

}
