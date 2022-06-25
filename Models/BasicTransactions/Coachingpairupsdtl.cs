using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MROCoatching.DataObjects.Models.BasicTransactions
{
    [Table("COACHINGPAIRUPSDTL")]
    public class Coachingpairupsdtl : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Ids { get; set; }
        [Display(Name = "Employee Id"), Required, StringLength(10)]
        public string EmployeeId { get; set; }
        [Display(Name = "Coach ID"), Required, StringLength(10)]
        public string Coachid { get; set; }
        [Display(Name = "Coach Name"), Required, StringLength(100)]
        public string Coachname { get; set; }
        [Display(Name = "Cost Center"), Required, StringLength(10)]
        public string Costcenter1 { get; set; }
        [NotMapped]
        public string Coscentername1 { get; set; }
        [Display(Name = "Assessor ID"), Required, StringLength(10)]
        public string Assessorid { get; set; }
        [Display(Name = "Assessor Name"), Required, StringLength(100)]
        public string Assessorname { get; set; }
        [Display(Name = "Cost Center"), Required, StringLength(10)]
        public string Costcenter2 { get; set; }
        [NotMapped]
        public string Coscentername2 { get; set; }
        [Display(Name = "Position Id"), Required, StringLength(10)]
        public string Position { get; set; }
        [Display(Name = "PL Name"), Required, StringLength(50)]
        public string Plname { get; set; }
        [Display(Name = "PL Level"), Required, StringLength(50)]
        public string Pllevel { get; set; }
        [Display(Name = "Status"), StringLength(5)]
        public string Status { get; set; }
        public Coachingpairupsdtl()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public Coachingpairupsdtl(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
    }
}
