using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using MROCoatching.DataObjects.Models.BasicTransactions;
using MROCoatching.DataObjects.Models.General;
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
    [Table("PLITEMS")]
    public class Items : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Display(Name = "ATA Chapter"), StringLength(10), Required]
        public string Refnumber { get; set; }
        [Display(Name = "PL Code"), StringLength(50), Required]
        public string Plcode { get; set; }
        [Display(Name = "Task Code"), StringLength(10), Required]
        public string Taskcode { get; set; }
        [Display(Name = "Descriptions")]
        public string Description { get; set; }
        [Display(Name = "AssesmenItem"), StringLength(1), Required]
        public string Assesment { get; set; }
        [Display(Name = "Status"), StringLength(5), Required]
        public string Status { get; set; }
        [Display(Name = "PL Level"), StringLength(50), Required]
        public string Pltypes { get; set; }
        public long Taskcount { get; set; }//Added for Operational Purpose
        [NotMapped]
        public string Coachid { get; set; }
        [NotMapped]
        public List<Coaching> Coachings { get; set; }
        [NotMapped]
        public List<Taskcode> TasTaskcode { get; set; }
        [NotMapped, Display(Name = "File")]
        public IFormFile Files { set; get; }
        [NotMapped]
        public string Checkbox { get; set; }
        [NotMapped]
        public bool CK_Checkbox { get; set; }
        [NotMapped]
        public string TSFNCODE { get; set; }
        [NotMapped]
        public string Selected { get; set; }//select items for Assessments
        [NotMapped]
        public string ATAREFCODE { get; set; }
        //[Display(Name = "Category"), StringLength(100)]
        //public string Category { get; set; }
        public Items()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public Items(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbContext.Items.Add(this);
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
        public async Task<object> SaveList(List<Items> items)
        {
            try
            {
                foreach (var item in items)
                {
                    if (item != null)
                    {
                        var getitem = new List<Items>();
                        if (item.Refnumber != null)
                        {
                            getitem = _dbContext.Items.AsNoTracking().Where(c => c.Plcode == item.Plcode && c.Refnumber == item.Refnumber && c.Taskcode == item.Taskcode).ToList();
                        }
                        else
                        {
                            getitem = _dbContext.Items.AsNoTracking().Where(c => c.Plcode == item.Plcode && c.Taskcode == item.Taskcode).ToList();
                        }
                        if (getitem.Count > 0)
                        {
                            continue;
                        }
                        else
                        {
                            _dbContext.Items.Add(item);
                            _dbContext.Entry(item).State = EntityState.Added;
                        }
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
                _dbContext.Items.Attach(this);
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
                return await _dbContext.Items.FirstOrDefaultAsync(con => con.Refnumber == Refnumber);
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
                var regstr = await _dbContext.Items.FirstOrDefaultAsync(con => con.Refnumber == Refnumber);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

    }
}
