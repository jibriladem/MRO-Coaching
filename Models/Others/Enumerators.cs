using System.Collections.Generic;
using System.ComponentModel;

namespace MROCoatching.DataObjects.Models.Others
{
    public enum RecordStatus { Active = 1, Inactive, Deleted }
    public enum EmployeeSubgroup { Management = 1, NonManagement }
    public enum Quarter { Q1 = 1, Q2, Q3, Q4 }


    public enum TimelineType
    {
        Duration = 1,
        Interval
    }
}
