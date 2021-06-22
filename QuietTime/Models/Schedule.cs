using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quartz;
using QuietTime.Other;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuietTime.Models
{
    public class Schedule : ObservableObject
    {
        public JobKey? Key;

        /// <summary>
        /// Whether the schedule is currently firing.
        /// </summary>
        public bool IsActive 
        { 
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }
        private bool _isActive;

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
