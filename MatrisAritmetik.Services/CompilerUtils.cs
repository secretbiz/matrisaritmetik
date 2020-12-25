using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using MatrisAritmetik.Core;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Services
{
    /// <summary>
    /// Class for methods that matrix compiler uses often
    /// <para>Created for better maintainability</para>
    /// </summary>
    public static class CompilerUtils
    {
        #region Arithmetic Methods
        /// <summary>
        /// Parses <paramref name="a"/> and <paramref name="b"/> as <see cref="float"/>s and adds them together
        /// </summary>
        /// <param name="a">Any numerically parsable value</param>
        /// <param name="b">Any numerically parsable value</param>
        /// <returns>Result of <paramref name="a"/>+<paramref name="b"/> parsed as <see cref="float"/>s</returns>
        private static T Add<T>(T a, T b)
        {
            return (dynamic)(float.Parse(a.ToString()) + float.Parse(b.ToString()));
        }

        /// <summary>
        /// Parses <paramref name="a"/> and <paramref name="b"/> as <see cref="float"/>s and multiplies them together
        /// </summary>
        /// <param name="a">Any numerically parsable value</param>
        /// <param name="b">Any numerically parsable value</param>
        /// <returns>Result of <paramref name="a"/>*<paramref name="b"/> parsed as <see cref="float"/>s</returns>
        private static T Mul<T>(T a, T b)
        {
            return (dynamic)(float.Parse(a.ToString()) * float.Parse(b.ToString()));
        }

        /// <summary>
        /// Dot product of given lists <paramref name="a"/> and <paramref name="b"/>
        /// <para>Values in <paramref name="a"/> and <paramref name="b"/> are parsed as <see cref="float"/>s during calculation</para>
        /// </summary>
        /// <param name="a">List of numerically parsable values</param>
        /// <param name="b">List of numerically parsable values</param>
        /// <returns>Dot product casted as <typeparamref name="T"/></returns>
        private static T DotProduct<T>(List<T> a,
                                       List<T> b)
        {
            if (a.Count != b.Count)
            {
                throw new Exception("Boyutlar hatalı");
            }

            if (a.Count == 0)
            {
                throw new Exception("Boyut sıfır");
            }

            T res = Mul(a[0], b[0]);

            for (int i = 1; i < a.Count; i++)
            {
                res = Add(res, Mul(a[i], b[i]));
            }

            return res;
        }

        /// <summary>
        /// Custom power operator
        /// </summary>
        /// <param name="a">base value</param>
        /// <param name="b">exponential value</param>
        /// <returns>a^b</returns>
        public static double PowerMethod(double a, double b)
        {
            double result;
            if (b == 0.0)
            {
                if (a == 0.0) // 0^0
                {
                    result = double.NaN;
                }
                else
                {
                    result = 1.0;    // x^0
                }
            }
            else if (b < 0.0)
            {
                result = a == 0.0 ? double.PositiveInfinity : Math.Pow(a, b);
            }
            else
            {
                result = a == 0.0 ? 0.0 : Math.Pow(a, b);
            }
            return result;
        }

        public static MatrisBase<T> MatrisMul<T>(MatrisBase<T> A, MatrisBase<T> B)
        {
            if (A.Col != B.Row)
            {
                throw new Exception(CompilerMessage.MAT_MUL_BAD_SIZE);
            }

            List<List<T>> result = new List<List<T>>();

            for (int i = 0; i < A.Row; i++)
            {
                result.Add(new List<T>());
                for (int j = 0; j < B.Col; j++)
                {
                    result[i].Add(DotProduct(A.GetValues()[i], B.ColList(j, 0)));
                }
            }

            return new MatrisBase<T>(result);

        }

        #endregion

        #region Other Utility Methods
        /// <summary>
        /// Check if given token <paramref name="tkn"/> is a <see cref="MatrisBase{object}"/>
        /// </summary>
        /// <param name="tkn">Token to check</param>
        /// <param name="matDict">Matrix dictionary to reference to</param>
        /// <returns>True if given token holds a <see cref="MatrisBase{object}"/></returns>
        public static bool CheckMatrixAndUpdateVal(Token tkn,
                                                    Dictionary<string, MatrisBase<dynamic>> matDict)
        {
            switch (tkn.tknType)
            {
                case TokenType.MATRIS:
                    if (matDict.ContainsKey(tkn.name))
                    {
                        tkn.val = matDict[tkn.name];
                    }
                    else if (!(tkn.val is MatrisBase<object>))
                    {
                        throw new Exception(CompilerMessage.NOT_SAVED_MATRIX(tkn.name));
                    }

                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Parse default values from <paramref name="parameterInfo"/> into given "null" values in <paramref name="arguments"/> 
        /// </summary>
        /// <param name="parameterCount">How many parameters to iterate over</param>
        /// <param name="arguments">Arguments array</param>
        /// <param name="parameterInfo">Parameter information array</param>
        /// <returns><paramref name="arguments"/> where "null" values are parsed as default values picked from <paramref name="parameterInfo"/></returns>
        public static object[] ParseDefaultValues(int parameterCount,
                                                  object[] arguments,
                                                  ParameterInfo[] parameterInfo)
        {
            for (int k = 0; k < parameterCount; k++)
            {
                if (arguments[k] != null)    // Skip already parsed values
                {
                    continue;
                }
                else if (parameterInfo[k].DefaultValue != null) // default value wasn't null
                {
                    switch (parameterInfo[k].DefaultValue.GetType().ToString())
                    {
                        case "System.DBNull":
                            {
                                throw new Exception(CompilerMessage.MISSING_ARGUMENT(parameterInfo[k].Name));
                            }
                        case "System.Int32":
                            {
                                arguments[k] = Convert.ToInt32(parameterInfo[k].DefaultValue, CultureInfo.CurrentCulture);
                                break;
                            }
                        case "System.Single":
                            {
                                arguments[k] = Convert.ToSingle(parameterInfo[k].DefaultValue, CultureInfo.CurrentCulture);
                                break;
                            }
                        case "System.Double":
                            {
                                arguments[k] = Convert.ToDouble(parameterInfo[k].DefaultValue, CultureInfo.CurrentCulture);
                                break;
                            }
                        default:
                            throw new Exception(CompilerMessage.PARAM_DEFAULT_PARSE_ERROR(parameterInfo[k].Name, parameterInfo[k].ParameterType.Name));
                    }
                }
            }

            return arguments;
        }

        #endregion

        #region Operator Methods
        /// <summary>
        /// Assign <paramref name="RHS"/> matrix or scalar to item named <paramref name="LHS"/>
        /// </summary>
        /// <param name="LHS">Left hand side, name will be picked from this</param>
        /// <param name="RHS">Right hand side, value will be picked from this</param>
        /// <param name="matDict">Matrix dictionary to refer to if needed</param>
        public static void OPAssignment(Token LHS,
                                        Token RHS,
                                        Dictionary<string, MatrisBase<dynamic>> matDict)
        {

            if (LHS.tknType != TokenType.MATRIS) // LHS should just be a valid name for a matrix
            {
                throw new Exception(CompilerMessage.EQ_FORMAT);
            }
            else
            {
                switch (RHS.tknType)
                {
                    case TokenType.NUMBER:  // RHS is scalar
                        {
                            RHS.val = new MatrisBase<dynamic>(1, 1, RHS.val);
                            break;
                        }
                    case TokenType.MATRIS:   // RHS is possibly a matrix
                        {
                            RHS.val = matDict.ContainsKey(RHS.name)
                                ? (dynamic)matDict[RHS.name]
                                : RHS.val is MatrisBase<object>
                                    ? RHS.val
                                    : throw new Exception(CompilerMessage.NOT_SAVED_MATRIX(RHS.name));
                            break;
                        }
                    default:
                        {
                            if (!(RHS.val is MatrisBase<object>))  // If RHS is not even a matrix, throw 
                            {
                                throw new Exception(CompilerMessage.EQ_FAILED);
                            }
                            break;
                        }
                }

                // Update the matrix table accordingly
                if (matDict.ContainsKey(LHS.name))
                {
                    matDict[LHS.name] = RHS.val;
                }
                else if (Validations.ValidMatrixName(LHS.name))
                {
                    if (matDict.Count < (int)MatrisLimits.forMatrisCount)
                    {
                        matDict.Add(LHS.name, RHS.val);
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.MAT_LIMIT);
                    }
                }
                else // LHS was invalid
                {
                    if (string.IsNullOrEmpty(LHS.name))
                    {
                        throw new Exception(CompilerMessage.EQ_FORMAT);
                    }
                    else
                    {
                        throw new Exception(CompilerMessage.MAT_NAME_INVALID);
                    }
                }
            }
        }

        /// <summary>
        /// Addition, subtraction, multiplication or division between <paramref name="LHS"/> and <paramref name="RHS"/> 
        /// </summary>
        /// <param name="symbol">One of : "+", "-", "*", "/"</param>
        /// <param name="LHS">Left hand side</param>
        /// <param name="RHS">Right hand side</param>
        /// <param name="matDict">Matrix dictionary to refer to if needed</param>
        public static void OPBasic(string symbol,
                                   Token LHS,
                                   Token RHS,
                                   Dictionary<string, MatrisBase<dynamic>> matDict)
        {
            CheckMatrixAndUpdateVal(RHS, matDict);
            CheckMatrixAndUpdateVal(LHS, matDict);
            RHS.tknType = LHS.tknType == TokenType.MATRIS ? TokenType.MATRIS : RHS.tknType;

            switch (symbol)
            {
                case "+":
                    {
                        RHS.val = LHS.val + RHS.val;
                        break;
                    }
                case "-":
                    {
                        RHS.val = LHS.val - RHS.val;
                        break;
                    }
                case "*":
                    {
                        RHS.val = LHS.val * RHS.val;
                        break;
                    }
                case "/":
                    {
                        RHS.val = LHS.val / RHS.val;
                        break;
                    }
                default:
                    throw new Exception(CompilerMessage.OP_INVALID(symbol));

            }
        }

        /// <summary>
        /// Given <paramref name="_base"/> value in mode <paramref name="_expo"/>
        /// </summary>
        /// <param name="_base">Base value</param>
        /// <param name="_expo">Mode value</param>
        /// <param name="matDict">Matrix dictionary to refer to if needed</param>
        public static void OPModulo(Token _base,
                                    Token _mode,
                                    Dictionary<string, MatrisBase<dynamic>> matDict)
        {
            if (_mode.tknType == TokenType.NUMBER) // dynamic % number
            {
                CheckMatrixAndUpdateVal(_base, matDict);

                _mode.val = _base.val % (dynamic)(float)_mode.val;
                _mode.tknType = _base.tknType;
            }
            else if (CheckMatrixAndUpdateVal(_mode, matDict)) // matris % matris
            {
                // base _mode
                // term to get mod of _baseshould be matrix
                _mode.val = CheckMatrixAndUpdateVal(_base, matDict) || ((MatrisBase<dynamic>)_mode.val).IsScalar()
                    ? _base.val % _mode.val
                    : throw new Exception(CompilerMessage.MOD_MAT_THEN_BASE_MAT);
            }
            else
            {
                throw new Exception(CompilerMessage.MODULO_FORMATS);
            }
        }

        /// <summary>
        /// Given <paramref name="_base"/> value raised to power <paramref name="_expo"/>
        /// </summary>
        /// <param name="_base">Base value</param>
        /// <param name="_expo">Exponential value</param>
        /// <param name="matDict">Matrix dictionary to refer to if needed</param>
        public static void OPExpo(Token _base,
                                  Token _expo,
                                  Dictionary<string, MatrisBase<dynamic>> matDict)
        {
            if (_expo.tknType != TokenType.NUMBER)
            {
                _expo.val = !CheckMatrixAndUpdateVal(_expo, matDict)
                                  ? throw new Exception(CompilerMessage.EXPO_NOT_SCALAR)
                                  : !((MatrisBase<dynamic>)_expo.val).IsScalar()
                                      ? throw new Exception(CompilerMessage.MAT_SHOULD_BE_SCALAR)
                                      : ((MatrisBase<dynamic>)_expo.val)[0, 0];
            }

            if (CheckMatrixAndUpdateVal(_base, matDict))  // base matrix
            {
                _expo.val = ((MatrisBase<object>)_base.val).Power((dynamic)_expo.val);
                _expo.tknType = TokenType.MATRIS;
            }
            else // base is number
            {
                _expo.val = CompilerUtils.PowerMethod(double.Parse(_base.val.ToString()), double.Parse(_expo.val.ToString()));
                _expo.tknType = TokenType.NUMBER;
            }
        }

        /// <summary>
        /// Matrix multiplication of given tokens
        /// </summary>
        /// <param name="LHS">Left matrix</param>
        /// <param name="RHS">Right matrix</param>
        /// <param name="matDict">Matrix dictionary to refer to if needed</param>
        public static void OPMatMul(Token LHS,
                                    Token RHS,
                                    Dictionary<string, MatrisBase<dynamic>> matDict)
        {

            MatrisBase<dynamic> mat1, mat2;
            mat1 = CheckMatrixAndUpdateVal(RHS, matDict)
                ? (MatrisBase<dynamic>)RHS.val
                : throw new Exception(CompilerMessage.OP_BETWEEN_(".*", "matrisler"));

            mat2 = CheckMatrixAndUpdateVal(LHS, matDict)
                ? (MatrisBase<dynamic>)LHS.val
                : throw new Exception(CompilerMessage.OP_BETWEEN_(".*", "matrisler"));

            RHS.val = MatrisMul(mat2, mat1);

        }

        /// <summary>
        /// Matrix multiplication of given <paramref name="_base"/> with itself <paramref name="_expo"/> times
        /// </summary>
        /// <param name="_base">Matrix to use</param>
        /// <param name="_expo">Exponential to use</param>
        /// <param name="matDict">Matrix dictionary to refer to if needed</param>
        public static void OPMatMulByExpo(Token _base,
                                          Token _expo,
                                          Dictionary<string, MatrisBase<dynamic>> matDict)
        {
            if (_expo.tknType != TokenType.NUMBER)
            {
                _expo.val = !CheckMatrixAndUpdateVal(_expo, matDict)
                                  ? throw new Exception(CompilerMessage.EXPO_NOT_SCALAR)
                                  : !((MatrisBase<dynamic>)_expo.val).IsScalar()
                                      ? throw new Exception(CompilerMessage.MAT_SHOULD_BE_SCALAR)
                                      : float.Parse(((MatrisBase<object>)_expo.val)[0, 0].ToString());

                if (!(_expo.val is int) && !(_expo.val is float) && !(_expo.val is double))
                {
                    throw new Exception(CompilerMessage.EXPO_NOT_SCALAR);
                }
            }

            if (_expo.val < 0)
            {
                throw new Exception(CompilerMessage.SPECOP_MATPOWER_EXPO);
            }
            else if (_expo.val == 0)
            {
                _expo.val = 1;
                _expo.tknType = TokenType.NUMBER;
                return;
            }

            if (CheckMatrixAndUpdateVal(_base, matDict))
            {
                if (!_base.val.IsSquare())
                {
                    throw new Exception(CompilerMessage.SPECOP_MATPOWER_SQUARE);
                }

                MatrisBase<dynamic> res = _base.val.Copy();
                using MatrisBase<dynamic> mat = res.Copy();

                for (int i = 1; i < _expo.val; i++)
                {
                    res = MatrisMul(res, mat);
                }

                _expo.val = res;
                _expo.tknType = TokenType.MATRIS;
            }
            else
            {
                throw new Exception(CompilerMessage.SPECOP_MATPOWER_BASE);
            }
        }

        /// <summary>
        /// Matrix multiplication of <paramref name="LHS"/> and inversed <paramref name="RHS"/> 
        /// </summary>
        /// <param name="LHS">Left matrix</param>
        /// <param name="RHS">Right matrix, inverse of this will be used</param>
        /// <param name="matDict">Matrix dictionary to refer to if needed</param>
        public static void OPMatMulWithInverse(Token LHS,
                                               Token RHS,
                                               Dictionary<string, MatrisBase<dynamic>> matDict)
        {
            MatrisBase<dynamic> mat1, mat2;
            mat1 = CheckMatrixAndUpdateVal(RHS, matDict)
                 ? (MatrisBase<dynamic>)RHS.val
                 : throw new Exception(CompilerMessage.OP_BETWEEN_("./", "matrisler"));

            mat2 = CheckMatrixAndUpdateVal(LHS, matDict)
                 ? (MatrisBase<dynamic>)LHS.val
                 : throw new Exception(CompilerMessage.OP_BETWEEN_("./", "matrisler"));

            RHS.val = MatrisMul(mat2, new MatrisArithmeticService<dynamic>().Inverse(mat1));
        }

        #endregion

    }
}
