using MROCoatching.DataObjects.Models.AccountMaster;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MROCoatching.DataObjects.Models.MasterTables
{
    public class MyTeams: Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string EmployeeId { get; set; }
    }
}
