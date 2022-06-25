using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MROCoatching.DataObjects.Models.CoachingTable
{
    [Table("ATAChapters")] 
    public class ATAChapters : Audit
    {
        public long Id { get; set; }
        [Display(Name = "ATA Chapter"), StringLength(10), Required]
        public string ATA_Chapter { get; set; }
        [Display(Name = "Chapter Title"), StringLength(100), Required]
        public string Chapter_Title { get; set; }
        [Display(Name = "Task Divisions"), StringLength(200)]
        public string Taskdiv { get; set; }
        [Display(Name = "Status"), StringLength(5)]
        public string Status { get; set; }
        [NotMapped, Display(Name = "File")]
        public IFormFile Files { set; get; }
        public ATAChapters()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public ATAChapters(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<object> Save()
        {
            try
            {
                _dbContext.ATAChapters.Add(this);
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
        public async Task<object> SaveList(List<ATAChapters> taskgroup)
        {
            try
            {
                foreach (var val in taskgroup)
                {
                    var value = _dbContext.ATAChapters.Where(con => con.ATA_Chapter == val.ATA_Chapter).FirstOrDefault();
                    if (value != null)
                    {
                        value.AEDAT = val.AEDAT;
                        value.AENAM = val.AENAM;
                        _dbContext.ATAChapters.Update(value);
                        _dbContext.Entry(value).State = EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.ATAChapters.Add(val);
                        _dbContext.Entry(val).State = EntityState.Added;
                    }
                }
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
                _dbContext.ATAChapters.Attach(this);
                _dbContext.Entry(this).State = EntityState.Modified;
                if (await _dbContext.SaveChangesAsync() > 0)
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
                _dbContext.Remove(this);
                if (await _dbContext.SaveChangesAsync() > 0)
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
        public async Task<object> Refresh()
        {
            try
            {
                return await _dbContext.ATAChapters.FirstOrDefaultAsync(con => con.ATA_Chapter == ATA_Chapter && con.Chapter_Title == Chapter_Title);
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
                var regstr = await _dbContext.ATAChapters.FirstOrDefaultAsync(con => con.ATA_Chapter == ATA_Chapter);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

    }
}
