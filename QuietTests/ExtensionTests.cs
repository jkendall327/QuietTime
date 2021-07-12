using System;
using FluentAssertions;
using QuietTime.Core.Models;
using QuietTime.Core.Other;
using Xunit;

namespace QuietTests
{
    public class ExtensionTests
    {
        [InlineData(-1, 0)]
        [InlineData(2, 100)]
        [Theory]
        public void ToPercentage_ShouldNormalizeValues_WhenBelowZeroOrAboveOne(float value, int expected)
        {
            var actual = Extensions.ToPercentage(value);

            actual.Should().Be(expected);
        }

        [Fact]
        public void Overlaps_ShouldReturnTrue_WhenSchedulesAreIdentical()
        {
            var s1 = new Schedule(TimeOnly.MinValue, TimeOnly.MinValue, 0, 0);
            var s2 = s1;

            s1.Overlaps(s2).Should().BeTrue();
        }

        [Fact]
        public void Overlaps_ShouldReturnFalse_WhenParameterIsNull()
        {
            Schedule s1 = Schedule.MinValues;
            Schedule s2 = null;

            s1.Overlaps(s2).Should().BeFalse();
        }

        [Fact]
        public void Overlaps_ShouldReturnFalse_WhenSchedulesDoNotOverlap()
        {
            Schedule s1 = Schedule.MinValues;
            s1.Start = new(9, 0); s1.End = new(12, 0);

            Schedule s2 = Schedule.MinValues;
            s2.Start = new(13, 0); s2.End = new(17, 0);

            s1.Overlaps(s2).Should().BeFalse();
        }

        [Fact]
        public void Overlaps_ShouldReturnTrue_WhenSchedulesOverlap()
        {
            Schedule s1 = Schedule.MinValues;
            s1.Start = new(9, 0); s1.End = new(12, 0);

            Schedule s2 = Schedule.MinValues;
            s2.Start = new(11, 0); s2.End = new(17, 0);

            s1.Overlaps(s2).Should().BeTrue();
        }

        [Fact]
        public void Overlaps_ShouldReturnTrue_WhenSchedulesOverlapOverDayBoundary()
        {
            Schedule s1 = Schedule.MinValues;
            s1.Start = new(1, 0); s1.End = new(5, 0);

            Schedule s2 = Schedule.MinValues;
            s2.Start = new(22, 0); s2.End = new(3, 0);

            s1.Overlaps(s2).Should().BeTrue();
        }
    }
}
