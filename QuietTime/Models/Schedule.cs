using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quartz;
using QuietTime.Other;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuietTime.Models
{
    /// <summary>
    /// Represents a a period during which the system audio will be capped to a specific volume.
    /// </summary>
    public class Schedule : ObservableObject
    {
        /// <summary>
        /// Uniquely identifies this schedule in the scheduling system.
        /// </summary>
        public JobKey? Key { get; set; }

        /// <summary>
        /// Whether the schedule is currently firing.
        /// </summary>
        public bool IsActive 
        { 
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }
        private bool _isActive;

        /// <summary>
        /// When this schedule will start.
        /// </summary>
        public TimeOnly Start
        {
            get { return _start; }
            set { SetProperty(ref _start, value); }
        }
        private TimeOnly _start;

        /// <summary>
        /// When this schedule will end.
        /// </summary>
        public TimeOnly End
        {
            get { return _end; }
            set { SetProperty(ref _end, value); }
        }
        private TimeOnly _end;

        /// <summary>
        /// The total duration of this schedule.
        /// </summary>
        public TimeSpan Length => End - Start;

        /// <summary>
        /// The maximum volume allowed while this schedule is active.
        /// </summary>
        public int VolumeDuring
        {
            get { return _volumeDuring; }
            set { SetProperty(ref _volumeDuring, value); }
        }
        private int _volumeDuring;

        /// <summary>
        /// What the maximum volume will be set to when this schedule ends.
        /// </summary>
        public int VolumeAfter
        {
            get { return _volumeAfter; }
            set { SetProperty(ref _volumeAfter, value); }
        }
        private int _volumeAfter;

        /// <summary>
        /// Creates a new schedule.
        /// </summary>
        /// <param name="start">When the schedule will start.</param>
        /// <param name="end">When the schedule will end.</param>
        /// <param name="volumeDuring">The maximum volume allowed while this schedule is active.</param>
        /// <param name="volumeAfter">What the maximum volume will be set to when this schedule ends.</param>
        public Schedule(TimeOnly start, TimeOnly end, int volumeDuring, int volumeAfter)
        {
            _start = start;
            _end = end;
            _volumeDuring = volumeDuring;
            _volumeAfter = volumeAfter;
        }
    }
}
