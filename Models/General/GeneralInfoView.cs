using MROCoatching.DataObjects.Data.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MROCoatching.DataObjects.Models.General
{
    public class GeneralInfoView
    {
        public ICollection<Detailinfo> Detailinfos { get; set; }
        public ICollection<Taskcode> Taskcodes { get; set; }
        public ICollection<Plusers> Plusers { get; set; }
        public GeneralInfoView()
        {
        }
        [NotMapped]
        public ApplicationDbContext _dbContext { get; set; }

        public GeneralInfoView(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
