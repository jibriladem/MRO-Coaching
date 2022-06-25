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

namespace MROCoatching.DataObjects.Models.MasterTables
{
    [Table("ASSESSORS")]
    public class Assessors : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required, StringLength(10)]
        [RegularExpression(@"^[0-9A-Za-z]{1,10}$", ErrorMessage = "Please input alphanumeric values only for Employee ID")]
        [Display(Name = "Employee Id")]
        public string EmployeeId { get; set; }
        [Required, StringLength(100)]
        [RegularExpression(@"^[A-Za-z\s_]{10,100}$", ErrorMessage = "Please input alphanumeric values only for Employee Name")]
        [Display(Name = "Employee Name")]
        public string Employeename { get; set; }
        [Required, StringLength(10)]
        [RegularExpression(@"^[0-9A-Za-z]{1,10}$", ErrorMessage = "Please input alphanumeric values only for Cost Center Code")]
        [Display(Name = "Cost Center")]
        public string Costcentercode { get; set; }
        [NotMapped, StringLength(50), Display(Name = "Cost Center Name")]
        public string Costcentername { get; set; }
        [Display(Name = "Position ID"), StringLength(10)]
        public string Postid { get; set; }
        [NotMapped, StringLength(50), Display(Name = "Position Name")]
        public string Postname { get; set; }
        [Display(Name = "Department Code"), StringLength(15)]
        public string Deptcode { get; set; }
        [NotMapped, Display(Name = "Department Name"), StringLength(100)]
        public string Deptname { get; set; }
        [Display(Name = "Assessor Training"), StringLength(10)]
        public string Assessortrn { get; set; }
        [Display(Name = "Assessor Authorizations"), StringLength(10)]
        public string Assessorauth { get; set; }
        [Display(Name = "Effective From Date")]
        public DateTime Fromdate { get; set; }
        [Display(Name = "Effective Too Date")]
        public DateTime Toodate { get; set; }
        [NotMapped, Display(Name = "File")]
        public IFormFile Files { set; get; }
        public Assessors()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }

        //private readonly ApplicationDbContext _dbcontext;
        public Assessors(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbContext.Assessors.Add(this);
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
        public async Task<object> SaveList(List<Assessors> assessors)
        {
            try
            {
                foreach (var val in assessors)
                {
                    var getval = _dbContext.Assessors.Where(con => con.EmployeeId == val.EmployeeId).FirstOrDefault();
                    if (getval != null)
                    {
                        getval.AEDAT = DateTime.Now;
                        getval.AENAM = val.AENAM;
                        _dbContext.Assessors.Update(getval);
                        _dbContext.Entry(getval).State = EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.Assessors.Add(val);
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
                _dbContext.Assessors.Attach(this);
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
                return await _dbContext.Assessors.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId);
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
                var regstr = await _dbContext.Assessors.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

    }
}
