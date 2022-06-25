using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MROCoatching.DataObjects.Models.UserManagment.Identity
{
    /// <summary>
    /// Privilege base model
    /// </summary>
    public class ApplicationPrivilege
    {
        public string Id { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Role privilege base model
    /// </summary>
    public class ApplicationRolePrivilege
    {
        public string RoleId { get; set; }
        public string PrivilegeId { get; set; }
        public virtual ApplicationPrivilege Privilage { get; set; }
    }

    /// <summary>
    /// Role base model (inherits Identity Role model
    /// </summary>
    public class ApplicationRole : IdentityRole<string>
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string name, string _description)
            : base(name)
        {
            Description = _description;
        }
        public virtual string Description { get; set; }
        public virtual ICollection<ApplicationRolePrivilege> RolePrivileges { get; set; }
    }

    /// <summary>
    /// User role base model (inherits Identity User Role model)
    /// </summary>
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public ApplicationUserRole()
            : base()
        { }
        public virtual ApplicationRole Role { get; set; }
    }

    /// <summary>
    /// User base model (inherits Identity User model)
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            FirstLogin = true;
        }
        public bool FirstLogin { get; set; }
        public string FullName { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}