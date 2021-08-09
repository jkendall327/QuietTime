using System;

namespace QuietTime.Core.AudioLocking
{
    public static class AudioLockerExtensions
    {
        public static int ToPercentage(this float val)
        {
            val = Math.Clamp(val, 0, 1);

            val *= 100;
            return (int)val;
        }
    }
}
