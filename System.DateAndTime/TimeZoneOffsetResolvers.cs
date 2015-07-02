using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public delegate DateTimeOffset TimeZoneOffsetResolver(DateTime dateTime, TimeZoneInfo timeZone);

    public static class TimeZoneOffsetResolvers
    {
        public static DateTimeOffset Default(DateTime dt, TimeZoneInfo timeZone)
        {
            if (timeZone.IsAmbiguousTime(dt))
            {
                var earlierOffset = timeZone.GetUtcOffset(dt.AddDays(-1));
                return new DateTimeOffset(dt, earlierOffset);
            }

            if (timeZone.IsInvalidTime(dt))
            {
                var earlierOffset = timeZone.GetUtcOffset(dt.AddDays(-1));
                var laterOffset = timeZone.GetUtcOffset(dt.AddDays(1));
                var transitionGap = laterOffset - earlierOffset;
                return new DateTimeOffset(dt.Add(transitionGap), laterOffset);
            }

            return new DateTimeOffset(dt, timeZone.GetUtcOffset(dt));
        }

        // TODO: include other kinds of resolvers
    }
}
