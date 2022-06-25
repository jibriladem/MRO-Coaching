using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace MROCoatching.DataObjects.Models.AssessingTable
{
    [Table("Assquestionlimits")]// Assessment Question Limits
    public class Assquestionlimits : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Display(Name = "Pl Types"), StringLength(25), Required]
        public string Assessmenttype { get; set; }
        [Display(Name = "PL Descriptions"), StringLength(255), Required]
        public string Assessmentdesc { get; set; }
        //[Display(Name = "Percentage %"), StringLength(10), Required]
        //public string Percentages { get; set; }
        [Display(Name = "Question Counts"), StringLength(10), Required]
        public string Questioncnt { get; set; }

        public Assquestionlimits()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }

        private readonly ApplicationDbContext _dbcontext;
        public Assquestionlimits(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbcontext.Assquestionlimits.Add(this);
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
        public async Task<object> SaveList(List<Assquestionlimits> Assquestionlimits)
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
                _dbcontext.Assquestionlimits.Attach(this);
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
                return await _dbContext.Assquestionlimits.FirstOrDefaultAsync(con => con.Assessmenttype == Assessmenttype);
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
                var regstr = await _dbContext.Assquestionlimits.FirstOrDefaultAsync(con => con.Assessmenttype == Assessmenttype);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

    }
}
