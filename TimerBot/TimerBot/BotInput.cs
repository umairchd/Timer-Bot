using System;
using CsvHelper.Configuration.Attributes;

namespace TimerBot
{
    public class BotInput
    {
        [Index(0)]
        public DateTime StartDate { get; set; }
        [Index(1)]
        public TimeSpan StartTime { get; set; }
        [Index(2)]
        public DateTime EndDate { get; set; }
        [Index(3)]
        public TimeSpan EndTime { get; set; }
        [Index(4)]
        public int Interval { get; set; }
        [Index(5)]
        public bool IsPast { get; set; }
        [Index(6)]
        public int Clicks { get; set; }
        [Index(7)]
        public bool KillSelf { get; set; }

    }
}