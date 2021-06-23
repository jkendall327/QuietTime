using QuietTime.Models;
using System;

namespace QuietTime.Other
{
    internal static class Extensions
    {
        internal static int ToPercentage(this float val)
        {
            val = Math.Clamp(val, 0, 1);

            val *= 100;
            return (int)val;
        }

        internal static bool Overlaps(this Schedule x, Schedule y)
        {
            if (x is null || y is null) return false;

            return x.Start <= y.End && y.Start <= x.Start;
        }
    }
}
