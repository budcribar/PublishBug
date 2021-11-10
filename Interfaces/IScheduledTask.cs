using System.Collections.Generic;

namespace Interfaces
{
    public interface IScheduledTaskTimeTrigger
    {
        string StartDay { get; set; } // YYY-MM-DD, 2018-01-30
        string StartTime { get; set; } // HH:mm:ss, 22:30:00
    }

    public interface IScheduledTaskDailyTrigger : IScheduledTaskTimeTrigger
    {
        short DaysInterval { get; set; }
    }

    public interface IScheduledTaskWeeklyTrigger : IScheduledTaskTimeTrigger
    {
        short WeeksInterval { get; set; }
        short DaysOfWeek { get; set; }
    }

    public interface IScheduledTaskMonthlyTrigger : IScheduledTaskTimeTrigger
    {
        IEnumerable<int> DaysOfMonth { get; set; }
    }

    public interface IScheduledTaskEventTrigger : IScheduledTaskTimeTrigger
    {
        string Log { get; set; }
        string Source { get; set; }
        int EventId { get; set; }
    }

    public interface IScheduledTask
    {
        string Id { get; set; }
        string Url { get; set; }
        string Description { get; set; }
        string NextRunTime { get; set; }
        bool Enabled { get; set; }
        IEnumerable<string> TestIds { get; set; }
        IEnumerable<IScheduledTaskTimeTrigger> OneTimeTriggers { get; set; }
        IEnumerable<IScheduledTaskDailyTrigger> DailyTriggers { get; set; }
        IEnumerable<IScheduledTaskWeeklyTrigger> WeeklyTriggers { get; set; }
        IEnumerable<IScheduledTaskMonthlyTrigger> MonthlyTriggers { get; set; }
        IEnumerable<IScheduledTaskEventTrigger> EventTriggers { get; set; }
        IEnumerable<string> EventRecords { get; set; }
    }
}
