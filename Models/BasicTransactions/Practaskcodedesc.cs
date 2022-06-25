using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MROCoatching.DataObjects.Models.BasicTransactions
{
    public class Practaskcodedesc : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Taskode { get; set; }
        public string Taskdesc { get; set; }
        public long? Taskcount { get; set; }
        public string Plcode { get; set; }
        public string Employeeid { get; set; }
        public Practaskcodedesc()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public Practaskcodedesc(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
    }
}
