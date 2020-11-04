using System;
using System.Collections.Generic;
using System.Text;

/* This file is for managing smaller classes, enumerators etc.
 * Created to reduce file amount.
 */
namespace MatrisAritmetik.Core
{
    // ENUM CLASSES
    // Command's states
    public enum CommandState { IDLE, UNAVAILABLE, SUCCESS, WARNING, ERROR };

    // For limiting matrix creation
    public enum MatrisLimits { forRows = 128, forCols = 128, forSize = 128 * 128, forMatrisCount = 8, forName = 64 };

    // Token types
    public enum TokenType { NULL, NUMBER, MATRIS, FUNCTION, ARGSEPERATOR, OPERATOR, LEFTBRACE, RIGHTBRACE, DOCS, DEFAULT };

    // Operator order
    public enum OperatorAssociativity { LEFT, RIGHT };

    public class Custom
    {
        public static Dictionary<string, string> Messages = new Dictionary<string, string>
        {
            {"console_info", ">>> '?' bir ön-ektir. Bilgi almak istediğiniz terimden önce '?' koyunuz (?komut)" +
                             "\n>>> Fonksiyonları kullanmak için '!' ön-ekini fonksiyon isminden önce koyunuz (!komut)"+
                             "\n>>> Çıktılar veya menülerin üzerine işaretçinizi bekleterek gerekli bilgilere ulaşabilirsiniz."+
                             "\n>>> Özel aritmetik operatörler (A,B: Matris, n: tam sayı):"+
                             "\n\t    '.*' : Matris çarpımı = A .* B"+
                             "\n\t    '.^' : Matrisin kendisiyle matris çarpımı = A .^ n == A .* A .* A ... .* A"+
                             "\n\t    './' : 2. matrisin tersi ile matris çarpımı = A ./ B == A .* !Inverse(B)"}
        };
    }
}
