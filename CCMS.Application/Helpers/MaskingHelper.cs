using System;

namespace CCMS.Application.Helpers
{
    public static class MaskingHelper
    {
        public static string MaskPan(string pan)
        {
            if (string.IsNullOrWhiteSpace(pan) || pan.Length < 10) return pan;
            return pan.Substring(0, 5) + "****" + pan.Substring(9);
        }

        public static string MaskAadhaar(string aadhaar)
        {
            if (string.IsNullOrWhiteSpace(aadhaar) || aadhaar.Length <= 4) return aadhaar;
            return new string('*', aadhaar.Length - 4) + aadhaar.Substring(aadhaar.Length - 4);
        }

        public static string MaskAccountNumber(string account)
        {
            if (string.IsNullOrWhiteSpace(account) || account.Length <= 4) return account;
            return new string('*', account.Length - 4) + account.Substring(account.Length - 4);
        }
    }
}
