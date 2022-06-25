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
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.DataObjects.Models.BasicTransactions
{
    [Table("COACHANNUALPLANS")]
    public class Coachannualplans : Audit
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
        [Display(Name = "Organisational Units"), Required, StringLength(100)]
        public string Organisationunit { get; set; }
        [Display(Name = "Cost Center Code"), Required, StringLength(10)]
        public string Costcentercode { get; set; }
        [NotMapped, Display(Name = "Cost Center Code")]
        public string Costcentercode1 { get; set; }
        [NotMapped, Display(Name = "Cost Center Code")]
        public string Costcentercode2 { get; set; }
        [Display(Name = "Cost Center Description"), Required, StringLength(100)]
        public string Costcenterdesc { get; set; }
        [Display(Name = "Last Performance ISC Result"), StringLength(10)]
        public string Lastperformisc { get; set; }
        [Display(Name = "Recent Performance ISC Result"), StringLength(10)]
        public string Recentperformisc { get; set; }
        [Display(Name = "Last Promotion Date")]
        public DateTime Lastpromotiondate { get; set; }
        [Display(Name = "Next Promotion Date")]
        public DateTime Nextpromotiondate { get; set; }
        [Display(Name = "Coach ID"), Required, StringLength(10)]
        public string Coachid { get; set; }
        [Display(Name = "Coach Name"), Required, StringLength(100)]
        public string Coachname { get; set; }
        [Display(Name = "Coaching Start Date")]
        public DateTime Coachingstartdate { get; set; }
        [Display(Name = "Coaching End Date")]
        public DateTime Coachingenddate { get; set; }
        [Display(Name = "When to Assess")]
        public DateTime Whentoassess { get; set; }
        [Display(Name = "Assessor ID"), Required, StringLength(10)]
        public string Assessorid { get; set; }
        [Display(Name = "Assessor Name"), Required, StringLength(100)]
        public string Assessorname { get; set; }
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
        [NotMapped]
        public List<Coachannualplans> actualpls { get; set; }
        [NotMapped]
        public List<Items> Items { get; set; }
        public List<Coaching> Coaching { get; set; }
        [NotMapped]
        public List<Coachannualplans> Coaches { get; set; }
        [NotMapped]
        public List<Coachannualplans> Assessors { get; set; }
        public Coachannualplans()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public Coachannualplans(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbContext.Coachannualplans.Add(this);
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
        public async Task<object> SaveList(List<Coachingpairupsdtl> coachingpairupsdtls, Coachingpairupshdr coachingpairupshdr)
        {
            try
            {
                var coachhdr = coachingpairupshdr;
                _dbContext.Coachingpairupshdr.Add(coachhdr);
                _dbContext.Entry(coachhdr).State = EntityState.Added;
                //if (await _dbContext.SaveChangesAsync() > 0)
                //{
                if (coachingpairupsdtls.Count > 0)
                {
                    _dbContext.Coachingpairupsdtl.AddRange(coachingpairupsdtls);
                    //_dbContext.Entry(coachingpairupsdtls).State = EntityState.Added;
                }
                //foreach (var vals in coachingpairupsdtls)
                //{
                //    var getvals = _dbContext.Coachingpairupsdtl.Where(con => con.EmployeeId == vals.EmployeeId && con.Coachid == vals.Coachid && con.Position == vals.Position && con.Plname == vals.Plname && con.Pllevel == vals.Pllevel).FirstOrDefault();
                //    if (getvals != null)
                //    {
                //        getvals.AEDAT = DateTime.Now;
                //        getvals.AENAM = vals.AENAM;
                //        _dbContext.Coachingpairupsdtl.Update(getvals);
                //        _dbContext.Entry(getvals).State = EntityState.Modified;
                //    }
                //    else
                //    {
                //        _dbContext.Coachingpairupsdtl.Add(vals);
                //        _dbContext.Entry(vals).State = EntityState.Added;
                //    }
                //}
                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return new DatabaseOperationResponse
                    {
                        Status = MROCoatching.DataObjects.Models.Others.OperationStatus.SUCCESS,
                        Message = Resource.RecordSucessfullyCreated,
                    };
                }

                //}

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
        public async Task<object> SaveList1(List<Coachannualplans> coachannualplans)
        {
            var coachinglist = new List<Coaching>();
            try
            {
                foreach (var vals in coachannualplans)
                {
                    vals.Items = _dbContext.Items.AsNoTracking().Where(con => con.Plcode == vals.Plname).ToList();
                    if (vals.Items.Count > 0)
                    {
                        foreach (var cc in vals.Items)
                        {
                            coachinglist.Add(new Coaching()
                            {
                                EmployeeId = vals.EmployeeId,
                                Employeename = vals.Employeename,
                                Coachid = vals.Coachid,
                                Coachname = vals.Coachname,
                                Taskcode = cc.Taskcode,
                                Taskdesc = cc.Description,
                                Plcode = cc.Plcode,
                                Plname = cc.Description,
                                Pllevel = cc.Pltypes,
                                Traineeapproval = "",
                                Coachapproval = "",
                                Status = "N",
                                Dateapproved = DateTime.Now,
                                ERDAT = DateTime.Now,
                                ERNAM = vals.ERNAM,
                                AEDAT = DateTime.Now,
                                AENAM = vals.AENAM,
                            });
                        }
                    }
                    if (coachinglist.Count > 0)
                    {
                        foreach (var cc1 in coachinglist)
                        {
                            var getval = _dbContext.Coaching.AsNoTracking().Where(c => c.EmployeeId == cc1.EmployeeId && c.Coachid == cc1.Coachid && c.Taskcode == cc1.Taskcode).FirstOrDefault();
                            if (getval != null)
                            {
                                _dbContext.Coaching.Update(cc1);
                                _dbContext.Entry(cc1).State = EntityState.Modified;
                            }
                            else
                            {
                                _dbContext.Coaching.Add(cc1);
                            }
                        }
                        coachinglist.Clear();
                    }
                    _dbContext.Coachannualplans.Update(vals);
                    _dbContext.Entry(vals).State = EntityState.Modified;
                }
                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return new DatabaseOperationResponse
                    {
                        Status = MROCoatching.DataObjects.Models.Others.OperationStatus.SUCCESS,
                        Message = Resource.RecordSucessfullyApproved,
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
                _dbContext.Coachannualplans.Attach(this);
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
                return await _dbContext.Coachannualplans.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId);
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
                var regstr = await _dbContext.Coachannualplans.FirstOrDefaultAsync(con => con.EmployeeId == EmployeeId && con.Plname == Plname && con.Position == Position);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}
