﻿/* This file is for managing smaller classes, enumerators etc.
 * Created to reduce file amount.
 */

using MatrisAritmetik.Core.Models;

namespace MatrisAritmetik.Core
{
    #region Enum Classes
    /// <summary>
    /// Enumerated <see cref="Command"/> states
    /// </summary>
    public enum CommandState
    {
        /// <summary>
        /// Command was created but was not processed
        /// </summary>
        IDLE,

        /// <summary>
        /// Command is currently being proccessed or had an unknown issue during a process
        /// </summary>
        UNAVAILABLE,

        /// <summary>
        /// Command was successfully processed
        /// </summary>
        SUCCESS,

        /// <summary>
        /// Command returned/threw a warning message
        /// </summary>
        WARNING,

        /// <summary>
        /// Command threw an error message
        /// </summary>
        ERROR
    };

    /// <summary>
    /// Enumerated <see cref="MatrisBase{T}"/> limitations
    /// </summary>
    public enum MatrisLimits
    {
        /// <summary>
        /// Limit row dimension
        /// </summary>
        forRows = 64,

        /// <summary>
        /// Limit column dimension
        /// </summary>
        forCols = 64,

        /// <summary>
        /// Limit matrix size
        /// </summary>
        forSize = 64 * 64,

        /// <summary>
        /// Limit how many matrices can be stored in a dictionary
        /// </summary>
        forMatrisCount = 16,

        /// <summary>
        /// Character limit for a matrix name
        /// </summary>
        forName = 64
    };

    /// <summary>
    /// Enumerated limits for command history
    /// </summary>
    public enum CompilerLimits
    {
        /// <summary>
        /// Limit how many command can be shown each command history page
        /// </summary>
        forShowOldCommands = 16,

        /// <summary>
        /// Limit how many characters can be shown for each output string
        /// </summary>
        forOutputCharacterLength = 128 * 128
    };

    /// <summary>
    /// Enumerated <see cref="Token"/> types
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Unknown token
        /// </summary>
        NULL,

        /// <summary>
        /// String token
        /// </summary>
        STRING,

        /// <summary>
        /// Number token
        /// </summary>
        NUMBER,

        /// <summary>
        /// Matrix token
        /// </summary>
        MATRIS,

        /// <summary>
        /// Function token
        /// </summary>
        FUNCTION,

        /// <summary>
        /// Function argument seperator token
        /// </summary>
        ARGSEPERATOR,

        /// <summary>
        /// Operator with a symbol token
        /// </summary>
        OPERATOR,

        /// <summary>
        /// Left brace token
        /// </summary>
        LEFTBRACE,

        /// <summary>
        /// Right brace token
        /// </summary>
        RIGHTBRACE,

        /// <summary>
        /// Token for "?" character
        /// </summary>
        DOCS,

        /// <summary>
        /// Output type for tests
        /// </summary>
        OUTPUT,

        /// <summary>
        /// Error type for tests
        /// </summary>
        ERROR,

        /// <summary>
        /// Void function return token, 
        /// </summary>
        VOID
    };

    /// <summary>
    /// Enumerated operator associativity sides
    /// </summary>
    public enum OperatorAssociativity
    {
        LEFT,
        RIGHT
    };
    #endregion

    #region Custom Error-Warning Message Classes
    /// <summary>
    /// Class for storing a <see cref="Command"/>'s <see cref="CommandStateMessage"/> and <see cref="CommandState"/> in a single instance
    /// </summary>
    public class CommandMessage
    {
        #region Public Fields
        /// <summary>
        /// Command's current state
        /// </summary>
        public CommandState State = CommandState.IDLE;
        /// <summary>
        /// Last message
        /// </summary>
        public string Message = "";
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a basic <see cref="CommandMessage"/> instance
        /// </summary>
        /// <param name="msg">Message to store</param>
        /// <param name="s"><see cref="Command"/>'s state to store</param>
        public CommandMessage(string msg, CommandState s = CommandState.IDLE)
        {
            Message = msg;
            State = s;
        }
        #endregion
    }

    /// <summary>
    /// Const strings and static string methods for storing custom command state messages
    /// </summary>
    public class CommandStateMessage
    {
        // CLEANUP
        public const string SUCCESS_CLEANUP = "Komut geçmişi temizlendi";

        // SUCCESSFUL RETURNS
        public const string SUCCESS_RET_NULL = "";
        public const string SUCCESS_RET_MAT = "";
        public const string SUCCESS_RET_NUM = "";

        // DOCS
        public const string SUCCESS_COMPILER_DOCS = "Konsol kullanma bilgisi alındı";
        public static string DOCS_MAT_FUNC_FOUND(string name)
        {
            return "Matris ve komut olan " + name + " hakkında bilgi alındı";
        }

        public static string DOCS_MAT_FOUND(string name)
        {
            return "Matris " + name + " hakkında bilgi alındı";
        }

        public static string DOCS_FUNC_FOUND(string name)
        {
            return "Fonksiyon " + name + " hakkında bilgi alındı";
        }

        public static string DOCS_NOT_MAT_FUNC(string name)
        {
            return name + " bir matris veya komut değil!";
        }

        public static string DOCS_NONE_FOUND(string name)
        {
            return name + " hakkında bir bilgi bulunamadı!";
        }

        // UNAVAILABLE
        public static string CMD_UNAVAILABLE(string org)
        {
            return "Komut işleme hatası-> \n" + org;
        }

        // ALREADY COMPILED
        public static string CMD_COMPILED(CommandState st, string msg)
        {
            return "Komut zaten işlenmiş. Durum: " + st + " Extra message: " + msg;
        }
    }

    /// <summary>
    /// Messages related to request warnings and errors
    /// </summary>
    public class RequestMessage
    {
        /// <summary>
        /// Given request didn't have required keys
        /// </summary>
        /// <param name="requestDesc">Short information of the request</param>
        /// <param name="keys">Expected keys</param>
        /// <returns>Message telling required keys</returns>
        public static string REQUEST_MISSING_KEYS(string requestDesc, string[] keys)
        {
            return requestDesc + " isteği başarısız! Gerekli parametrelere değer verilmedi: " + string.Join(",", keys);
        }
    }

    /// <summary>
    /// Collection of strings and functions to create warning, error and information messages
    /// </summary>
    public class CompilerMessage
    {
        ////// MATRIX ERRORS
        //// LIMITS
        /// <summary>
        /// Matrix creation limit
        /// </summary>
        public static string MAT_LIMIT = "Matris limitine(=" + (int)MatrisLimits.forMatrisCount + ") ulaşıldı, atama işlemi yapmak için bir matrisi siliniz! ";
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
        public static string MAT_LIMITS_HELP = "Matris limitleri:" +
                                               "\n\tSatır:" + (int)MatrisLimits.forRows + " Sütun:" + (int)MatrisLimits.forCols +
                                               "\n\tKaydedilebilir matris sayısı:" + (int)MatrisLimits.forMatrisCount +
                                               "\n\tMatris isim uzunluğu:" + (int)MatrisLimits.forName;

        //// NAME
        /// <summary>
        /// Matrix name was empty
        /// </summary>
        public const string MAT_NAME_EMPTY = "Matris ismi boş olamaz!";
        /// <summary>
        /// Matrix name had invalid characters
        /// </summary>
        public const string MAT_NAME_INVALID = "Matris ismi sadece alfabetik karakterler ve '_' içerebilir! ";
        /// <summary>
        /// Matrix was invalid somehow
        /// </summary>
        public const string MAT_INVALID = "Matris kullanılamaz durumda!";

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

        //// SIZE AND DIMENSIONS
        /// <summary>
        /// Matrix dimensions were invalid
        /// </summary>
        public const string MAT_INVALID_SIZE = "Matris boyutu hatalı!";
        /// <summary>
        /// Matrix is not a square matrix
        /// </summary>
        public const string MAT_NOT_SQUARE = "Matris kare bir matris olmalı!";
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

        //// FUNCTION RELATED
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

        //// DOCS
        /// <summary>
        /// Message telling how to call the docs
        /// </summary>
        public const string DOCS_HELP = "'?' kullanımı ile ilgili bilgi için '?' komutunu kullanın.";
        /// <summary>
        /// Short and detailed documentation about the matrix compiler
        /// </summary>
        public static string COMPILER_HELP =
                             ">>> '?' bir ön-ektir. Bilgi almak istediğiniz terimden önce '?' koyunuz (?komut_veya_matris)" +
                             "\n>>> Çıktılar veya menülerin üzerine işaretçinizi bekleterek gerekli bilgilere ulaşabilirsiniz." +

                             "\n\n>>> Fonksiyonları kullanmak için '!' ön-ekini fonksiyon isminden önce koyunuz (!komut)" +
                             "\n\to İstenen bir parametreye değer vermek için 'parametre_ismi:argüman' formatını kullanınız " +
                             "\n\to Parametre ismi kullanıldıktan sonra pozisyonel argüman verilemez." +

                             "\n\n>>> Özel aritmetik operatörler (A,B: Matris, n: tam sayı):" +
                             "\n\to .* : Matris çarpımı = A .* B" +
                             "\n\to .^ : Matrisin kendisiyle matris çarpımı = A .^ n == A .* A .* A ... .* A" +
                             "\n\to ./ : 2. matrisin tersi ile matris çarpımı = A ./ B == A .* !Inverse(B)" +

                             "\n\n>>> Komutlar ve çıktılara özel stiller eklemek için komuttan sonra ';' karakterini kullanınız" +
                             "\n>>> Stiller 'ayar_1 değer_1 ; ayar_2 değer_2 ; ...' formatını takip etmelidir" +
                             "\n\to Komut stilleri için ayardan önce 'cmd:' ön-ekini kullanınız" +
                             "\n\to Çıktı stilleri için ayardan önce 'out:' ön-ekini kullanınız" +
                             "\n\to Ön-ek kullanılmazsa ayarlar hem komut hem de çıktıya uygulanır" +
                             "\n\n\to CSS stilleri verilen kurallar çerçevesinde uygulanabilir" +
                             "\n\t       Ayar       |        Açıklama         |    Beklenen değer   " +
                             "\n\t ---------------- | ----------------------- | --------------------" +
                             "\n\t       quiet      | Komut geçmişinde gizle  |          boş        " +
                             "\n\t        tex       |  LaTeX formatında yaz   |          boş        " +
                             "\n\t       color      |  Yazı rengini ayarla    | renk ismi veya #rgb " +
                             "\n\t background-color | Arkaplan rengini ayarla | renk ismi veya #rgb " +
                             "\n\t    font-weight   |     Yazı kalınlığı      | 0-900, bold, lighter" +

                             "\n\n\to Örnek stilli komutlar:" +
                             "\n\t\t!Identity(4) ; cmd:quiet ; out:font-weigth bolder ; color blue" +
                             "\n\t\t!RandFloatMat(4,4,2,3,1); background-color #d233c1; color white; tex\n\n" +
                             MAT_LIMITS_HELP;

        //// ARGUMENTS
        /// <summary>
        /// Argument count didn't match any expected count
        /// </summary>
        public const string ARG_COUNT_ERROR = "Argüman sayısı hatalı!";
        /// <summary>
        /// Minimum value was bigger than the maximum value
        /// </summary>
        public const string MAT_MINMAX_ORDER = "max değeri min değerinden büyük olamaz!";

        //// COMMAND FORMATS
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

        //// OPERATORS
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
        public const string EXPO_NOT_NUMBER = "Üssel kısım tam sayı olmalı!";

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

        //// UNEXPECTED OBJECT OR NAME
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

        //// INVALID OPERANDS
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

        //// FUNCTION ERRORS
        /// <summary>
        /// Positional argument was given after a parameter-hinted argument
        /// </summary>
        public const string ARG_GIVEN_AFTER_HINTED_PARAM = "Parametre ismi kullanıldıktan sonra pozisyonel argüman kullanılamaz!";

        /// <summary>
        /// No arguments were passed to function
        /// </summary>
        /// <param name="name">Fucntion name</param>
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
        /// Given value was not successfully parsed 
        /// </summary>
        /// <param name="val">Value</param>
        /// <param name="parseType">Type name tried <paramref name="val"/> to be parsed as</param>
        /// <returns>Message telling parsing didn't succeed</returns>
        public static string ARG_PARSE_ERROR(string val, string parseType)
        {
            return "'" + val + "' değeri " + parseType + " olarak kullanılamadı!";
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
            if (name.Trim() == "")
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
            if (name.Trim() == "")
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

    };
    #endregion
}
