using System;

namespace CCMS.Application.Helpers
{
    public static class CaseNumberGenerator
    {
        private static int _counter = 1;

        public static string Generate()
        {
            var dateStr = DateTime.UtcNow.ToString("yyyyMMdd");
            var sequence = _counter.ToString("D4");
            _counter++;
            return $"CCMS-{dateStr}-{sequence}";
        }
    }
}
