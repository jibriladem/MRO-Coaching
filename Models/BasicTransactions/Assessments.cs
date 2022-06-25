using Microsoft.EntityFrameworkCore;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.AccountMaster;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MROCoatching.DataObjects.Models.AssessingTable
{
    public class Assessments : Audit
    {
        public long Id { get; set; }
        public string Taskcode { get; set; }
        public string Taskdesc { get; set; }
        public long? Taskcount { get; set; }
        public string EmployeeId { get; set; }
        public string Employeename { get; set; }
        public string AssessorId { get; set; }
        public string Assessorname { get; set; }
        public string CoachId { get; set; }
        public string Coachname { get; set; }
        public string Referencecode { get; set; }
        public string TSFNnumbers { get; set; }
        public DateTime Dateofassessment { get; set; }
        public string Traineeapprovals { get; set; }
        public string Coachapprovals { get; set; }
        public string Assessorapprovals { get; set; }
        public DateTime Apprvoaldate { get; set; }
        public string Plname { get; set; }
        public string Pldescriptions { get; set; }
        public string Posname { get; set; }
        public string Posdescriptions { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public Assessments()
        {

        }
        public ApplicationDbContext _dbContext { get; set; }
        public Assessments(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;

        }
        public async Task<object> Save()
        {
            try
            {
                _dbContext.Assessments.Add(this);
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
        public async Task<object> Update()
        {
            try
            {
                //this.AEDAT = DateTime.Now;
                _dbContext.Assessments.Attach(this);
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
        public async Task<object> Refresh()
        {
            try
            {
                return await _dbContext.Assessments.FirstOrDefaultAsync(con => con.Taskcode == Taskcode && con.Taskcount == Taskcount && con.Plname == Plname);
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
                var regstr = await _dbContext.Assessments.FirstOrDefaultAsync(con => con.Taskcode == Taskcode && con.Taskcount == Taskcount && con.Plname == Plname);
                return regstr == null ? false : true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

    }
}

