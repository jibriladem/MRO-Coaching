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
    [Table("COACHES")]
    public class Coaches : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required, Display(Name = "Employee Id"), StringLength(10)]
        [RegularExpression(@"^[0-9A-Za-z]{5,10}$", ErrorMessage = "Please input alphanumeric values only for Employee ID")]
        public string EmployeeId { get; set; }
        [RegularExpression(@"^[A-Za-z\s_]{10,100}$", ErrorMessage = "Please input alphanumeric values only for Employee Name")]
        [Required, Display(Name = "Employee Name"), StringLength(100)]
        public string Employeename { get; set; }
        [Required, Display(Name = "Cost Center"), StringLength(10)]
        [RegularExpression(@"^[0-9A-Za-z]{5,10}$", ErrorMessage = "Please input alphanumeric values only for Cost Center Code")]
        public string Costcentercode { get; set; }
        [NotMapped, StringLength(50), Display(Name = "Cost Center Name")]
        public string Costcentername { get; set; }
        [Display(Name = "Department Code"), StringLength(10)]
        public string Deptcode { get; set; }
        [NotMapped, Display(Name = "Department Name"), StringLength(100)]
        public string Deptname { get; set; }
        [Display(Name = "Online Coach Training"), StringLength(10)]
        public string Onlinecoach { get; set; }
        [Display(Name = "Classroom Coach Training"), StringLength(10)]
        public string Classroomcoach { get; set; }
        [Display(Name = "Coach Authorizations")]
        public string Coachauth { get; set; }
        [Display(Name = "Effective From Date")]
        public DateTime Fromdate { get; set; }
        [Display(Name = "Effective Too Date")]
        public DateTime Toodate { get; set; }
        [NotMapped, Display(Name = "File")]
        public IFormFile Files { set; get; }
        [NotMapped]
        public string Training { get; set; }
        public Coaches()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }

        //private readonly ApplicationDbContext _dbcontext;
        public Coaches(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbContext.Coaches.Add(this);
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
        public async Task<object> SaveList(List<Coaches> coaches)
        {
            try
            {
                foreach (var val in coaches)
                {
                    var getval = _dbContext.Coaches.Where(con => con.Costcentercode == val.Costcentercode).FirstOrDefault();
                    if (getval != null)
                    {
                        _dbContext.Coaches.Update(getval);
                        _dbContext.Entry(getval).State = EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.Coaches.Add(val);
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
                _dbContext.Coaches.Attach(this);
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
                return await _dbContext.Coaches.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId);
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
                var regstr = await _dbContext.Coaches.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}
