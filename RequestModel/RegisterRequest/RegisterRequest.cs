using MROCoatching.DataObjects.RequestModel.Personal;
using System;
using System.Collections.Generic;
using System.Text;

namespace MROCoatching.DataObjects.RequestModel.RegisterRequest
{
   public  class RegisterRequest
    {

        //public long Id { get; set; }
        //public string Username { get; set; }
        //public string Password { get; set; }
        //public string FirstName { get; set; }
        //public string MiddleName { get; set; }
        //      public string LastName { get; set; }
        //public DateTime DateOfBirth { get; set; }
        //public string Gender { get; set; }
        //public string Nationality { get; set; }
        //public string BirthCountry { get; set; }
        //public string BirthCity { get; set; }



        public PersonRequest PersonRequest { get; set; }
        public UserRequest UserRequest { get; set; }
    }
}
