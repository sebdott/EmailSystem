using System;
using System.Collections.Generic;
using System.Text;

namespace EmailingSystem.Cron
{
    class CronSettings
    {
        public string InitiateSetStatusProcessURL { get; set; }
        public int InitiateSetStatusProcessRepeatNoOfDays { get; set; }
        public int InitiateSetStatusProcessRepeatHour { get; set; }
        public int InitiateSetStatusProcessRepeatMin { get; set; }
        public string InitiateSendEmailProcessURL { get; set; }
        public int InitiateSendEmailProcessRepeatNoOfDays { get; set; }
        public int InitiateSendEmailProcessRepeatHour { get; set; }
        public int InitiateSendEmailProcessRepeatMin { get; set; }
    }
}
