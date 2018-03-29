// Version		: 001
// Designer		: ChienBV
// Date			: 2018/01/23
// Comment		: Create new

namespace ShipOnline.UtilityService
{
    using ShipOnline.Resources;
    using System;
    using System.Resources;

    public static partial class Utility
    {
        public static DateTime ConvertDateTimeToVN(DateTime target)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(target.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
        }

        /// <summary>
        /// Get current Date and Time
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentDateTime()
        {
            return ConvertDateTimeToVN(DateTime.Now);
        }

        /// <summary>
        /// Get current date only
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentDateOnly()
        {
            return ConvertDateTimeToVN(DateTime.Now).Date;
        }

    }
}
