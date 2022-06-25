using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.Others;
using MROCoatching.DataObjects.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.DataObjects.Models.AccountMaster
{
    public class Menus : Audit
    {
        public Menus()
        {

        }
        [Key]
        public long MenuId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        [ForeignKey("ParentMenuId")]
        public long? ParentId { get; set; }
        public string Privilages { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public bool Favorite { get; set; }

        public virtual Menus ParentMenuId { get; set; }
        [NotMapped]
        private ApplicationDbContext applicationDbContext;
        public Menus(ApplicationDbContext _applicationDbContext)
        {
            applicationDbContext = _applicationDbContext;
            applicationDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public async Task<List<Menus>> GetList()
        {
            return await applicationDbContext.Menus.ToListAsync();
        }
        public object Save()
        {
            try
            {
                applicationDbContext.Menus.Add(this);
                if (!Exist())
                {
                    if (applicationDbContext.SaveChanges() > 0)
                    {

                        return new DatabaseOperationResponse
                        {
                            Status = Others.OperationStatus.SUCCESS,
                            Message = "successfully inserted."
                        };

                    }
                    return new DatabaseOperationResponse
                    {
                        Message = "was not successfully inserted.",
                        Status = Others.OperationStatus.ERROR
                    };
                }
                else
                {
                    return new DatabaseOperationResponse
                    {

                        Message = "Already Exist", //Resources.RecordAlreadyExist,
                        Status = Others.OperationStatus.NOT_OK
                    };
                }

            }
            catch (Exception ex)
            {
                return new DatabaseOperationResponse
                {
                    ex = ex,
                    Message = "Error occured while inserting the record",
                    Status = Others.OperationStatus.ERROR,
                };
            }
        }

        public object Update()
        {
            try
            {
                applicationDbContext.Menus.Attach(this);
                applicationDbContext.Entry(this).State = EntityState.Modified;

                if (applicationDbContext.SaveChanges() > 0)
                {
                    return new DatabaseOperationResponse
                    {
                        Status = Others.OperationStatus.SUCCESS,
                        Message = "successfully updated."
                    };

                }
                else
                {
                    return new DatabaseOperationResponse
                    {
                        Message = "was not successfully updated.",
                        Status = Others.OperationStatus.Ok
                    };
                }
            }
            catch (Exception ex)
            {
                return new DatabaseOperationResponse
                {
                    ex = ex,
                    Message = "Error occured while updating the",
                    Status = Others.OperationStatus.ERROR,
                };
            }
        }
        public object Delete()
        {
            try
            {
                applicationDbContext.Remove(this);
                if (applicationDbContext.SaveChanges() > 0)
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
                    Message = "Error occured while deleting the",
                    Status = Others.OperationStatus.ERROR,
                };
            }
        }

        public bool Exist()
        {

            try
            {
                var menusFind = applicationDbContext.Menus.Where(c => c.Name == Name && c.ParentId == ParentId)?.FirstOrDefault();
                if (menusFind != null)
                {
                    if (menusFind.ACTIND != RecordStatus.Active)
                    {
                        menusFind.ACTIND = RecordStatus.Active;
                        applicationDbContext.Menus.Attach(menusFind);
                        applicationDbContext.Entry(menusFind).State = EntityState.Modified;
                        applicationDbContext.SaveChanges();
                    }
                    return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
