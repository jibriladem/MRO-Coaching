using System;
using System.Collections.Generic;
using System.Text;

namespace MROCoatching.DataObjects.Models.Others
{
    public class MenuConstractor
    {
        public long Id { get; set; }
        public string Area { get; set; }
        public string Category { get; set; } // Masterdata, Operational, Account Management
        public string Controller { get; set; } // BalancedScorecard, EPDMS, Evaluation
        public string Action { get; set; } //Index (CRUD) & others 
        public string Privilages { get; set; } //Index (CRUD) & others 
    }
}
