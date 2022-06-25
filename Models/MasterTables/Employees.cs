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
    [Table("EMPLOYEES")]
    public class Employees : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Display(Name = "Employee Id"), StringLength(10), Required]
        [RegularExpression(@"^[0-9A-Za-z]{5,10}$", ErrorMessage = "Please input alphanumeric values only for Employee ID")]
        public string EmployeeId { get; set; }
        [Display(Name = "Employee Name"), StringLength(100), Required]
        public string Employeename { get; set; }
        [Display(Name = "Cost Center"), StringLength(10), Required]
        public string Costcentercode { get; set; }
        [StringLength(50), Display(Name = "Cost Center Name"), Required]
        public string Costcentername { get; set; }
        [Display(Name = "Seniority Date")]
        public DateTime Seniordate { get; set; }
        [Display(Name = "Position Id"), Required]
        public string PositionId { get; set; }
        [Display(Name = "Promotion Date"), Required]
        public DateTime Promotiondate { get; set; }
        [Display(Name = "Next Promotion Date")]
        public DateTime? NextPromdate { get; set; }
        [Display(Name = "Next PL")]
        public string Nextpl { get; set; }
        [Display(Name = "Is Management?"), Required]
        public string Status1 { get; set; }
        [Required]
        public string Empgroup { get; set; }
        [Display(Name = " Reports To"), Required]
        public string Reportsto { get; set; }
        [Display(Name = "Is Expert?")]
        public string Status2 { get; set; }
        [Display(Name = "Effective From Date"), Required]
        public DateTime Fromdate { get; set; }
        [Display(Name = "Effective To Date"), Required]
        public DateTime Toodate { get; set; }
        [Display(Name = "Status"), Required]
        public string Status { get; set; }
        [StringLength(50), Display(Name = "Position Descriptions"), Required]
        public string Positiondesc { get; set; }
        [NotMapped]
        public bool CheckStatus2 { get; set; }
        [NotMapped]
        public bool CheckStatus1 { get; set; }
        [NotMapped]
        public bool CheckStatus { get; set; }
        [NotMapped, Display(Name = "File")]
        public IFormFile Files { set; get; }
        [NotMapped]
        public object FolderName { get; set; }
        [NotMapped]
        public string FileName { get; set; }

        public Employees()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }

        //private readonly ApplicationDbContext _dbcontext;
        public Employees(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                var val = _dbContext.Employees.Where(con => con.EmployeeId == this.EmployeeId).FirstOrDefault();
                if (val != null)
                {
                    val.Toodate = DateTime.Now;
                    val.AEDAT = DateTime.Now;
                    val.AENAM = this.ERNAM;
                    _dbContext.Employees.Update(val);
                    _dbContext.Entry(val).State = EntityState.Modified;
                }
                _dbContext.Employees.Add(this);
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
        public async Task<object> SaveList(List<Employees> employees)
        {
            try
            {
                foreach (var emp in employees)
                {
                    if (emp != null)
                    {
                        var gets = _dbContext.Employees.Where(c => c.EmployeeId == emp.EmployeeId && c.Costcentercode == emp.Costcentercode && c.Toodate > DateTime.Now && c.Status == "Y").FirstOrDefault();
                        if (gets != null)
                        {
                            gets.Toodate = DateTime.Now;
                            gets.Status = "N";
                            gets.AEDAT = DateTime.Now;
                            _dbContext.Employees.Update(gets);
                            _dbContext.Entry(gets).State = EntityState.Modified;
                            _dbContext.Employees.Add(emp);
                            _dbContext.Entry(emp).State = EntityState.Added;
                        }
                        else
                        {
                            _dbContext.Employees.Add(emp);
                            _dbContext.Entry(emp).State = EntityState.Added;
                        }
                    }
                }

                //employees.ForEach(emp =>
                //{
                //    var gets = _dbContext.Employees.Where(c => c.EmployeeId == emp.EmployeeId && c.Costcentercode == emp.Costcentercode && c.Toodate > DateTime.Now && c.status == "Y").FirstOrDefault();
                //    if (gets != null)
                //    {
                //        gets.Toodate = DateTime.Now;
                //        gets.status = "N";
                //        gets.AEDAT = DateTime.Now;
                //        _dbContext.Employees.Update(gets);
                //        _dbContext.Entry(gets).State = EntityState.Modified;
                //        _dbContext.Employees.Add(emp);
                //        _dbContext.Entry(emp).State = EntityState.Added;
                //    }
                //    else
                //    {
                //        _dbContext.Employees.Add(emp);
                //        _dbContext.Entry(emp).State = EntityState.Added;
                //    }
                //});
                //_dbContext.Employees.AddRange(employees);
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
                _dbContext.Employees.Attach(this);
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
                return await _dbContext.Employees.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId);
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
                var emp = await _dbContext.Employees.Where(con => con.EmployeeId == EmployeeId && con.Costcentercode == Costcentercode).FirstOrDefaultAsync();
                //var emp = _dbContext.Employees.AsNoTracking().Where(con => con.EmployeeId == EmployeeId && con.Costcentercode == Costcentercode).FirstOrDefault();
                return emp == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}
