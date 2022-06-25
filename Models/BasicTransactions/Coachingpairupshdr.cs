using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using MROCoatching.DataObjects.Models.CoachingTable;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MROCoatching.DataObjects.Models.BasicTransactions
{
    [Table("COACHINGPAIRUPSHDR")]
    public class Coachingpairupshdr : Audit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Display(Name = "Employee Id"), Required, StringLength(10)]
        public string EmployeeId { get; set; }
        [Display(Name = "Employee Name"), Required, StringLength(100)]
        public string Employeename { get; set; }
        [Display(Name = "Position Id"), Required, StringLength(10)]
        public string Position { get; set; }
        [NotMapped]
        public string Postname { get; set; }
        [Display(Name = "Cost Center"), Required, StringLength(10)]
        public string Costcentercode { get; set; }
        [Display(Name = "Cost Center Description"), Required, StringLength(100)]
        public string Costcenterdesc { get; set; }
        public string Recentperformisc { get; set; }
        //[Display(Name = "Last Promotion Date")]
        //public DateTime Lastpromotiondate { get; set; }
        //[Display(Name = "Next Promotion Date")]
        //public DateTime? Nextpromotiondate { get; set; }
        [Display(Name = "Coaching Start Date")]
        public DateTime Coachingstartdate { get; set; }
        [Display(Name = "Coaching End Date")]
        public DateTime Coachingenddate { get; set; }
        [Display(Name = "When to Assess")]
        public DateTime Whentoassess { get; set; }
        [Display(Name = "PL Name"), Required, StringLength(50)]
        public string Plname { get; set; }
        [Display(Name = "PL Level"), Required, StringLength(50)]
        public string Pllevel { get; set; }
        [Display(Name = "Status"), StringLength(5)]
        public string Status { get; set; }
        [Display(Name = "Approval"), StringLength(1)]
        public string Approve1 { get; set; }//Approved while creating
        [Display(Name = "Approval"), StringLength(1)]
        public string Approve2 { get; set; }//Approved by HCM
        [Display(Name = "Approval"), StringLength(1)]
        public string Approve3 { get; set; }
        [NotMapped]
        public string getappr { get; set; }
        [Display(Name = "Reasons for Rejection")]
        public string Reasons { get; set; }
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        [NotMapped]
        public List<Coachingpairupsdtl> Coachingpairupsdtl { get; set; }
        [NotMapped]
        public List<Coachingpairupsdtl> Coachingpairupsdtl2 { get; set; }
        //[NotMapped]
        //public List<Items> Items { get; set; }
        [NotMapped]
        public int nbrofcoach { get; set; }
        [NotMapped]
        public int nbrofass { get; set; }
        [NotMapped]
        public List<Items> Items { get; set; }
        [NotMapped]
        public List<Coachingpairupshdr> actualpls { get; set; }
        [NotMapped]
        public string TSFNCODE { get; set; }
        [NotMapped]
        public string ATAREFCODE { get; set; }
        [NotMapped]
        public string getqnsamt { get; set; }
        [NotMapped]
        public long? getcompleted { get; set; }
        [NotMapped]
        public long? getwarning { get; set; }
        [NotMapped]
        public long? getrejected { get; set; }
        [NotMapped]
        public List<Coaching> Coaching { get; set; }
        public Coachingpairupshdr()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public Coachingpairupshdr(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                if (this.Coachingpairupsdtl.Count > 0)
                {
                    foreach (var item in this.Coachingpairupsdtl)
                    {
                        _dbContext.Coachingpairupsdtl.Add(item);
                        _dbContext.Entry(item).State = EntityState.Added;
                    }
                }
                _dbContext.Coachingpairupshdr.Add(this);
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
        public async Task<object> SaveList(List<Coachingpairupshdr> coachingpairupshdrs)
        {
            try
            {
                foreach (var vals in coachingpairupshdrs)
                {
                    var getvals = _dbContext.Coachingpairupshdr.Where(con => con.EmployeeId == vals.EmployeeId && con.Position == vals.Position && con.Plname == Plname).FirstOrDefault();
                    if (getvals != null)
                    {
                        getvals.AEDAT = DateTime.Now;
                        getvals.AENAM = vals.AENAM;
                        _dbContext.Coachingpairupshdr.Update(getvals);
                        _dbContext.Entry(getvals).State = EntityState.Modified;
                    }
                    else
                    {
                        _dbContext.Coachingpairupshdr.Add(vals);
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
                if (this.Coachingpairupsdtl.Count > 0)
                {
                    foreach (var val in this.Coachingpairupsdtl)
                    {
                        var getval = _dbContext.Coachingpairupsdtl.AsNoTracking().Where(c => c.EmployeeId == val.EmployeeId && c.Coachid == val.Coachid && c.Assessorid == val.Assessorid).FirstOrDefault();
                        if (getval != null)
                        {
                            continue;
                        }
                        else
                        {
                            val.Ids = 0;
                            _dbContext.Coachingpairupsdtl.Add(val);
                            _dbContext.Entry(val).State = EntityState.Added;
                        }
                    }
                }
                _dbContext.Coachingpairupshdr.Attach(this);
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
        public async Task<object> Reject()
        {
            try
            {
                _dbContext.Coachingpairupshdr.Attach(this);
                _dbContext.Entry(this).State = EntityState.Modified;
                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return new DatabaseOperationResponse
                    {
                        Status = Others.OperationStatus.Reject,
                        Message = Resource.ActionwasRejected,
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
        public async Task<object> Approve(Coachingpairupshdr getdata)
        {
            try
            {
                if (getdata != null)
                {
                    _dbContext.Coachingpairupshdr.Attach(getdata);
                    _dbContext.Entry(getdata).State = EntityState.Modified;
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
        public async Task<object> Deletelist(List<Coachingpairupsdtl> getdetails)
        {
            try
            {
                _dbContext.RemoveRange(getdetails);
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
                return await _dbContext.Coachingpairupshdr.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId);
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
                var regstr = await _dbContext.Coachingpairupshdr.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId && con.Plname == Plname && con.Position == Position);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
        public async Task<object> Approves()
        {
            var coachinglist = new List<Coaching>();
            try
            {
                var getitems = this.Items = _dbContext.Items.AsNoTracking().Where(con => con.Plcode == this.Plname).ToList();
                if (this.Coachingpairupsdtl.Count > 0)
                {
                    foreach (var val in this.Coachingpairupsdtl)
                    {
                        if (val.Ids > 0)
                        {
                            continue;
                        }
                        else
                        {
                            _dbContext.Coachingpairupsdtl.Add(val);
                            _dbContext.Entry(val).State = EntityState.Added;
                        }
                    }
                }
                if (getitems.Count > 0)
                {
                    foreach (var cc in getitems)
                    {
                        coachinglist.Add(new Coaching()
                        {
                            EmployeeId = this.EmployeeId,
                            Employeename = this.Employeename,
                            Taskcode = cc.Taskcode,
                            Taskdesc = cc.Description,
                            Plcode = cc.Plcode,
                            Plname = _dbContext.Types.AsNoTracking().Where(c => c.Plcode == cc.Plcode).Select(c => c.Pldiscreption).FirstOrDefault(),//cc.Description,
                            Pllevel = cc.Pltypes,
                            Traineeapproval = "",
                            Coachapproval = "",
                            Status = "N",
                            Taskcount = cc.Taskcount,
                            Actionpln = "N",
                            Dateapproved = DateTime.Now,
                            ERDAT = DateTime.Now,
                            ERNAM = this.ERNAM,
                            AEDAT = DateTime.Now,
                            AENAM = this.AENAM,
                        });
                    }
                    if (coachinglist.Count > 0)
                    {
                        foreach (var cc1 in coachinglist)
                        {
                            var getval = _dbContext.Coaching.AsNoTracking().Where(c => c.EmployeeId == cc1.EmployeeId && c.Pllevel == cc1.Pllevel && c.Taskcode == cc1.Taskcode && c.Plcode == cc1.Plcode).FirstOrDefault();
                            if (getval != null)
                            {
                                //cc1.ID = getval.ID;
                                //getval.AEDAT = DateTime.Now;
                                //getval.AENAM = cc1.AENAM;
                                //_dbContext.Coaching.Update(getval);
                                //_dbContext.Entry(getval).State = EntityState.Modified;
                            }
                            else
                            {
                                _dbContext.Coaching.Add(cc1);
                            }
                        }
                        coachinglist.Clear();
                    }
                }
                if (this != null)
                {
                    var getdata = _dbContext.Coachingpairupshdr.AsNoTracking().Where(c => c.EmployeeId == this.EmployeeId && c.Position == this.Position && c.Pllevel == this.Pllevel && c.Plname == this.Plname).FirstOrDefault();
                    if (getdata != null)
                    {
                        getdata.Approve2 = "Y";
                        getdata.AEDAT = this.AEDAT;
                        getdata.AENAM = this.AENAM;
                        getdata.Reasons = this.Reasons;
                    }
                    _dbContext.Coachingpairupshdr.Update(getdata);
                    _dbContext.Entry(getdata).State = EntityState.Modified;
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
               // return null;
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
    }
}
