﻿using System.Text;
using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core
{
    /// <summary>
    /// Collection of strings and functions to create warning, error and information messages
    /// </summary>
    public static class CompilerMessage
    {
        #region FILE MESSAGES
        public const string FILE_TYPE_INVALID = "Dosya tipi geçersiz! Text veya CSV olmalı!";
        public const string FILE_SIZE_INVALID = "Dosya boyutu en fazla 5MB olabilir!";
        public const string TEXT_SIZE_INVALID = "Yazı boyutu en fazla 5MB olabilir!";
        #endregion

        #region DATAFRAME MESSAGES

        #region SPAN
        //// SPAN
        public static string DF_TOTALSPAN_UNMATCH(int dimension, int span)
        {
            return "Etiket toplam genişliği " + dimension + " olmalı, " + span + " genişlik verildi!";
        }
        #endregion

        #region LIMITS
        /// <summary>
        /// Dataframe creation limit
        /// </summary>
        private static readonly string _dfLimit = "Veri tablosu limitine(=" + (int)DataframeLimits.forDataframeCount + ") ulaşıldı, atama işlemi yapmak için bir veri tablosunu siliniz! ";
        public static string DF_LIMIT => _dfLimit;

        /// <summary>
        /// All limits summarized in a multi-line string 
        /// </summary>
        private static readonly string _dfLimitsHelp = ">>> Veri tablosu limitleri(maksimum):"
                                                       + $"\n\tSatır:{(int)DataframeLimits.forRows}"
                                                       + $"\n\tSütun:{(int)DataframeLimits.forCols}"
                                                       + $"\n\tKaydedilebilir veri tablosu sayısı:{(int)DataframeLimits.forDataframeCount}"
                                                       + $"\n\tVeri tablosu isim uzunluğu:{(int)DataframeLimits.forName}"
                                                       + $"\n\tSatır etiket seviyesi:{(int)DataframeLimits.forRowLabelLevels}"
                                                       + $"\n\tSütun etiket seviyesi:{(int)DataframeLimits.forColLabelLevels}";
        public static string DF_LIMITS_HELP => _dfLimitsHelp;
        #endregion

        #region NAME
        /// <summary>
        /// A dataframe with given <paramref name="name"/> was successfully created and added to dictionary
        /// </summary>
        /// <param name="name">Dataframe names used</param>
        /// <returns>Message telling dataframe was created</returns>
        public static string SAVED_DF(string name)
        {
            return "'" + name + "' adlı bir veri tablosu oluşturuldu!";
        }
        /// <summary>
        /// Given dataframe name already exists
        /// </summary>
        /// <param name="name">Name given</param>
        /// <returns>Message telling name already exists</returns>
        public static string DF_NAME_ALREADY_EXISTS(string name)
        {
            return name + " adlı bir veri tablosu zaten oluşturulmuş!";
        }
        /// <summary>
        /// Dataframe with given name doesn't exist 
        /// </summary>
        /// <param name="name">Name given</param>
        /// <returns>Message telling no dataframe with given name was found</returns>
        public static string NOT_SAVED_DF(string name)
        {
            return "'" + name + "' adlı bir veri tablosu bulunamadı!";
        }

        /// <summary>
        /// Dataframe named <paramref name="name"/> was successfully deleted from the dictionary
        /// </summary>
        /// <param name="name">Dataframe's name</param>
        /// <returns>Message telling deletion was successful</returns>
        public static string DELETED_DF(string name)
        {
            return "'" + name + "' adlı veri tablosu silindi!";
        }
        #endregion

        #region SIZE AND DIMENSIONS
        /// <summary>
        /// Dataframe dimensions were invalid
        /// </summary>
        public const string DF_INVALID_SIZE = "Veri tablosu boyutları hatalı!";
        #endregion

        #region INVALID VALUES
        public const string DF_HAS_NON_NUMBER_VALS = "Veri tablosunda sayı olarak kullanılamayan değerler var!";
        #endregion

        #endregion

        #region MATRIX MESSAGES

        #region INTERNAL ERROR
        public const string MAT_DICT_INVALID = "Matrislere erişimde bir hata oluştu, lütfen tarayıcınızı yeniden başlatınız ya da yeni bir oturum başlatınız.";

        #endregion

        #region LIMITS
        /// <summary>
        /// Matrix creation limit
        /// </summary>
        private static readonly string _matLimit = "Matris limitine(=" + (int)MatrisLimits.forMatrisCount + ") ulaşıldı, atama işlemi yapmak için bir matrisi siliniz! ";
        public static string MAT_LIMIT => _matLimit;

        /// <summary>
        /// Matrix name length exceeded
        /// </summary>
        /// <param name="givenLen">Length of the given name</param>
        /// <returns>String containing information about name lengths</returns>
        public static string MAT_NAME_CHAR_LIMIT(int givenLen)
        {
            return "Matris adı en fazla " + (int)MatrisLimits.forName + " karakterden oluşabilir, " + givenLen + " karakter verildi!";
        }
        /// <summary>
        /// All limits summarized in a multi-line string 
        /// </summary>
        private static readonly string _matLimitsHelp = ">>> Matris limitleri(maksimum):"
                                                        + $"\n\tSatır:{(int)MatrisLimits.forRows}"
                                                        + $"\n\tSütun:{(int)MatrisLimits.forCols}"
                                                        + $"\n\tKaydedilebilir matris sayısı:{(int)MatrisLimits.forMatrisCount}"
                                                        + $"\n\tMatris isim uzunluğu:{(int)MatrisLimits.forName}";
        public static string MAT_LIMITS_HELP => _matLimitsHelp;

        #endregion

        #region NAME
        /// <summary>
        /// Matrix name was empty
        /// </summary>
        public const string MAT_NAME_EMPTY = "Matris ismi boş olamaz!";
        /// <summary>
        /// Matrix name had invalid characters
        /// </summary>
        public const string MAT_NAME_INVALID = "Matris ismi sadece alfabetik karakterler ve '_' içerebilir! ";
        /// <summary>
        /// Matrix values were invalid/empty
        /// </summary>
        public const string MAT_INVALID = "Matris değerleri boş olamaz!";

        /// <summary>
        /// Given matrix name already exists
        /// </summary>
        /// <param name="name">Name given</param>
        /// <returns>Message telling name already exists</returns>
        public static string MAT_NAME_ALREADY_EXISTS(string name)
        {
            return name + " adlı bir matris zaten oluşturulmuş!";
        }
        /// <summary>
        /// Matrix with given name doesn't exist 
        /// </summary>
        /// <param name="name">Name given</param>
        /// <returns>Message telling no matrix with given name was found</returns>
        public static string NOT_SAVED_MATRIX(string name)
        {
            return "'" + name + "' adlı bir matris bulunamadı!";
        }

        /// <summary>
        /// A matrix with given <paramref name="name"/> was successfully created and added to dictionary
        /// </summary>
        /// <param name="name">Matrix names used</param>
        /// <returns>Message telling matrix was created</returns>
        public static string SAVED_MATRIX(string name)
        {
            return "'" + name + "' adlı bir matris oluşturuldu!";
        }

        /// <summary>
        /// Matrix named <paramref name="name"/> was successfully deleted from the dictionary
        /// </summary>
        /// <param name="name">Matrix's name</param>
        /// <returns>Message telling deletion was successful</returns>
        public static string DELETED_MATRIX(string name)
        {
            return "'" + name + "' adlı matris silindi!";
        }
        #endregion

        #region SIZE AND DIMENSIONS
        /// <summary>
        /// Matrix dimensions were invalid
        /// </summary>
        public const string MAT_INVALID_SIZE = "Matris boyutu hatalı!";
        /// <summary>
        /// Matrix is not a square matrix
        /// </summary>
        public const string MAT_NOT_SQUARE = "Matris kare bir matris olmalı!";
        /// <summary>
        /// Resizing process was not processable with given dimensions
        /// </summary>
        public const string MAT_INVALID_RESIZE = "Matris yeni boyutları uygun değil!";

        /// <summary>
        /// Given dimensions didn't match expected dimensions
        /// </summary>
        /// <param name="expected">Expected dimension information</param>
        /// <param name="got">Given dimensions</param>
        /// <returns>Message telling expectation and the given value wasn't matched</returns>
        public static string MAT_UNEXPECTED_DIMENSIONS(string expected, string got)
        {
            return "Beklenen boyut: " + expected + ", Alınan boyut: " + got;
        }
        /// <summary>
        /// Given column dimension didn't match the expected dimension
        /// </summary>
        /// <param name="expected">Expected dimension information</param>
        /// <param name="got">Given dimensions</param>
        /// <returns>Message telling expectation and the given value wasn't matched</returns>
        public static string MAT_UNEXPECTED_COLUMN_SIZE(string expected, string got)
        {
            return "Sütün sayısı " + expected + " olmalı (" + got + " verildi)";
        }
        /// <summary>
        /// Given row dimension didn't match the expected dimension
        /// </summary>
        /// <param name="expected">Expected dimension information</param>
        /// <param name="got">Given dimensions</param>
        /// <returns>Message telling expectation and the given value wasn't matched</returns>
        public static string MAT_UNEXPECTED_ROW_SIZE(string expected, string got)
        {
            return "Satır sayısı " + expected + " olmalı (" + got + " verildi)";
        }
        /// <summary>
        /// Given indices are out of range
        /// </summary>
        /// <param name="axisName">Name of the axis of wrong indices</param>
        /// <param name="min">Expected minimum index</param>
        /// <param name="max">Expected maximum index</param>
        /// <returns>Information about indexing with given arguments</returns>
        public static string MAT_OUT_OF_RANGE_INDEX(string axisName, int min, int max)
        {
            return min + " tabanlı " + axisName + " indeksi minimum " + min + " maksimum " + max + " olabilir!";
        }
        /// <summary>
        /// Given row indices were invalid
        /// </summary>
        public const string MAT_INVALID_ROW_INDICES = "Satır indeks aralığı hatalı!";
        /// <summary>
        /// Given column indices were invalid
        /// </summary>
        public const string MAT_INVALID_COL_INDICES = "Sütun indeks aralığı hatalı!";

        /// <summary>
        /// Given matrix have to be a scalar matrix to be used as an operand
        /// </summary>
        public const string MAT_SHOULD_BE_SCALAR = "Verilen matris skaler olmalı!";
        #endregion

        #region OPERATOR ISSUES
        /// <summary>
        /// Dynamic value failed to be parsed as float
        /// </summary>
        public const string DYNAMIC_VAL_PARSE_FAILED = "Verilen değer sayı olarak kullanılamadı!";

        /// <summary>
        /// Addition float try-parse failed
        /// </summary>
        public const string ADDITION_PARSE_FAILED = "Toplama işlemi yapılamaz, sayı olarak kullanılamayan değerler bulundu!";
        public const string ADDITION_SIZE_INVALID = "Matris boyutları toplama işlemi için aynı olmalı!";

        /// <summary>
        /// Subtraction float try-parse failed
        /// </summary>
        public const string SUBTRACTION_PARSE_FAILED = "Çıkarma işlemi yapılamaz, sayı olarak kullanılamayan değerler bulundu!";
        public const string SUBTRACTION_SIZE_INVALID = "Matris boyutları çıkarma işlemi için aynı olmalı!";

        /// <summary>
        /// Multiplication float try-parse failed
        /// </summary>
        public const string MULTIPLICATION_PARSE_FAILED = "Çarpma işlemi yapılamaz, sayı olarak kullanılamayan değerler bulundu!";
        public const string MULTIPLICATION_SIZE_INVALID = "Matris boyutları çarpma işlemi için aynı olmalı!";

        /// <summary>
        /// Multiplication float try-parse failed
        /// </summary>
        public const string DIVISION_PARSE_FAILED = "Bölme işlemi yapılamaz, sayı olarak kullanılamayan değerler bulundu!";
        public const string DIVISION_SIZE_INVALID = "Matris boyutları bölme işlemi için aynı olmalı!";

        /// <summary>
        /// Multiplication float try-parse failed
        /// </summary>
        public const string MODULO_PARSE_FAILED = "Mod işlemi yapılamaz, sayı olarak kullanılamayan değerler bulundu!";
        public const string MODULO_SIZE_INVALID = "Matris boyutları mod işlemi için aynı olmalı!";
        public const string MODULO_MAT_INVALID = "Matris mod olarak kullanılamaz!";
        #endregion

        #endregion

        #region COMPILER RELATED

        #region FUNCTION ERRORS AND WARNINGS

        public const string INVALID_CONVERSION_TO_MAT = "Verilen veri tablosu, matris olarak kullanılamaz!";

        public const string INVALID_CONVERSION_TO_DF = "Verilen matris, veri tablosu olarak kullanılamaz!";

        /// <summary>
        /// Positional argument was given after a parameter-hinted argument
        /// </summary>
        public const string ARG_GIVEN_AFTER_HINTED_PARAM = "Parametre ismi kullanıldıktan sonra pozisyonel argüman kullanılamaz!";

        /// <summary>
        /// No arguments were passed to function
        /// </summary>
        /// <param name="name">Function name</param>
        /// <param name="count">Parameter count of <paramref name="name"/> function</param>
        /// <returns>Parameter count of <paramref name="name"/> function and how to get information about them</returns>
        public static string FUNC_REQUIRES_ARGS(string name, int count)
        {
            return name + " " + count + " parametreli bir fonksiyondur. Detaylar için: ?" + name;
        }

        /// <summary>
        /// Too many arguments were given
        /// </summary>
        /// <param name="name">Function name</param>
        /// <param name="max">Maximum parameter count</param>
        /// <param name="given">Argument count</param>
        /// <returns>Message telling <paramref name="name"/> parameter count information</returns>
        public static string FUNC_PARAMCOUNT_EXCESS(string name, int max, int given)
        {
            return name + " fonksiyonu en fazla " + max + " parametre kabul eder, " + given + " verildi!";
        }

        /// <summary>
        /// Given service name was invalid
        /// <para> THIS IS AN EXCEPTION ONLY USED INTERNALLY</para>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string UNKNOWN_SERVICE(string name)
        {
            return "Bilinmeyen servis ismi: " + name;
        }

        /// <summary>
        /// Given name didn't match any saved matrix or variable
        /// </summary>
        /// <param name="name">Given incorrect name</param>
        /// <returns>Message telling given <paramref name="name"/> didn't match any known variable</returns>
        public static string UNKNOWN_VARIABLE(string name)
        {
            return "'" + name + "' adlı bir matris/değişken bulunamadı!";
        }

        /// <summary>
        /// Passed argument type was unexpected or unknown
        /// </summary>
        /// <param name="name">Function name</param>
        /// <param name="ind">Parameter index</param>
        /// <returns>Message telling <paramref name="name"/> got an unexpected type argument</returns>
        public static string UNKNOWN_ARGUMENT_TYPE(string name, int ind)
        {
            return name + " fonksiyonunun " + ind + ". argümanının türü belirlenemedi!";
        }

        /// <summary>
        /// Defined parameter type was unknown
        /// <para>THIS IS AN EXCEPTION ONLY USED INTERNALLY</para>
        /// </summary>
        /// <param name="name">Given type name</param>
        /// <returns>Message telling type <paramref name="name"/> was unknown</returns>
        public static string UNKNOWN_PARAMETER_TYPE(string name)
        {
            return "Fonksiyon için tanımlanan parametre türü tanımlanamadı: " + name;
        }

        /// <summary>
        /// Convert type name to turkish
        /// </summary>
        /// <param name="parseType">Type name to convert</param>
        public static void TYPE_TO_TR(ref string parseType)
        {
            switch (parseType)
            {
                case "int":
                    parseType = "tamsayı";
                    break;
                case "float":
                    parseType = "ondalıklı";
                    break;
                case "double":
                    parseType = "ondalıklı";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Given value was not successfully parsed 
        /// </summary>
        /// <param name="val">Value</param>
        /// <param name="parseType">Type name tried <paramref name="val"/> to be parsed as</param>
        /// <returns>Message telling parsing didn't succeed</returns>
        public static string ARG_PARSE_ERROR(string val, string parseType)
        {
            TYPE_TO_TR(ref parseType);

            return string.IsNullOrWhiteSpace(val)
                ? "Verilen değerlerde " + parseType + " olarak kullanılayanlar var!"
                : "'" + val + "' değeri " + parseType + " olarak kullanılamadı!";
        }

        /// <summary>
        /// Given value to parameter <paramref name="name"/> was not valid
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="msg">Extra message</param>
        /// <returns>Message telling given argument to parameter <paramref name="name"/> is not valid</returns>
        public static string ARG_INVALID_VALUE(string name, string msg)
        {
            return name + " parametresi değeri hatalı. " + msg;
        }

        /// <summary>
        /// Non-default valued parameter didn't get any arguments
        /// </summary>
        /// <param name="name">Parameter name which expected a value</param>
        /// <returns>Message telling <paramref name="name"/> requires a value</returns>
        public static string MISSING_ARGUMENT(string name)
        {
            return name + " parametresine değer verilmeli!";
        }

        /// <summary>
        /// Given default value for parameter named <paramref name="name"/> was wrong
        /// <para>THIS IS AN EXCEPTION ONLY USED INTERNALLY</para>
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="realtype">Expected type name</param>
        /// <returns>Message telling <paramref name="name"/> can't be parsed as <paramref name="realtype"/></returns>
        public static string PARAM_DEFAULT_PARSE_ERROR(string name, string realtype)
        {
            TYPE_TO_TR(ref realtype);
            return name + " parametresinin default değeri hatalı, " + realtype + " olmalı!";
        }

        /// <summary>
        /// Given method doesn't exist in IFrontService interface
        /// </summary>
        /// <param name="name">Method name</param>
        /// <returns>Message telling <paramref name="name"/> is not a method</returns>
        public static string UNKNOWN_FRONTSERVICE_FUNC(string name)
        {
            return "FrontService servisi " + name + " adlı bir metod bulundurmaz!";
        }

        /// <summary>
        /// A parameter was referenced multiple times
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>Message telling <paramref name="name"/> was referenced multiple times</returns>
        public static string MULTIPLE_REFERENCES(string name)
        {
            return name + " parametresine sadece bir defa değer verilebilir!";
        }

        /// <summary>
        /// Parameter name was invalid or unknown
        /// </summary>
        /// <param name="name">Given name</param>
        /// <returns>Message telling <paramref name="name"/> was unknown</returns>
        public static string PARAMETER_NAME_INVALID(string name)
        {
            if (name == null)
            {
                return "Parametre ismi hatalı!";
            }
            else if (string.IsNullOrEmpty(name.Trim()))
            {
                return "Parametre ismi hatalı!";
            }
            return "'" + name + "' adlı bir parametre bulunamadı!";
        }

        /// <summary>
        /// Parameter name hinted was invalid or unknown 
        /// </summary>
        /// <param name="name">Given name</param>
        /// <returns>Message telling <paramref name="name"/> was invalid</returns>
        public static string PARAMETER_HINT_INVALID(string name)
        {
            if (name == null)
            {
                return "Parametre ismi hatalı!";
            }
            else if (string.IsNullOrEmpty(name.Trim()))
            {
                return "Parametre ismi hatalı!";
            }
            return name + " bir parametre adı olarak kullanılamaz!";
        }

        /// <summary>
        /// Expected format name_hint:argument wasn't matched 
        /// </summary>
        /// <param name="format">Expected format</param>
        /// <returns>Message informing about the expected format</returns>
        public static string STRING_FORMAT_INVALID(string format)
        {
            return "Argüman formatı hatalı, format: " + format + " olmalı!";
        }
        #endregion

        #region COMPILER MODE RELATED
        /// <summary>
        /// Given mode was not valid to reference a value
        /// </summary>
        /// <param name="mode">Compiler mode</param>
        /// <returns>Message telling user can't reference to some values using this <paramref name="mode"/></returns>
        public static string COMPILER_MODE_MISMATCH(CompilerDictionaryMode mode)
        {
            switch (mode)
            {
                case CompilerDictionaryMode.Matrix:
                    {
                        return $"Matris derleyici modu yalnızca matris referanslarına izin verir!";
                    }
                case CompilerDictionaryMode.Dataframe:
                    {
                        return $"Veri tablosu derleyici modu yalnızca veri tablosu referanslarına izin verir!";
                    }
                default:
                    {
                        return $"Derleyici modunda bir hata oluştu!";
                    }
            }
        }

        /// <summary>
        /// Given mode was not valid for returning return <paramref name="type"/>
        /// </summary>
        /// <param name="mode">Compiler mode</param>
        /// <param name="type">Return type</param>
        /// <returns>Message telling user can't use functions which has <paramref name="type"/> return type using this <paramref name="mode"/></returns>
        public static string COMPILER_RETURNTYPE_MISMATCH(CompilerDictionaryMode mode, string type)
        {
            switch (mode)
            {
                case CompilerDictionaryMode.Matrix:
                    {
                        return $"Matris derleyici modu {type} tipi dönen fonksiyonlara izin vermez!";
                    }
                case CompilerDictionaryMode.Dataframe:
                    {
                        return $"Veri tablosu derleyici modu {type} tipi dönen fonksiyonlara izin vermez!";
                    }
                default:
                    {
                        return $"Derleyici modunda bir hata oluştu!";
                    }
            }
        }

        #endregion

        #region FUNCTION RELATED
        // MATRIX MULTIPLICATION SIZE
        /// <summary>
        /// Matrix dimensions didn't match for matrix multiplication
        /// </summary>
        public const string MAT_MUL_BAD_SIZE = "Matris çarpımı için matrisler arası satır ve sütün boyutları uyuşmalı";

        // DETERMINANT
        /// <summary>
        /// Matrix determinant was zero, failed to get the inverse
        /// </summary>
        public const string MAT_DET_ZERO_NO_INV = "Matris determinantı 0, ters matris bulunamaz!";

        // CONCATENATION
        /// <summary>
        /// Concatenation axis given is invalid
        /// </summary>
        public const string MAT_CONCAT_AXIS_ERROR = "Axis parametresi satır eklemek için 0, sütün için 1 olmalı.";

        // CONCATENATION SIZE
        /// <summary>
        /// Matrix dimension didn't match for concatenation along given axis
        /// </summary>
        /// <param name="axis">Concatenation axis</param>
        /// <returns>Message telling which dimension should match for given axis</returns>
        public static string MAT_CONCAT_DIM_ERROR(string axis)
        {
            string other = axis == "Satır" ? "sütun" : " satır";
            return axis + " olarak eklemek için " + other + " boyutları aynı olmalı!";
        }

        // PSEUDOINVERSE
        /// <summary>
        /// Matrix was not full-rank
        /// </summary>
        public const string MAT_PSEINV_NOT_FULL_RANK = "Genelleştirilmiş ters matris oluşturulması için matris tam rank olmalı!";
        /// <summary>
        /// Side argument for pseudo-inverse function was invalid
        /// </summary>
        public const string MAT_PSEINV_BAD_SIDE = "Sol genelleştirilmiş matris için side -1, sağ için 1 olmalı!";
        /// <summary>
        /// Determinant was found zero during calculations of pseudo-inverse
        /// </summary>
        /// <param name="direction">Pseudo-inverse calculation side</param>
        /// <returns>Message telling no inverse was found from given side</returns>
        public static string MAT_PSEINV_DET_ZERO(string direction)
        {
            return "Verilen matrisin " + direction + " tersi bulunamadı!";
        }
        #endregion

        #region DOCS
        /// <summary>
        /// Message telling how to call the docs
        /// </summary>
        public const string DOCS_HELP = "'?' kullanımı ile ilgili bilgi için '?' komutunu kullanın.";

        /// <summary>
        /// Information about constants in the compiler
        /// </summary>
        public static string CONSTANTS
        {
            get
            {
                StringBuilder res = new StringBuilder();
                foreach (string key in Constants.Dict.Keys)
                {
                    res.Append('\n')
                       .Append('\t')
                       .Append(' ', 2)
                       .Append('!')
                       .Append(key)
                       .Append('\t', 2)
                       .Append(Constants.Description(key));
                }
                res.Append('\n')
                   .Append('\t')
                   .Append(' ', 2)
                   .Append('-', 30)
                   .Append(' ');
                return res.ToString();
            }
        }

        private static readonly string _compilerHelp =
                                     ">>> '?' bir ön-ektir. Bilgi almak istediğiniz terimden önce '?' koyunuz. (örnek: ?MatrisMul)"
                                     + "\n>>> Çıktılar veya menülerin üzerine işaretçinizi bekleterek gerekli bilgilere ulaşabilirsiniz."
                                     + "\n\n>>> '!' bir ön-ektir:"
                                     + "\n\to Komutları kullanmak için komut isminden önce '!' koyunuz. (örnek: !Rank)(yardım: ?komut)"
                                     + "\n\to Özel değerleri kullanmak için isimden önce '!' koyunuz. (örnek: !null)(yardım: ?değer)"
                                     + "\n\to İstenen bir parametreye değer vermek için 'parametre:argüman' formatını kullanınız.(örnek: seed:5)"
                                     + "\n\to Parametre ismi kullanıldıktan sonra pozisyonel argüman verilemez.(hatalı format: !komut(param:1,-1)"
                                     + "\n\to Özel değerler ve açıklamaları:\n\t  ------------------------------ "
                                     + CONSTANTS
                                     + "\n\n>>> Özel aritmetik operatörler (A,B: Matris, n: tam sayı):"
                                     + "\n\to .* : Matris çarpımı = A .* B"
                                     + "\n\to .^ : Matrisin kendisiyle matris çarpımı = A .^ n == A .* A .* A ... .* A"
                                     + "\n\to ./ : 2. matrisin tersi ile matris çarpımı = A ./ B == A .* !Inverse(B)"
                                     + "\n\n>>> Komutlar ve çıktılara özel stiller eklemek için komuttan sonra ';' karakterini kullanınız."
                                     + "\n>>> Stiller 'ayar_1 değer_1 ; ayar_2 değer_2 ; ...' formatını takip etmelidir."
                                     + "\n\to Komut stilleri için ayardan önce 'cmd:' ön-ekini kullanınız."
                                     + "\n\to Çıktı stilleri için ayardan önce 'out:' ön-ekini kullanınız."
                                     + "\n\to Ön-ek kullanılmazsa ayarlar hem komut hem de çıktıya uygulanır."
                                     + "\n\n\to CSS stilleri verilen kurallar çerçevesinde uygulanabilir"
                                     + "\n\t       Ayar       |        Açıklama         |    Beklenen değer   "
                                     + "\n\t ---------------- | ----------------------- | --------------------"
                                     + "\n\t       quiet      | Komut geçmişinde gizle  |          boş        "
                                     + "\n\t        tex       |  LaTeX formatında yaz   |          boş        "
                                     + "\n\t       color      |  Yazı rengini ayarla    | renk ismi veya #rgb "
                                     + "\n\t background-color | Arkaplan rengini ayarla | renk ismi veya #rgb "
                                     + "\n\t    font-weight   |     Yazı kalınlığı      | 0-900, bold, lighter"
                                     + "\n\n\to Örnek stilli komutlar:"
                                     + "\n\t\t!Identity(4) ; cmd:quiet ; out:font-weigth bolder ; color blue"
                                     + "\n\t\t!RandInt(14,8,max:30,seed:54); background-color #122122; color white; tex\n\n";

        /// <summary>
        /// Detailed documentation about the matrix/dataframe compiler
        /// </summary>
        /// <param name="mode">Compiler mode, adds limitation information to the end of returned string depending on this mode</param>
        /// <returns>Detailed multi-line string explaining rules of the compiler</returns>
        public static string COMPILER_HELP(CompilerDictionaryMode mode = CompilerDictionaryMode.Matrix)
        {
            return _compilerHelp + (mode == CompilerDictionaryMode.Matrix
                                         ? MAT_LIMITS_HELP
                                         : mode == CompilerDictionaryMode.Dataframe
                                                ? DF_LIMITS_HELP
                                                : MAT_LIMITS_HELP + "\n\n" + DF_LIMITS_HELP);
        }

        #endregion

        #region ARGUMENTS
        /// <summary>
        /// Argument count didn't match any expected count
        /// </summary>
        public const string ARG_COUNT_ERROR = "Argüman sayısı hatalı!";
        /// <summary>
        /// Minimum value was bigger than the maximum value
        /// </summary>
        public const string MAT_MINMAX_ORDER = "max değeri min değerinden büyük olamaz!";
        /// <summary>
        /// Starting value was bigger than the ending value
        /// </summary>
        public const string MAT_START_END_ORDER = "start değeri end değerinden büyük olamaz!";
        /// <summary>
        /// Given interval was not usable
        /// </summary>
        public const string MAT_INTERVAL_INVALID = "Verilen alt-aralık değeri geçersiz";
        /// <summary>
        /// Too many values would be created from given interval
        /// </summary>
        public const string MAT_INTERVAL_EXCESS = "Verilen alt-aralık değeri aralığı tanımlanan limitten fazla değere böldü";

        #endregion

        #region COMMAND FORMATS
        /// <summary>
        /// Given command was formatted wrong
        /// </summary>
        public const string CMD_FORMAT_ERROR = "Hatalı komut formatı!";
        /// <summary>
        /// Paranthesis was formatted wrong
        /// </summary>
        public const string PARANTHESIS_FORMAT_ERROR = "Parantez formatı hatalı!";
        /// <summary>
        /// Paranthesis didn't match correctly
        /// </summary>
        public const string PARANTHESIS_COUNT_ERROR = "Parametre sayısı hatalı!";

        #endregion

        #region OPERATORS
        // = OPERATOR
        /// <summary>
        /// Assignment operator was used multiple times
        /// </summary>
        public const string EQ_MULTIPLE_USE = "Atama operatörü birden fazla kez kullanılamaz!";
        /// <summary>
        /// Assignment operator format was wrong
        /// </summary>
        public const string EQ_FORMAT = "Atama işlemi sadece 'matris = matris_veya_skaler' formatında olabilir!";
        /// <summary>
        /// Assignment didn't happen because right-hand-side(RHS) was not a scalar or a matrix value
        /// </summary>
        public const string EQ_FAILED = "Atama işlemi başarısız. Atanan değer bir matris veya skaler olmalı!";

        // % OPERATOR
        /// <summary>
        /// Mod was a non-scalar matrix with base being non-matrix
        /// </summary>
        public const string MOD_MAT_THEN_BASE_MAT = "Mod matris ise diğer terim de matris olmalı!";
        /// <summary>
        /// Modulo operator format was incorrect
        /// </summary>
        public const string MODULO_FORMATS = "Modülo işlemi sadece sayı%sayı , matris%sayı ve matris%matris formatında olabilir!";

        // ^ OPERATOR
        /// <summary>
        /// Exponential value was not a real number
        /// </summary>
        public const string EXPO_NOT_SCALAR = "Üssel kısım skaler olmalı!";

        // .^ OPERATOR
        /// <summary>
        /// Base of .^ operator was not a matrix
        /// </summary>
        public const string SPECOP_MATPOWER_BASE = " .^ işlemi taban olarak matris gerektirir";
        /// <summary>
        /// Exponential of .^ operator was a negative value, expected >= 0
        /// </summary>
        public const string SPECOP_MATPOWER_EXPO = " .^ operatöründe 2. argüman negatif olamaz!";
        /// <summary>
        /// Given matrix was not square for .^ operator
        /// </summary>
        public const string SPECOP_MATPOWER_SQUARE = "Sadece kare matrisler .^ operatörünü kullanabilir!";

        #endregion

        #region UNEXPECTED OBJECT OR NAME
        /// <summary>
        /// Given <paramref name="name"/> was not a type of <paramref name="expected"/>
        /// </summary>
        /// <param name="name">Name of the value found incorrect type</param>
        /// <param name="expected">Expected type for value named <paramref name="name"/></param>
        /// <returns>Message telling given value's type didn't match what was expected</returns>
        public static string NOT_A_(string name, string expected)
        {
            return "'" + name + "' bir " + expected + " değil!";
        }
        #endregion

        #region INVALID OPERANDS
        /// <summary>
        /// Given operands can not be used with given operator
        /// </summary>
        /// <param name="op">Operator name</param>
        /// <param name="between">Arguments <paramref name="op"/> would expect</param>
        /// <returns>Message telling what <paramref name="op"/> would expect as arguments</returns>
        public static string OP_BETWEEN_(string op, string between)
        {
            return op + " işlemi " + between + " arasında gerçekleştirilir!";
        }

        /// <summary>
        /// Given operator didn't match any known operators
        /// </summary>
        /// <param name="op">Given incorrect operator</param>
        /// <returns>Message telling <paramref name="op"/> is invalid</returns>
        public static string OP_INVALID(string op)
        {
            return "Hatalı işlem operatörü: " + op;
        }

        /// <summary>
        /// Given operator requires arguments
        /// </summary>
        /// <param name="op">Operator name</param>
        /// <returns>Message telling <paramref name="op"/> can't be used by itself</returns>
        public static string OP_CANT_BE_ALONE(string op)
        {
            return op + " operatörü tek başına kullanılamaz ";
        }

        /// <summary>
        /// 'null' can't be used with operators
        /// </summary>
        public const string OP_WITH_NULL = "Aritmetik işlemlerde null değeri kullanılamaz!";

        #endregion

        #endregion

    };
}
