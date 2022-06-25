using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace MROCoatching.DataObjects.Models.AssessingTable
{
    [Table("Assessmentresult")]// Assessment Result
    public class Assessmentresult : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Display(Name = "PL Code"), StringLength(50), Required]
        public string Plcode { get; set; }
        [Display(Name = "Employee Id"), StringLength(255), Required]
        public string EmployeeId { get; set; }
        [Display(Name = "T. Sequence Number"), StringLength(10), Required]
        public string Trainseqnbr { get; set; }
        [Display(Name = "Re Assessment"), StringLength(5), Required]
        public string Reassessment { get; set; }
        [Display(Name = "Category Code"), StringLength(25), Required]
        public string Asscatgcode { get; set; }
        public long? Asstypseqnbr { get; set; }
        [Display(Name = "A. Sequence Number")]
        public long? Assquesseq { get; set; }
        [Display(Name = "Assessment Questions"), StringLength(255)]
        public string Assquestions { get; set; }
        [Display(Name = "Assessment Result"), StringLength(15)]
        public string Assresults { get; set; }
        public string Remarks { get; set; }
        [Display(Name = "Status"), StringLength(5)]
        public string Status { get; set; }
        [NotMapped]
        public bool checkStatus { get; set; }
        [NotMapped, StringLength(10)]
        public string Status1 { get; set; }
        public Assessmentresult()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }

        private readonly ApplicationDbContext _dbcontext;
        public Assessmentresult(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbcontext.Asstypes.Add(this);
                if (await _dbcontext.SaveChangesAsync() > 0)
                {
                    return new DatabaseOperationResponse
                    {
                        Status = MROCoatching.DataObjects.Models.Others.OperationStatus.SUCCESS,
                        Message = Resource.RecordSucessfullyCreated,
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
        public async Task<object> SaveList(List<Assessmentresult> asstypes)
        {
            try
            {
                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return new DatabaseOperationResponse
                    {
                        Status = MROCoatching.DataObjects.Models.Others.OperationStatus.SUCCESS,
                        Message = Resource.RecordSucessfullyCreated,
                    };
                }

                return new DatabaseOperationResponse
                {

                    Message = Resource.OperationWasNotSucessful,
                    Status = MROCoatching.DataObjects.Models.Others.OperationStatus.Ok
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
        public async Task<object> Update()
        {
            try
            {
                //this.AEDAT = DateTime.Now;
                _dbcontext.Asstypes.Attach(this);
                _dbcontext.Entry(this).State = EntityState.Modified;
                if (await _dbcontext.SaveChangesAsync() > 0)
                {
                    return new DatabaseOperationResponse
                    {
                        Status = Others.OperationStatus.SUCCESS,
                        Message = Resource.RecordSucessfullyUpdated,
                    };
                }
                else
                {
                    return new DatabaseOperationResponse
                    {
                        Message = Resource.OperationWasNotSucessful,
                        Status = Others.OperationStatus.Ok
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                return new DatabaseOperationResponse
                {
                    ex = ex,
                    Message = Resource.ErrorOccuredWhileUpdatingRecord,
                    Status = Others.OperationStatus.ERROR
                };
            }
        }
        public async Task<object> Delete()
        {
            try
            {
                _dbcontext.Remove(this);
                if (await _dbcontext.SaveChangesAsync() > 0)
                {
                    return new DatabaseOperationResponse
                    {
                        Status = Others.OperationStatus.SUCCESS,
                        Message = Resource.RecordSucessfullyDeleted,
                    };
                }
                else
                {
                    return new DatabaseOperationResponse
                    {
                        Message = Resource.OperationWasNotSucessful,
                        Status = Others.OperationStatus.Ok
                    };
                }
            }
            catch (Exception ex)
            {
                return new DatabaseOperationResponse
                {
                    ex = ex,
                    Message = Resource.ErrorOccuredWhileDeletingRecord,
                    Status = Others.OperationStatus.ERROR
                };
            }
        }
        public async Task<List<object>> GetList()
        {
            try
            {
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<object> Refresh()
        {
            try
            {
                return await _dbContext.Asstypes.FirstOrDefaultAsync(con => con.Plcode == Plcode && con.EmployeeId == EmployeeId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> Exist()
        {
            try
            {
                var regstr = await _dbContext.Asstypes.FirstOrDefaultAsync(con => con.Plcode == Plcode && con.EmployeeId == EmployeeId);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

    }
}
