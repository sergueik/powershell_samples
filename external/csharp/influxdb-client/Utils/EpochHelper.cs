using System;

namespace Utils
{
    internal static class EpochHelper
    {
        private static readonly DateTime Origin = new DateTime(1970, 1, 1);

        public static long ToEpoch(this DateTime time)
        {
            TimeSpan t = time - Origin;
            return (long) (t.TotalSeconds*1000);
        }
    }
}