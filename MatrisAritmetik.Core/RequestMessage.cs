using System;

namespace MatrisAritmetik.Core
{
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

        /// <summary>
        /// Given request was being spammed / was too close to previous command
        /// </summary>
        /// <param name="last">Last date recieved</param>
        /// <returns>Message telling how long to wait</returns>
        public static string REQUEST_SPAM(DateTime last)
        {
            return "Yeni bir komut göndermek için " + Math.Round(((int)CompilerLimits.forCmdSendRateInSeconds - (DateTime.Now - last).TotalSeconds), 2) + " saniye bekleyiniz!";
        }
    }
}
