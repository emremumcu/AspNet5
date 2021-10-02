using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet5.AppLib.Tools
{
    public static class Utilities
    {
        // DateTime.UnixEpoch is the same as new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixTime(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(DateTime dt)
        {
            
            Int32 unixTimestamp = (Int32)(dt.Subtract(epoch)).TotalSeconds;
            return unixTimestamp;
        }
    }
}
