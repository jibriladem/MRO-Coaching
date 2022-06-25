using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace MROCoatching.DataObjects.Models.BasicTransactions
{
    public class Practicalassessments : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Display(Name = "PL Code"), Required, StringLength(15)]
        public string Plcode { get; set; }//can get PL Desc and Level from Master
        [Display(Name = "Trainee ID"), Required, StringLength(15)]
        public string Employeeid { get; set; }//Can get Name and Cost Centers from Master
        public int Trnseqnumber { get; set; }//How many Items Assessed
        public string Taskgrpcode { get; set; }//List of Task Codes -> Has Details 
        public string Taskcount { get; set; }
        public string Refgrpcode { get; set; }//ATA Chapter List -> Has Details
        public string Mntgrpcode { get; set; }//TSFN List -> Has Details
        public string Assessorid { get; set; }//Assessessor ID
        public string Assessorname { get; set; }//Assessor Name
        public DateTime Dateassessed { get; set; }//Date of Assessment (List)

        public Practicalassessments()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public Practicalassessments(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save(Practicalassessments practicalassessments, Pracchapterdesc pracchapterdesc, Practaskcodedesc practaskcodedesc, Pracmaintcddesc pracmaintcddesc, Coaching getcoaching)
        {
            try
            {
                if (practicalassessments != null)
                {
                    if (practicalassessments.Id != 0)
                    {
                        _dbContext.Practicalassessments.Attach(practicalassessments);
                        _dbContext.Entry(practicalassessments).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.Practicalassessments.Add(practicalassessments);
                        _dbContext.Entry(practicalassessments).State = EntityState.Added;
                    }
                }
                if (practaskcodedesc != null)
                {
                    if (practaskcodedesc.Id != 0)
                    {
                        _dbContext.Practaskcodedesc.Attach(practaskcodedesc);
                        _dbContext.Entry(practaskcodedesc).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.Practaskcodedesc.Add(practaskcodedesc);
                        _dbContext.Entry(practaskcodedesc).State = EntityState.Added;
                    }
                }
                if (pracmaintcddesc != null)
                {
                    if (pracmaintcddesc.Id != 0)
                    {
                        _dbContext.Pracmaintcddesc.Attach(pracmaintcddesc);
                        _dbContext.Entry(pracmaintcddesc).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.Pracmaintcddesc.Add(pracmaintcddesc);
                        _dbContext.Entry(pracmaintcddesc).State = EntityState.Added;
                    }
                }
                if (pracchapterdesc != null)
                {
                    if (pracchapterdesc.Id != 0)
                    {
                        _dbContext.Pracchapterdesc.Attach(pracchapterdesc);
                        _dbContext.Entry(pracchapterdesc).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.Pracchapterdesc.Add(pracchapterdesc);
                        _dbContext.Entry(pracchapterdesc).State = EntityState.Added;
                    }
                }
                // _dbContext.Practicalassessments.Add(this);
                if (getcoaching != null)
                {
                    _dbContext.Coaching.Attach(getcoaching);
                    _dbContext.Entry(getcoaching).State = EntityState.Modified;
                }
                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return new DatabaseOperationResponse
                    {
                        Status = MROCoatching.DataObjects.Models.Others.OperationStatus.Approve,
                        Message = Resource.RecordSucessfullyApproved,
                    };
                }

                return new DatabaseOperationResponse
                {
                    Status = MROCoatching.DataObjects.Models.Others.OperationStatus.Ok,
                    Message = Resource.OperationWasNotSucessful,
                };
            }
            catch (Exception ex)
            {
                return new DatabaseOperationResponse
                {
                    ex = ex,
                    Message = Resource.ErrorOccuredWhileCreatingRecord,
                    Status = MROCoatching.DataObjects.Models.Others.OperationStatus.ERROR
                };
            }
        }
    }
}
