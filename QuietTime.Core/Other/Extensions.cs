using System;
using QuietTime.Core.Models;

namespace QuietTime.Core.Other
{
    public static class Extensions
    {
        public static int ToPercentage(this float val)
        {
            val = Math.Clamp(val, 0, 1);

            val *= 100;
            return (int)val;
        }

        public static bool Overlaps(this Schedule x, Schedule y)
        {
            if (x is null || y is null) return false;

            return x.Start <= y.End && y.Start <= x.Start;
        }
    }
}
