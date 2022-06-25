using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MROCoatching.DataObjects.Models.BasicTransactions
{
    public class Pracmaintcddesc : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Mntcode { get; set; }
        public string Mntdesc { get; set; }
        public string Plcode { get; set; }
        public string Employeeid { get; set; }
        public Pracmaintcddesc()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public Pracmaintcddesc(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
    }
}
