using MROCoatching.DataObjects.Models.AccountMaster;
using System;

namespace MROCoatching.DataObjects.Models.Others
{
    public class AuditLog : Audit
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
