using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MROCoatching.DataObjects.Models.AccountMaster
{
    public class PasswordStore
    {
        public PasswordStore()
        {
           
        }
        [Key]
        public long Id { get; set; }
        public string Password { get; set; }
    }
}
