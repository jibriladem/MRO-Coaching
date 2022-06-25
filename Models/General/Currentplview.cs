using MROCoatching.DataObjects.Data.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MROCoatching.DataObjects.Models.General
{
    public class Currentplview
    {
        public ICollection<Costcenters> Costcenters { get; set; }
        public ICollection<Positions> Positions { get; set; }
        public Currentplview()
        {

        }
        [NotMapped]
        public ApplicationDbContext _dbContext { get; set; }

        public Currentplview(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
