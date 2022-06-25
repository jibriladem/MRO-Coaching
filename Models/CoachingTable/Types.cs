using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
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
    [Table("PLTYPES")]
    public class Types : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Display(Name = "PL Code"), StringLength(50), Required]
        public string Plcode { get; set; }
        [Display(Name = "PL Discreption"), StringLength(100), Required]
        public string Pldiscreption { get; set; }
        [Display(Name = "A/c or eng Model"), StringLength(25), Required]
        public string Model { get; set; }
        [Display(Name = "Cost Center"), StringLength(25), Required]
        public string Costcenter { get; set; }
        [Display(Name = "PL Level"), StringLength(50), Required]
        public string Pllevel { get; set; }
        [Display(Name = "Pl Verssion"), StringLength(25), Required]
        public string Plversion { get; set; }
        [Display(Name = "Revision Date")]
        public DateTime Revisiondate { get; set; }
        [Display(Name = "effective To")]
        public DateTime Effectiveto { get; set; }
        [Display(Name = "Original Date")]
        public DateTime Originaldate { get; set; }
        [Display(Name = "Document No"), StringLength(25), Required]
        public string Documentno { get; set; }
        [Display(Name = "Status"), StringLength(1)]
        public string Status { get; set; }
        [NotMapped]
        public List<Taskcode> Taskcode { get; set; }
        [NotMapped]
        public List<Items> Items { get; set; }
        [NotMapped]
        public string Costcentername { get; set; }
        [NotMapped]
        public bool checkStatus { get; set; }
        [NotMapped, StringLength(10)]
        public string Status1 { get; set; }
        [NotMapped, Display(Name = "File")]
        public IFormFile Files { set; get; }
        public Types()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }

        //private readonly ApplicationDbContext _dbcontext;
        public Types(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbContext.Types.Add(this);
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
        public async Task<object> SaveList(List<Types> types)
        {
            try

            {
                foreach (var val in types)
                {
                    var value = _dbContext.Types.Where(con => con.Plcode == val.Plcode).FirstOrDefault();
                    if (value != null)
                    {
                        value.AEDAT = val.AEDAT;
                        value.AENAM = val.AENAM;
                        _dbContext.Types.Update(value);
                        _dbContext.Entry(value).State = EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.Types.Add(val);
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
                _dbContext.Types.Attach(this);
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
        public async Task<object> Update1(List<Items> items)
        {
            try
            {
                //this.AEDAT = DateTime.Now;
                //_dbContext.Items.Attach(items);
                //_dbContext.Entry(items).State = EntityState.Modified;
                if (items.Count > 0)
                {
                    var grp = items.GroupBy(c => new { c.Taskcode, c.Plcode }).ToList();
                    foreach (var item in grp)
                    {
                        var val = _dbContext.Items.AsNoTracking().Where(c => c.Plcode == item.Key.Plcode && c.Taskcode == item.Key.Taskcode).ToList();
                        if (val.Count > 0)
                        {
                            foreach(var val1 in val)
                            {
                                val1.Assesment = "Y";
                                val1.AEDAT = DateTime.Now;
                                //val1.AENAM = item.Key.AENAM;
                                _dbContext.Items.Attach(val1);
                                _dbContext.Entry(val1).State = EntityState.Modified;
                            }
                        }
                    }
                }
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
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<object> Refresh()
        {
            try
            {
                return await _dbContext.Types.FirstOrDefaultAsync(con => con.Plcode == Plcode);
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
                var regstr = await _dbContext.Types.FirstOrDefaultAsync(con => con.Plcode == Plcode);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

    }
}
