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
    public class Coachedon : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Display(Name = "Employee Id"), StringLength(10), Required]
        [RegularExpression(@"^[0-9A-Za-z]{5,10}$", ErrorMessage = "Please input alphanumeric values only for Employee ID")]
        public string EmployeeId { get; set; }
        [Required, Display(Name = "Employee Name"), StringLength(100)]
        [RegularExpression(@"^[A-Za-z\s_]{10,100}$", ErrorMessage = "Please input alphanumeric values only for Employee Name")]
        public string Employeename { get; set; }
        [Display(Name = "Cost Center"), StringLength(10), Required]
        [RegularExpression(@"^[0-9A-Za-z]{5,10}$", ErrorMessage = "Please input alphanumeric values only for Cost Center Code")]
        public string Costcentercode { get; set; }
        [StringLength(50), Display(Name = "Cost Center Name")]
        public string Costcentername { get; set; }
        [Display(Name = "Department Code")]
        public string Deptcode { get; set; }
        [Display(Name = "Department Name")]
        public string Deptname { get; set; }
        [Display(Name = "Current Position")]
        public string Currposition { get; set; }
        [Display(Name = "Next Promoted to")]
        public string Toobecoachedon { get; set; }
        [Display(Name = "Status")]
        public string Coachingstatus { get; set; }
        [Display(Name = "Last Promotion Date")]
        public DateTime Lastpromdate { get; set; }
        [Display(Name = "Next Promotion Date")]
        public DateTime Nextpromdate { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        public Coachedon()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public Coachedon(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                var val = _dbContext.Coachedon.Where(con => con.EmployeeId == this.EmployeeId).FirstOrDefault();
                if (val != null)
                {
                    _dbContext.Coachedon.Update(val);
                    _dbContext.Entry(val).State = EntityState.Modified;
                }
                _dbContext.Coachedon.Add(this);
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
        public async Task<object> SaveList(List<Coachedon> coachedons)
        {
            try
            {
                foreach (var emp in coachedons)
                {
                    if (emp != null)
                    {
                        //var gets = _dbContext.Employees.Where(c => c.EmployeeId == emp.EmployeeId && c.Costcentercode == emp.Costcentercode && c.Toodate > DateTime.Now && c.status == "Y").FirstOrDefault();
                        //if (gets != null)
                        //{
                        //    gets.Toodate = DateTime.Now;
                        //    gets.status = "N";
                        //    gets.AEDAT = DateTime.Now;
                        //    _dbContext.Employees.Update(gets);
                        //    _dbContext.Entry(gets).State = EntityState.Modified;
                        //    _dbContext.Coachedon.Add(emp);
                        //    _dbContext.Entry(emp).State = EntityState.Added;
                        //}
                        //else
                        //{
                        //    _dbContext.Coachedon.Add(emp);
                        //    _dbContext.Entry(emp).State = EntityState.Added;
                        //}
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
                _dbContext.Coachedon.Attach(this);
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
                return await _dbContext.Coachedon.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IDisposable OpenRead(Employees enteredFiles)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Exist()
        {
            try
            {
                var emp = await _dbContext.Coachedon.Where(con => con.EmployeeId == EmployeeId && con.Costcentercode == Costcentercode && con.Toobecoachedon == Toobecoachedon && con.Status == Status).FirstOrDefaultAsync();
                return emp == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

    }
}
