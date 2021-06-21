using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace QuietTime.Models
{
    class Schedule : ObservableObject
    {
        private readonly static List<Schedule> Schedules = new();

        public IEnumerable<Schedule> GetSchedules()
        {
            var list = new List<Schedule>();
            list.AddRange(Schedules);
            return list;
        }

        private DateTime _start;
        public DateTime Start
        {
            get { return _start; }
            set { SetProperty(ref _start, value); }
        }

        private DateTime _end;
        public DateTime End
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

        public Schedule(DateTime start, DateTime end, int volumeDuring, int volumeAfter)
        {
            _start = start;
            _end = end;
            _volumeDuring = volumeDuring;
            _volumeAfter = volumeAfter;
        }
    }
}
