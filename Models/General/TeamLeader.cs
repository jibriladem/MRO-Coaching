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
using System.Threading.Tasks;

namespace MROCoatching.DataObjects.Models.General
{
    [Table("TEAMLEADER")]
    public class TeamLeader : Audit
    {
        public long Id { get; set; }
        //[Display(Name = "Sub Section Code"), StringLength(10), Required]
        //public string Teamcode { get; set; }
        //[Display(Name = "Sub Section Name"), StringLength(100), Required]
        //public string Teamname { get; set; }
        [Display(Name = "Section Code"), StringLength(10), Required]
        public string Sectcode { get; set; }
        [Display(Name = "Section Name"), StringLength(100), Required]
        public string Sectname { get; set; }
        [Display(Name = "Cost Center"), StringLength(10), Required]
        public string Costcentercode { get; set; }
        [StringLength(50), Required, Display(Name = "Reports To")]
        public string Reportsto { get; set; }
        [StringLength(50), Required, Display(Name = "Cost Center Name")]
        public string Costcentername { get; set; }
        [Display(Name = "Department Code")]
        public string Deptcode { get; set; }
        [NotMapped, Display(Name = "Department Name")]
        public string Deptname { get; set; }
        [Display(Name = "Effective From Date")]
        public DateTime Fromdate { get; set; }
        [Display(Name = "Effective To Date")]
        public DateTime Toodate { get; set; }
        [NotMapped, Display(Name = "File")]
        public IFormFile Files { set; get; }
        public TeamLeader()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public TeamLeader(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbContext.TeamLeader.Add(this);
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
        public async Task<object> SaveList(List<TeamLeader> teams)
        {
            try
            {
                foreach (var vals in teams)
                {
                    var getval = _dbContext.TeamLeader.Where(con => con.Costcentercode == vals.Costcentercode).FirstOrDefault();
                    if (getval != null)
                    {
                        getval.AEDAT = DateTime.Now;
                        getval.AENAM = vals.AENAM;
                        _dbContext.TeamLeader.Update(getval);
                        _dbContext.Entry(getval).State = EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.TeamLeader.Add(vals);
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
                _dbContext.TeamLeader.Attach(this);
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
                return await _dbContext.TeamLeader.FirstOrDefaultAsync(con => con.Costcentercode == Costcentercode);
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
                var regstr = await _dbContext.TeamLeader.FirstOrDefaultAsync(con => con.Costcentercode == Costcentercode);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

    }
}
