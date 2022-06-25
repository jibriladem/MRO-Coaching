using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MROCoatching.DataObjects.Models.BasicTransactions
{
    public class Pracchapterdesc : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Referencenbr { get; set; }
        public string Referencedesc { get; set; }
        public string Plcode { get; set; }
        public string Employeeid { get; set; }
        public Pracchapterdesc()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public Pracchapterdesc(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
    }
}
