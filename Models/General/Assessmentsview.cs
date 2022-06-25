using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.BasicTransactions;
using MROCoatching.DataObjects.Models.CoachingTable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MROCoatching.DataObjects.Models.General
{
    public class Assessmentsview
    {
        public ICollection<Items> Items { get; set; }
        public ICollection<Items> Item { get; set; }
        public ICollection<Coaching> Coaching { get; set; }
        public ICollection<Coachingpairupshdr> Coachingpairupshdrs { get; set; }
        public ICollection<Coachingpairupsdtl> Coachingpairupsdtls { get; set; }
        public ICollection<Plusers> Plusers { get; set; }
        public Assessmentsview()
        {
        }
        [NotMapped]
        public ApplicationDbContext _dbContext { get; set; }

        public Assessmentsview(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
