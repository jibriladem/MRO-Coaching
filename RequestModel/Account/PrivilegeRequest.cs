using System;
using System.Collections.Generic;
using System.Text;

namespace MROCoatching.DataObjects
{
	public class PrivilegeRequest 
	{
		public long Id { get; set; }
		public string Action { get; set; }
		public string Module { get; set; }
		public string Description { get; set; }
	}
}
