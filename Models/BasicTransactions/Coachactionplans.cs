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

namespace MROCoatching.DataObjects.Models.BasicTransactions
{
    [Table("COACHACTIONPLANS")]
    public class Coachactionplans : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Display(Name = "Fleet"), Required, StringLength(50)]
        public string Fleet { get; set; }
        [Display(Name = "Tail Number"), Required, StringLength(50)]
        public string Tailnumber { get; set; }
        [Display(Name = "Task Code"), StringLength(50)]
        public string Taskcode { get; set; }
        [Display(Name = "Task Descriptions"), StringLength(250)]
        public string Taskdesc { get; set; }
        public DateTime plandate { get; set; }

        [Display(Name = "Excuted (Yes/No)"), StringLength(10)]
        public string Excuted { get; set; }

        [Display(Name = "Remarks"), StringLength(250)]
        public string Remark { get; set; }
        public Coachactionplans()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public Coachactionplans(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbContext.Coachactionplans.Add(this);
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
        public async Task<object> SaveList(List<Coachactionplans> coachactionplans)
        {
            try
            {
                foreach (var vals in coachactionplans)
                {
                    var getvals = _dbContext.Coachactionplans.Where(con => con.Fleet == vals.Fleet).FirstOrDefault();
                    if (getvals != null)
                    {
                        getvals.AEDAT = DateTime.Now;
                        getvals.AENAM = vals.AENAM;
                        _dbContext.Coachactionplans.Update(getvals);
                        _dbContext.Entry(getvals).State = EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.Coachactionplans.Add(vals);
                        _dbContext.Entry(vals).State = EntityState.Added;
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
                //this.AEDAT = DateTime.Now;
                _dbContext.Coachactionplans.Attach(this);
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
                return await _dbContext.Coachactionplans.FirstOrDefaultAsync(con => con.Fleet == Fleet);
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
                var regstr = await _dbContext.Coachactionplans.FirstOrDefaultAsync(con => con.Fleet == Fleet);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

    }
}
