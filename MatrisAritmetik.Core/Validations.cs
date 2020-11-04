using System.Text.RegularExpressions;

/* This file is for managing smaller classes, enumerators etc.
 * Created to reduce file amount.
 */
namespace MatrisAritmetik.Core
{
    public static class Validations
    {
        public static bool ValidMatrixName(string name)
        {
            Regex name_regex = new Regex(@"^\w*|[0-9]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (name.Replace(" ", "") == "")
                return false;

            if (name.Length > (int)MatrisLimits.forName)
                return false;

            return !("0123456789".Contains(name[0])) &&
                   (name_regex.Match(name).Groups[0].Value == name);
        }

    }
}
