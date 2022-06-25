using MROCoatching.Abstractions.Identity;
using MROCoatching.DataObjects.Data.Context;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MROCoatching.DataObjects.Services.Identity
{
    public class ETUserManager : IETUserManager
    {
        readonly IServiceProvider serviceProvider;
        UserManager<ApplicationUser> userManager;
        ApplicationDbContext applicationDbContext;

        public ETUserManager(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider;
            var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

            applicationDbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            applicationDbContext.Database.EnsureCreated();
        }

        public async Task<bool> AddUserToRole(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            var idResult = await userManager.AddToRoleAsync(user, roleName);
            return idResult.Succeeded;
        }

        public async Task ClearUserRoles(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = applicationDbContext.UserRoles.Where(ur => ur.UserId == user.Id).ToList();

                var currentRoles = new List<IdentityUserRole<string>>();

                currentRoles.AddRange(userRoles);

                foreach (var role in currentRoles)
                {
                    string name = applicationDbContext.Roles.FirstOrDefault(x => x.Id == role.RoleId).Name;
                    await userManager.RemoveFromRoleAsync(user, name);
                }
            }
        }

        public async Task DeleteRole(string roleId)
        {
            var roleUsers = applicationDbContext.Users.Where(u => u.UserRoles.Any(r => r.RoleId == roleId));
            var role = applicationDbContext.Roles.Find(roleId);

            foreach (var user in roleUsers)
                await RemoveFromRole(user.Id, role.Name);

            applicationDbContext.Roles.Remove(role);
            applicationDbContext.SaveChanges();
        }

        public async Task RemoveFromRole(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            await userManager.RemoveFromRoleAsync(user, roleName);
        }
        public async Task<string> GetUserRolesAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            string roleIds = "";
            if (user != null)
            {
                var userRoles = applicationDbContext.UserRoles.Where(ur => ur.UserId == user.Id).ToList();

                var currentRoles = new List<IdentityUserRole<string>>();

                currentRoles.AddRange(userRoles);

                foreach (var role in currentRoles)
                {
                    string roleId = applicationDbContext.Roles.FirstOrDefault(x => x.Id == role.RoleId).Id;
                    roleIds = roleId;
                }
                return roleIds;
            }
            return roleIds;
        }
    }
}
