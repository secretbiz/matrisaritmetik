namespace MatrisAritmetik.Core
{

    /// <summary>
    /// Const strings and static string methods for storing custom command state messages
    /// </summary>
    public static class CommandStateMessage
    {
        // CLEANUP
        public const string SUCCESS_CLEANUP = "Komut geçmişi temizlendi";

        // SUCCESSFUL RETURNS
        public const string SUCCESS_RET_NULL = "";
        public const string SUCCESS_RET_MAT = "";
        public const string SUCCESS_RET_NUM = "";

        // DOCS
        public const string SUCCESS_COMPILER_DOCS = "Derleyici kullanma bilgisi alındı";

        public static string DOCS_MAT_FOUND(string name)
        {
            return "Matris " + name + " hakkında bilgi alındı";
        }

        public static string DOCS_FUNC_FOUND(string name)
        {
            return "Fonksiyon " + name + " hakkında bilgi alındı";
        }

        public static string DOCS_SPECIAL_FOUND(string name)
        {
            return "Özel değer " + name + " hakkında bilgi alındı";
        }

        public static string DOCS_MAT_FUNC_FOUND(string name)
        {
            return "Matris ve komut olan " + name + " hakkında bilgi alındı";
        }

        public static string DOCS_MAT_SPECIAL_FOUND(string name)
        {
            return "Matris ve özel değer " + name + " hakkında bilgi alındı";
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
}
