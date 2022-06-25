using MROCoatching.DataObjects.Models.Others;
using System;
using System.ComponentModel.DataAnnotations;

namespace MROCoatching.DataObjects.Models.AccountMaster
{
    public class Audit
    {
        [Display(Name = "Who Created"), StringLength(100)]
        public string ERNAM { get; set; }
        [Display(Name = "Date Created")]
        public DateTime? ERDAT { get; set; }
        [Display(Name = "Who Changed"), StringLength(100)]
        public string AENAM { get; set; }
        [Display(Name = "Date Changed")]
        public DateTime? AEDAT { get; set; }
        [Display(Name = "Active")]
        public RecordStatus ACTIND { get; set; }
    }
}
