using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

/* This file is for managing smaller classes, enumerators etc.
 * Created to reduce file amount.
 */
namespace MatrisAritmetik.Core
{
    // ENUM CLASSES
    public enum CommandState { IDLE = -2, UNAVAILABLE = -1, SUCCESS = 0, WARNING = 1, ERROR = 2 };


    // COMMAND PARSING RELATED
    // Represents each term of an expression
    public class Command_Term 
    {
        public dynamic left;
        public dynamic op;
        public dynamic token;
        public dynamic right;

        public Command_Term(dynamic _left, dynamic _op, dynamic _right)
        {
            left = _left;
            op = _op;
            token = _op;
            right = _right;
        }
    }

    // Represents factors, numbers
    public class Command_Number
    {
        public dynamic token;
        public dynamic value;

        public Command_Number(dynamic _token)
        {
            token = _token;
            value = _token.value;
        }
    }

    // Represents saved matrices
    public class Command_Matris
    {
        public dynamic token;
        public dynamic value;

        public Command_Matris(dynamic _token)
        {
            token = _token;
            value = _token.value;
        }

    }

    // Represents function calls with "!"
    public class Command_Function
    {
        public dynamic token;
        public dynamic value;

        public Command_Function(dynamic _token)
        {
            token = _token;
            value = _token.value;
        }

    }

    public class Parser
    {
        public Parser() { }
    }

    // Session stuff
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            string serialized = JsonSerializer.Serialize(value,typeof(T));
            session.SetString(key, serialized);
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }
}
