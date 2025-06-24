namespace SchedulingModule.Domain.Enums;

public class ScheduleTypeEnum
{
    public enum Enum_ScheduleType
    {
        Daily,
        Weekly,
        DateWise,
        Custom

    }
    public enum Enum_ScheduleSubType
    {
        Everyday,
        Every,
        Selecteddays,
        Weekdays,
        Weekenddays,
        Custom

    }
}