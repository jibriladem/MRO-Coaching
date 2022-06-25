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
    [Table("PAIRUPPLANS")]
    public class Pairupplans : Audit
    {
        public long Id { get; set; }
        [Display(Name = "Full Name"), Required, StringLength(100)]
        public string Fullname { get; set; }
        [Display(Name = "Registration Number"), StringLength(100), Required]
        public string Regnumber { get; set; }
        [Display(Name = "Current Position"), StringLength(100), Required]
        public string CurrentPosition { get; set; }
        [Display(Name = "Last Promotion")]
        public DateTime LastPromotiondate { get; set; }
        [Display(Name = "Next Promotion")]
        public DateTime NextPromotiondate { get; set; }
        [Display(Name = "PL Type")]
        public string Pltype { get; set; }
        [Display(Name = "PL Level")]
        public string Pllevel { get; set; }
        [Display(Name = "Coaching Start Date")]
        public DateTime Coachingstartdate { get; set; }
        [Display(Name = "Coaching End Date")]
        public DateTime Coachingenddate { get; set; }
        [Display(Name = "Coach Proposed by Section Mgr"), StringLength(100), Required]
        public string Coachproposedmgr { get; set; }
        [Display(Name = "Coach ID"), StringLength(10), Required]
        public string Coachid { get; set; }
        [Display(Name = "Coach Name"), StringLength(100), Required]
        public string Coachname { get; set; }
        [Display(Name = "Assessor Id"), StringLength(10), Required]
        public string Assessorid { get; set; }
        [Display(Name = "Assessor Name"), StringLength(100), Required]
        public string Assessorname { get; set; }
        [Display(Name = "Remarks"), StringLength(250)]
        public string Remark { get; set; }
        [Display(Name = "Status"), StringLength(5)]
        public string Status { get; set; }
        public Pairupplans()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public Pairupplans(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbContext.Pairupplans.Add(this);
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
        public async Task<object> SaveList(List<Pairupplans> pairupplans)
        {
            try
            {
                foreach (var vals in pairupplans)
                {
                    var getvals = _dbContext.Pairupplans.Where(con => con.Fullname == vals.Fullname).FirstOrDefault();
                    if (getvals != null)
                    {
                        getvals.AEDAT = DateTime.Now;
                        getvals.AENAM = vals.AENAM;
                        _dbContext.Pairupplans.Update(getvals);
                        _dbContext.Entry(getvals).State = EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.Pairupplans.Add(vals);
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
                _dbContext.Pairupplans.Attach(this);
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
                return await _dbContext.Pairupplans.FirstOrDefaultAsync(con => con.Fullname == Fullname);
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
                var regstr = await _dbContext.Pairupplans.FirstOrDefaultAsync(con => con.Fullname == Fullname);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

    }
}
