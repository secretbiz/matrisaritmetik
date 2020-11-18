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


    public class RequestMessage
    {
        public static string REQUEST_MISSING_KEYS(string requestDesc, string[] keys)
        {
            return requestDesc + " isteği başarısız! Gerekli parametrelere değer verilmedi: " + string.Join(",", keys);
        }
    }

    public class CompilerMessage
    {
        ////// MATRIX ERRORS
        //// LIMITS
        public static string MAT_LIMIT = "Matris limitine(=" + (int)MatrisLimits.forMatrisCount + ") ulaşıldı, atama işlemi yapmak için bir matrisi siliniz! ";
        public static string MAT_NAME_CHAR_LIMIT(int givenLen)
        {
            return "Matris adı en fazla " + (int)MatrisLimits.forName + " karakterden oluşabilir, " + givenLen + " karakter verildi!";
        }

        //// NAME
        public const string MAT_NAME_EMPTY = "Matris ismi boş olamaz!";
        public const string MAT_NAME_INVALID = "Matris ismi sadece alfabetik karakterler ve '_' içerebilir! ";
        public static string MAT_NAME_ALREADY_EXISTS(string name)
        {
            return name + " adlı bir matris zaten oluşturulmuş!";
        }
        public static string NOT_SAVED_MATRIX(string name)
        {
            return "'" + name + "' adlı bir matris bulunamadı!";
        }

        //// SIZE AND DIMENSIONS
        public const string MAT_INVALID_SIZE = "Matris boyutu hatalı!";
        public const string MAT_NOT_SQUARE = "Matris kare bir matris olmalı!";
        public static string MAT_UNEXPECTED_DIMENSIONS(string expected, string got)
        {
            return "Beklenen boyut: " + expected + ", Alınan boyut: " + got;
        }
        public static string MAT_UNEXPECTED_COLUMN_SIZE(string expected, string got)
        {
            return "Sütün sayısı " + expected + " olmalı (" + got + " verildi)";
        }
        public static string MAT_UNEXPECTED_ROW_SIZE(string expected, string got)
        {
            return "Satır sayısı " + expected + " olmalı (" + got + " verildi)";
        }
        public static string MAT_OUT_OF_RANGE_INDEX(string axisName, int min, int max)
        {
            return min + " tabanlı " + axisName + " indeksi minimum " + min + " maksimum " + max + " olabilir!";
        }

        // MATRIX MULTIPLICATION SIZE
        public const string MAT_MUL_BAD_SIZE = "Matris çarpımı için matrisler arası satır ve sütün boyutları uyuşmalı";

        // CONCATENATION SIZE
        public static string MAT_CONCAT_DIM_ERROR(string axis)
        {
            string other = axis == "Satır" ? "sütun" : " satır";
            return axis + " olarak eklemek için " + other + " boyutları aynı olmalı!";
        }

        //// FUNCTION RELATED
        // DETERMINANT
        public const string MAT_DET_ZERO_NO_INV = "Matris determinantı 0, ters matris bulunamaz!";

        // CONCATENATION
        public const string MAT_CONCAT_AXIS_ERROR = "Axis parametresi satır eklemek için 0, sütün için 1 olmalı.";

        // PSEUDOINVERSE
        public const string MAT_PSEINV_NOT_FULL_RANK = "Genelleştirilmiş ters matris oluşturulması için matris tam rank olmalı!";
        public const string MAT_PSEINV_BAD_SIDE = "Sol genelleştirilmiş matris için side -1, sağ için 1 olmalı!";
        public static string MAT_PSEINV_DET_ZERO(string direction)
        {
            return "Verilen matrisin " + direction + " tersi bulunamadı!";
        }

        //// DOCS
        public const string DOCS_HELP = "'?' kullanımı ile ilgili bilgi için '?' komutunu kullanın.";
        public const string COMPILER_HELP =
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
                             "\n\to Komut stilleri için ayardan önce 'name:' ön-ekini kullanınız" +
                             "\n\to Çıktı stilleri için ayardan önce 'vals:' ön-ekini kullanınız" +
                             "\n\to Ön-ek kullanılmazsa ayarlar hem komut hem de çıktıya uygulanır" +
                             "\n\n\to CSS stilleri verilen kurallar çerçevesinde uygulanabilir" +
                             "\n\t       Ayar       |        Açıklama         |    Beklenen değer   " +
                             "\n\t ---------------- | ----------------------- | --------------------" +
                             "\n\t       quiet      | Komut geçmişinde gizle  |          boş        " +
                             "\n\t        tex       |  LaTeX formatında yaz   |          boş        " +
                             "\n\t       color      |  Yazı rengini ayarla    | renk ismi veya #rgb " +
                             "\n\t background-color | Arkaplan rengini ayarla | renk ismi veya #rgb " +

                             "\n\n\to Örnek stilli komut: !Identity(4) ; name:quiet ; color blue ";

        //// ARGUMENTS
        public const string ARG_COUNT_ERROR = "Argüman sayısı hatalı!";
        public const string MAT_MINMAX_ORDER = "max değeri min değerinden büyük olamaz!";

        //// COMMAND FORMATS
        public const string CMD_FORMAT_ERROR = "Hatalı komut formatı!";
        public const string PARANTHESIS_FORMAT_ERROR = "Parantez formatı hatalı!";
        public const string PARANTHESIS_COUNT_ERROR = "Parametre sayısı hatalı!";

        //// OPERATORS
        // = OPERATOR
        public const string EQ_MULTIPLE_USE = "Atama operatörü birden fazla kez kullanılamaz!";
        public const string EQ_FORMAT = "Atama işlemi sadece 'matris = matris' formatında olabilir!";
        public const string EQ_FAILED = "Atama işlemi başarısız. Atanan değer bir matris olmalı!";

        // % OPERATOR
        public const string MOD_MAT_THEN_BASE_MAT = "Mod matris ise diğer terim de matris olmalı!";
        public const string MODULO_FORMATS = "Modülo işlemi sadece sayı%sayı , matris%sayı ve matris%matris formatında olabilir!";

        // ^ OPERATOR
        public const string EXPO_NOT_NUMBER = "Üssel kısım tam sayı olmalı!";

        // .^ OPERATOR
        public const string SPECOP_MATPOWER_BASE = " .^ işlemi taban olarak matris gerektirir";
        public const string SPECOP_MATPOWER_EXPO = " .^ operatöründe 2. argüman negatif olamaz!";
        public const string SPECOP_MATPOWER_SQUARE = "Sadece kare matrisler .^ operatörünü kullanabilir!";

        //// UNEXPECTED OBJECT OR NAME
        public static string NOT_A_(string name, string expected)
        {
            return "'" + name + "' bir " + expected + " değil!";
        }

        //// INVALID OPERANDS
        public static string OP_BETWEEN_(string op, string between)
        {
            return op + " işlemi " + between + " arasında gerçekleştirilir!";
        }

        public static string OP_INVALID(string op)
        {
            return "Hatalı işlem operatörü: " + op;
        }

        public static string OP_CANT_BE_ALONE(string op)
        {
            return op + " operatörü tek başına kullanılamaz ";
        }

        //// FUNCTION ERRORS
        public const string ARG_GIVEN_AFTER_HINTED_PARAM = "Parametre ismi kullanıldıktan sonra pozisyonel argüman kullanılamaz!";

        public static string FUNC_REQUIRES_ARGS(string name, int count)
        {
            return name + " " + count + " parametreli bir fonksiyondur. Detaylar için: ?" + name;
        }

        public static string FUNC_PARAMCOUNT_EXCESS(string name, int max, int given)
        {
            return name + " fonksiyonu en fazla " + max + " parametre kabul eder, " + given + " verildi!";
        }

        public static string UNKNOWN_SERVICE(string name)
        {
            return "Bilinmeyen servis ismi: " + name;
        }

        public static string UNKNOWN_VARIABLE(string name)
        {
            return "'" + name + "' adlı bir matris/değişken bulunamadı!";
        }

        public static string UNKNOWN_ARGUMENT_TYPE(string name, int ind)
        {
            return name + " fonksiyonunun " + ind + ". argümanının türü belirlenemedi!";
        }

        public static string UNKNOWN_PARAMETER_TYPE(string name)
        {
            return "Fonksiyon için tanımlanan parametre türü tanımlanamadı: " + name;
        }

        public static string ARG_PARSE_ERROR(string val, string parseType)
        {
            return "'" + val + "' değeri " + parseType + " olarak kullanılamadı!";
        }

        public static string MISSING_ARGUMENT(string name)
        {
            return name + " parametresine değer verilmeli!";
        }

        public static string PARAM_DEFAULT_PARSE_ERROR(string name, string realtype)
        {
            return name + " parametresinin default değeri hatalı, " + realtype + " olmalı!";
        }

        public static string UNKNOWN_FRONTSERVICE_FUNC(string name)
        {
            return "FrontService servisi " + name + " adlı bir metod bulundurmaz!";
        }

        public static string MULTIPLE_REFERENCES(string name)
        {
            return name + " parametresine sadece bir defa değer verilebilir!";
        }

        public static string PARAMETER_NAME_INVALID(string name)
        {
            if (name.Trim() == "")
            {
                return "Parametre ismi hatalı!";
            }
            return "'" + name + "' adlı bir parametre bulunamadı!";
        }

        public static string PARAMETER_HINT_INVALID(string name)
        {
            if (name.Trim() == "")
            {
                return "Parametre ismi hatalı!";
            }
            return name + " bir parametre adı olarak kullanılamaz!";
        }

        public static string STRING_FORMAT_INVALID(string format)
        {
            return "Argüman formatı hatalı, format: " + format + " olmalı!";
        }

    };
    #endregion
}
