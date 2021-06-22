using QuietTime.Models;

namespace QuietTime.Other
{
    internal static class Extensions
    {
        internal static int ToPercentage(this float val)
        {
            val *= 100;
            return (int)val;
        }

        public static bool Overlaps(this Schedule x, Schedule y)
        {
            if (x.Start < y.Start && x.End > y.Start)
            {
                return true;
            }
            else if (x.Start < y.End && x.End < y.End)
            {
                return true;
            }

            return false;
        }
    }
}
