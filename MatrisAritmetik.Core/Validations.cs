using System.Text.RegularExpressions;

/* This file is for managing smaller classes, enumerators etc.
 * Created to reduce file amount.
 */
namespace MatrisAritmetik.Core
{
    public static class Validations
    {
        public static bool ValidMatrixName(string name, bool throwOnBadName=false)
        {
            name = name.Trim();
            Regex name_regex = new Regex(@"^\w*|[0-9]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (name.Replace(" ", "") == "")
            {
                if(throwOnBadName)
                {
                    throw new System.Exception(CompilerMessage.MAT_NAME_EMPTY);
                }
                return false;
            }

            if (name.Length > (int)MatrisLimits.forName)
            {
                if (throwOnBadName)
                {
                    throw new System.Exception(CompilerMessage.MAT_NAME_CHAR_LIMIT(name.Length));
                }
                return false;
            }

            if (!("0123456789".Contains(name[0])) && (name_regex.Match(name).Groups[0].Value == name))
            {
                return true;
            }

            if (throwOnBadName)
            {
                throw new System.Exception(CompilerMessage.MAT_NAME_INVALID);
            }
            return false;
        }

    }
}
