using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuietTime.Models
{
    public class Schedule : ObservableObject
    {
        private readonly static List<Schedule> Schedules = new();

        public static IEnumerable<Schedule> GetSchedules()
        {
            var list = new List<Schedule>();
            list.AddRange(Schedules);

            list.Add(new Schedule(TimeOnly.Parse("09:00"), TimeOnly.Parse("17:45"), 20, 40));
            list.Add(new Schedule(TimeOnly.Parse("11:00"), TimeOnly.Parse("15:00"), 10, 50));
            list.Add(new Schedule(TimeOnly.Parse("03:00"), TimeOnly.Parse("12:00"), 23, 70));

            return list;
        }

        public static void AddSchedule(Schedule schedule)
        {
            if (Schedules.All(x => !Overlaps(x, schedule)))
            {
                Schedules.Add(schedule);
            }
        }

        private static bool Overlaps(Schedule x, Schedule y)
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

        private TimeOnly _start;
        public TimeOnly Start
        {
            get { return _start; }
            set { SetProperty(ref _start, value); }
        }

        private TimeOnly _end;
        public TimeOnly End
        {
            get { return _end; }
            set { SetProperty(ref _end, value); }
        }

        public TimeSpan Length => End - Start;

        private int _volumeDuring;
        public int VolumeDuring
        {
            get { return _volumeDuring; }
            set { SetProperty(ref _volumeDuring, value); }
        }

        private int _volumeAfter;
        public int VolumeAfter
        {
            get { return _volumeAfter; }
            set { SetProperty(ref _volumeAfter, value); }
        }

        public Schedule(TimeOnly start, TimeOnly end, int volumeDuring, int volumeAfter)
        {
            _start = start;
            _end = end;
            _volumeDuring = volumeDuring;
            _volumeAfter = volumeAfter;
        }
    }
}
