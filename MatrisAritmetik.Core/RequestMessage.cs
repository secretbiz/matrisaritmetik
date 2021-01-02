using System;

namespace MatrisAritmetik.Core
{
    /// <summary>
    /// Messages related to request warnings and errors
    /// </summary>
    public static class RequestMessage
    {
        /// <summary>
        /// Given request didn't have required keys
        /// </summary>
        /// <param name="requestDesc">Short name to use for the request</param>
        /// <param name="keys">Expected keys</param>
        /// <returns>Message telling required keys</returns>
        public static string REQUEST_MISSING_KEYS(string requestDesc, params string[] keys)
        {
            return requestDesc + " isteği başarısız! Gerekli parametrelere değer verilmedi: " + string.Join(",", keys);
        }

        /// <summary>
        /// Given request was being spammed / was too close to previous command
        /// </summary>
        /// <param name="last">Last date recieved</param>
        /// <returns>Message telling how long to wait</returns>
        public static string REQUEST_SPAM(DateTime last)
        {
            return "Yeni bir komut göndermek için " + Math.Round((int)CompilerLimits.forCmdSendRateInSeconds - (DateTime.Now - last).TotalSeconds, 2) + " saniye bekleyiniz!";
        }

        /// <summary>
        ///  Invalid argument <paramref name="val"/> for given parameter <paramref name="para"/>
        /// </summary>
        /// <param name="para">Parameter name</param>
        /// <param name="val">Argument</param>
        /// <returns>Message telling given argument was invalid</returns>
        public static string REQUEST_PARAM_INVALID(string para, string val = "")
        {
            return para + " için geçersiz değer" + (string.IsNullOrWhiteSpace(val) ? "." : ": " + val);
        }
    }
}
