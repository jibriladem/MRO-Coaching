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
    [Table("COSTCENTER")]
    public class Costcenters : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required, Display(Name = "Cost Center Code"), StringLength(10)]
        public string Costcentercode { get; set; }
        [Required, Display(Name = "Cost Center Name"), StringLength(100)]
        public string Costcentername { get; set; }
        [Display(Name = "Cost Center Descriptions"), StringLength(100), Required]
        public string Costcenterdesc { get; set; }
        [Display(Name = "Effective From Date")]
        public DateTime Fromdate { get; set; }
        [Display(Name = "Effective To Date")]
        public DateTime Toodate { get; set; }
        [Display(Name = "Status"), StringLength(1)]
        public string Status { get; set; }
        [NotMapped]
        public bool chk_Status { get; set; }
        [NotMapped, Display(Name = "File")]
        public IFormFile Files { set; get; }
        [NotMapped]
        public List<Positions> positions { get; set; }
        public Costcenters()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }

        //private readonly ApplicationDbContext _dbcontext;
        public Costcenters(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbContext.Costcenters.Add(this);
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
        public async Task<object> SaveList(List<Costcenters> costcenters)
        {
            try
            {
                foreach (var val in costcenters)
                {
                    var value = _dbContext.Costcenters.Where(con => con.Costcentercode == val.Costcentercode).FirstOrDefault();
                    if (value != null)
                    {
                        value.AEDAT = val.AEDAT;
                        value.AENAM = val.AENAM;
                        _dbContext.Costcenters.Update(value);
                        _dbContext.Entry(value).State = EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.Costcenters.Add(val);
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
                //this.AEDAT = DateTime.Now;
                _dbContext.Costcenters.Attach(this);
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
                return await _dbContext.Costcenters.FirstOrDefaultAsync(con => con.Costcentercode == Costcentercode);
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
                var regstr = await _dbContext.Costcenters.FirstOrDefaultAsync(con => con.Costcentercode == Costcentercode);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}
