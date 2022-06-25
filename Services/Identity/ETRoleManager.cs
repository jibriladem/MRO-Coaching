using MROCoatching.Abstractions.Identity;
using MROCoatching.DataObjects.Models.UserManagment.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace MROCoatching.DataObjects.Services.Identity
{
    public class ETRoleManager : IETRoleManager
    {
        readonly IServiceProvider serviceProvider;
        RoleManager<ApplicationRole> roleManager;

        public ETRoleManager(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider;

            var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            roleManager = serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
        }

        public async Task<bool> CreateRole(string roleName, string description)
        {
            var idResult = await roleManager.CreateAsync(new ApplicationRole(roleName, description));
            return idResult.Succeeded;
        }

        public async Task<bool> RoleExists(string roleName)
        {
            return await roleManager.RoleExistsAsync(roleName);
        }
    }
}
