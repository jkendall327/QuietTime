namespace QuietTime.Other
{
    internal static class Extensions
    {
        internal static int ToPercentage(this float val)
        {
            val *= 100;
            return (int)val;
        }
    }
}
