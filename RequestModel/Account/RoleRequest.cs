using System.Collections.Generic;

namespace MROCoatching.DataObjects
{
    public class RoleRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<long> Privileges { get; set; }
    }
}
