using System.Collections.Generic;

namespace MROCoatching.DataObjects
{
    public class UserRequest
    {
        public long Id { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsSuperAdmin { get; set; }
        public List<long> Roles { get; set; }
    }
}
