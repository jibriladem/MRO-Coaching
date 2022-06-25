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

namespace MROCoatching.DataObjects.Models.MasterTables
{
    [Table("MANAGERS")]
    public class Managers : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [RegularExpression(@"^[0-9A-Za-z]{1,10}$", ErrorMessage = "Please input alphanumeric values only for Employee ID")]
        [Required, Display(Name = "Employee Id"), StringLength(10)]
        public string EmployeeId { get; set; }
        [Required, Display(Name = "Employee Name"), StringLength(100)]
        [RegularExpression(@"^[A-Za-z\s_]{5,100}$", ErrorMessage = "Please input alphanumeric values only for Employee ID")]
        public string Employeename { get; set; }
        [Display(Name = "Status"), StringLength(1)]
        public string Status3 { get; set; }
        [Display(Name = "Cost Center Code"), StringLength(10), Required]
        public string Costcentercode { get; set; }
        [NotMapped, StringLength(50), Display(Name = "Cost Center Name")]
        public string Costcentername { get; set; }
        [Display(Name = "Position ID"), StringLength(10), Required]
        public string Postid { get; set; }
        [NotMapped, StringLength(50)]
        public string Postname { get; set; }
        [Display(Name = "Department Code"), StringLength(10)]
        public string Deptcode { get; set; }
        [NotMapped, StringLength(100), Display(Name = "Department Name")]
        public string Deptname { get; set; }
        [Display(Name = "Divisions"), StringLength(50)]
        public string Divisions { get; set; }
        [Display(Name = "Effective From Date")]
        public DateTime Fromdate { get; set; }
        [Display(Name = "Effective Too Date")]
        public DateTime Toodate { get; set; }
        [NotMapped, Display(Name = "File")]
        public IFormFile Files { set; get; }

        public Managers()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }

        //private readonly ApplicationDbContext _dbcontext;
        public Managers(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbContext.Managers.Add(this);
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
        public async Task<object> SaveList(List<Managers> managers)
        {
            try
            {
                foreach (var val in managers)
                {
                    var getval = _dbContext.Managers.Where(con => con.EmployeeId == val.EmployeeId).FirstOrDefault();
                    if (getval != null)
                    {
                        getval.AEDAT = DateTime.Now;
                        getval.AENAM = val.AENAM;
                        _dbContext.Managers.Update(getval);
                        _dbContext.Entry(getval).State = EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.Managers.Add(val);
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
                _dbContext.Managers.Attach(this);
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
                return await _dbContext.Managers.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId);
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
                var regstr = await _dbContext.Managers.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

    }
}
