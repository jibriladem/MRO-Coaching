using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.DataObjects.Models.BasicTransactions
{
    [Table("COACHING")]
    public class Coaching : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Display(Name = "Employee Id")]
        public string EmployeeId { get; set; }
        [Display(Name = "Employee Name")]
        public string Employeename { get; set; }
        [Display(Name = "Coach Id")]
        public string Coachid { get; set; }
        [Display(Name = "Coach Name")]
        public string Coachname { get; set; }
        [Display(Name = "Task Code"), StringLength(10)]
        public string Taskcode { get; set; }
        [NotMapped]
        public string Taskname { get; set; }
        [Display(Name = "Task Descriptions")]
        public string Taskdesc { get; set; }
        [Display(Name = "Trainee Approval")]
        public string Traineeapproval { get; set; }
        [Display(Name = "Coach Approval")]
        public string Coachapproval { get; set; }
        [Display(Name = "Approval Date")]
        public DateTime Dateapproved { get; set; }
        [StringLength(1)]
        public string Status { get; set; }
        public DateTime? AssStartdate { get; set; }
        public DateTime? AssEnddate { get; set; }
        [Display(Name = "ATA Reference Code"), StringLength(50)]
        public string ATAREFCODE { get; set; }
        [Display(Name = "Maintenance Record Reference"), StringLength(50)]
        public string TSFNCODE { get; set; }
        [Display(Name = "Action"), StringLength(5), DefaultValue("N")]
        public string Actionpln { get; set; }
        [Display(Name = "Action From Date")]
        public DateTime? Actionfrm { get; set; }
        [Display(Name = "Action Too Date")]
        public DateTime? Actiontoo { get; set; }
        [Display(Name = "PL Code"), Required, StringLength(50)]
        public string Plcode { get; set; }
        [Display(Name = "PL Name"), Required, StringLength(50)]
        public string Plname { get; set; }
        [Display(Name = "PL Level"), Required, StringLength(50)]
        public string Pllevel { get; set; }
        public long Taskcount { get; set; }//Added for Operational Purpose
        [Display(Name = "Reasons for")]
        public string Reasons { get; set; }
        [Display(Name = "Assessment Actions"), StringLength(5)]
        public string Assaction { get; set; }
        [NotMapped]
        public DateTime? Coachingstart { get; set; }
        [NotMapped]
        public DateTime? Coachingend { get; set; }
        [NotMapped]
        public DateTime? Whenassess { get; set; }
        [NotMapped]
        public string Checkbox { get; set; }
        [NotMapped]
        public bool CK_Checkbox { get; set; }
        [NotMapped]
        public List<Coaching> Coachings { get; set; }
        [NotMapped]
        public DateTime? Actdate {get; set; }
        [NotMapped]
        public long? Completed { get; set; }
        [NotMapped]
        public long? Onprogress { get; set; }
        [NotMapped]
        public long? Notstarted { get; set; }
        [NotMapped]
        public string sender { get; set; }
        public Coaching()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public Coaching(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbContext.Coaching.Add(this);
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

        public async Task<object> SaveList(List<Coaching> coachings)
        {
            try
            {
                foreach (var vals in coachings)
                {
                    var getvals = _dbContext.Coaching.Where(con => con.EmployeeId == vals.EmployeeId).FirstOrDefault();
                    if (getvals != null)
                    {
                        getvals.AEDAT = DateTime.Now;
                        getvals.AENAM = vals.AENAM;
                        _dbContext.Coaching.Update(getvals);
                        _dbContext.Entry(getvals).State = EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.Coaching.Add(vals);
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
                _dbContext.Coaching.Attach(this);
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
        public async Task<object> Update1()
        {
            try
            {
                _dbContext.Coaching.Attach(this);
                _dbContext.Entry(this).State = EntityState.Modified;
                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return new DatabaseOperationResponse
                    {
                        Status = Others.OperationStatus.Approve,
                        Message = Resource.RecordSucessfullyApproved,
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
        public async Task<object> Updatelist(List<Coaching> getlist)
        {
            try
            {
                if (getlist.Count > 0)
                {
                    foreach (var value1 in getlist)
                    {
                        //var apprv = _dbContext.Coachannualplans.Where(c => c.Coachid == value1.Coachid && c.EmployeeId == value1.EmployeeId).FirstOrDefault();
                        //if (apprv != null)
                        //{
                        //    var apprvl = _dbContext.Coaching.Where(c => c.Coachid == apprv.Coachid && c.EmployeeId == apprv.EmployeeId).ToList();
                        //    if (apprvl.Count > 0)
                        //    {
                        //        var teamNames = apprvl.Where(c => c.Coachapproval != "").Select(t => t.Coachapproval).ToList();
                        //        if (teamNames.Count == 0)
                        //        {
                        //            continue;
                        //        }
                        //        else
                        //        {
                        //            var getval = _dbContext.Coachannualplans.Where(c => c.EmployeeId == apprv.EmployeeId && c.Coachid == apprv.Coachid).ToList();
                        //            if (getval.Count > 0)
                        //            {
                        //                foreach (var vals in getval)
                        //                {
                        //                    vals.Approve3 = "Y";
                        //                    _dbContext.Coachannualplans.Update(vals);
                        //                    _dbContext.Entry(vals).State = EntityState.Modified;
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        _dbContext.Coaching.Update(value1);
                        _dbContext.Entry(value1).State = EntityState.Modified;
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
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<object> Refresh()
        {
            try
            {
                return await _dbContext.Coaching.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId);
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
                var regstr = await _dbContext.Coaching.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

    }
}
