using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace QuietTime.Models
{
    internal class ScheduleDTO
    {
        public string? KeyGroup { get; private set; }
        public string? KeyName { get; private set; }

        public string Start { get; set;}
        public string End { get; set; }

        public int VolumeDuring { get; set; }
        public int VolumeAfter { get; set; }

        [JsonConstructor]
        public ScheduleDTO()
        {

        }

        public ScheduleDTO(Schedule schedule)
        {
            KeyGroup = schedule.Key?.Group;
            KeyName = schedule.Key?.Name;

            Start = schedule.Start.ToString();
            End = schedule.End.ToString();

            VolumeDuring = schedule.VolumeDuring;
            VolumeAfter = schedule.VolumeAfter;
        }

        public Schedule ToSchedule()
        {
            var ret = new Schedule(
                TimeOnly.Parse(Start),
                TimeOnly.Parse(End),
                VolumeDuring,
                VolumeAfter);

            if (KeyName is not null && KeyGroup is not null)
            {
                ret.Key = new(KeyName, KeyGroup);
            }

            return ret;
        }
    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

}
