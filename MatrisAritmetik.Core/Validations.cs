using System;
using System.Text.RegularExpressions;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core
{
    /// <summary>
    /// Class for methods related to validating of strings
    /// </summary>
    public static class Validations
    {
        /// <summary>
        /// Check if given <paramref name="name"/> is a valid matrix name
        /// <para>Throws <see cref="CompilerMessage.MAT_NAME_EMPTY"/> if <paramref name="throwOnBadName"/> is "true"</para>
        /// </summary>
        /// <param name="name">Name for a matrix</param>
        /// <param name="throwOnBadName">Wheter to throw if name is invalid</param>
        /// <returns>True if given <paramref name="name"/> is valid, false otherwise</returns>
        public static bool ValidMatrixName(string name,
                                           bool throwOnBadName = false)
        {
            name = name.Trim();
            Regex name_regex = new Regex(@"^\w*|[0-9]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (string.IsNullOrEmpty(name.Replace(" ", "")))
            {
                if (throwOnBadName)
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

            if (!"0123456789".Contains(name[0]) && (name_regex.Match(name).Groups[0].Value == name))
            {
                return true;
            }

            if (throwOnBadName)
            {
                throw new System.Exception(CompilerMessage.MAT_NAME_INVALID);
            }
            return false;
        }

        /// <summary>
        /// Validate given <paramref name="mat"/> match with given compiler <paramref name="mode"/>
        /// </summary>
        /// <param name="mode">Compiler mode</param>
        /// <param name="mat">Matrix to check</param>
        public static void CheckModeAndMatrixReference(CompilerDictionaryMode mode,
                                                       dynamic mat)
        {
            if (mode == CompilerDictionaryMode.Dataframe && !(mat is Dataframe))
            {
                throw new Exception(CompilerMessage.COMPILER_MODE_MISMATCH(mode));
            }
            else if (mode == CompilerDictionaryMode.Matrix && mat is Dataframe)
            {
                throw new Exception(CompilerMessage.COMPILER_MODE_MISMATCH(mode));
            }
        }

        /// <summary>
        /// Validate given <paramref name="mat"/> match with given compiler <paramref name="mode"/>
        /// </summary>
        /// <param name="mode">Compiler mode</param>
        /// <param name="mat">Matrix to check</param>
        public static void CheckModeAndReturnType(CompilerDictionaryMode mode,
                                                  string returntype)
        {
            switch (returntype)
            {
                case "Matris":
                    {
                        if (mode == CompilerDictionaryMode.Dataframe)
                        {
                            throw new Exception(CompilerMessage.COMPILER_RETURNTYPE_MISMATCH(mode, returntype));
                        }
                        break;
                    }
                case "Veri Tablosu":
                    {
                        if (mode == CompilerDictionaryMode.Matrix)
                        {
                            throw new Exception(CompilerMessage.COMPILER_RETURNTYPE_MISMATCH(mode, returntype));
                        }
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }
    }
}
