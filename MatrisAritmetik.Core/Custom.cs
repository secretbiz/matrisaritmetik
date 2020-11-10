/* This file is for managing smaller classes, enumerators etc.
 * Created to reduce file amount.
 */
namespace MatrisAritmetik.Core
{
    public class CommandMessage
    {
        public CommandState State = CommandState.IDLE;
        public string Message = "";
        public CommandMessage(string msg, CommandState s = CommandState.IDLE)
        {
            Message = msg;
            State = s;
        }
    }

    // ENUM CLASSES
    // Command's states
    public enum CommandState
    {
        IDLE,
        UNAVAILABLE,
        SUCCESS,
        WARNING,
        ERROR
    };

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

    // For limiting matrix creation
    public enum MatrisLimits
    {
        forRows = 128,
        forCols = 128,
        forSize = 128 * 128,
        forMatrisCount = 8,
        forName = 64
    };

    // Token types
    public enum TokenType
    {
        NULL,
        NUMBER,
        MATRIS,
        FUNCTION,
        ARGSEPERATOR,
        OPERATOR,
        LEFTBRACE,
        RIGHTBRACE,
        DOCS,
        OUTPUT,
        ERROR,
        VOID
    };

    // Operator order
    public enum OperatorAssociativity { LEFT, RIGHT };

    public class CompilerMessage
    {
        // LIMITS
        public static string MAT_LIMIT = "Matris limitine(=" + (int)MatrisLimits.forMatrisCount + ") ulaşıldı, atama işlemi yapmak için bir matrisi siliniz! ";

        // MATRIX ERRORS
        public const string MAT_INVALID_SIZE = "Matris boyutu hatalı!";
        public static string MAT_UNEXPECTED_DIMENSIONS(string expected, string got)
        {
            return "Beklenen boyut: " + expected + ", Alınan boyut: " + got;
        }

        public static string MAT_UNEXPECTED_COLUMN_SIZE(string expected, string got)
        {
            return "Sütün sayısı " + expected + "olmalı (" + got + " verildi)";
        }

        public static string MAT_UNEXPECTED_ROW_SIZE(string expected, string got)
        {
            return "Satır sayısı " + expected + "olmalı (" + got + " verildi)";
        }

        // DOCS
        public const string DOCS_HELP = "'?' kullanımı ile ilgili bilgi için '?' komutunu kullanın.";
        public const string COMPILER_HELP = ">>> '?' bir ön-ektir. Bilgi almak istediğiniz terimden önce '?' koyunuz (?komut)" +
                             "\n>>> Fonksiyonları kullanmak için '!' ön-ekini fonksiyon isminden önce koyunuz (!komut)" +
                             "\n>>> Çıktılar veya menülerin üzerine işaretçinizi bekleterek gerekli bilgilere ulaşabilirsiniz." +
                             "\n>>> Özel aritmetik operatörler (A,B: Matris, n: tam sayı):" +
                             "\n\t    '.*' : Matris çarpımı = A .* B" +
                             "\n\t    '.^' : Matrisin kendisiyle matris çarpımı = A .^ n == A .* A .* A ... .* A" +
                             "\n\t    './' : 2. matrisin tersi ile matris çarpımı = A ./ B == A .* !Inverse(B)";

        // ARGUMENTS
        public const string ARG_COUNT_ERROR = "Argüman sayısı hatalı!";

        // COMMAND FORMATS
        public const string CMD_FORMAT_ERROR = "Hatalı komut formatı!";
        public const string PARANTHESIS_FORMAT_ERROR = "Parantez formatı hatalı!";
        public const string PARANTHESIS_COUNT_ERROR = "Parametre sayısı hatalı!";

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

        // MATRIX NAME INVALID
        public static string INVALID_MAT_NAME(string name)
        {
            return "Matris ismi " + name + " olamaz.";
        }

        // UNEXPECTED OBJECT OR NAME
        public static string NOT_A_(string name, string expected)
        {
            return "'" + name + "' bir " + expected + " değil!";
        }

        public static string NOT_SAVED_MATRIX(string name)
        {
            return "'" + name + "' adlı bir matris bulunamadı!";
        }

        // INVALID OPERANDS
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

        // FUNCTION ERRORS
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
            return val + " değeri " + parseType + " olarak kullanılamadı!";
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
            return name + " adlı bir parametre bulunamadı!";
        }

        public static string STRING_FORMAT_INVALID(string format)
        {
            return "Argüman formatı hatalı, format: " + format + " olmalı!";
        }
    };

}
