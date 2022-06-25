using System;
using System.Collections.Generic;
using System.Text;

namespace MROCoatching.DataObjects.RequestModel.Personal
{
    public class PersonRequest
    {
        public long PersonId { get; set; }
        public string FisrtName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string Height { get; set; }
        public string EyeColor { get; set; }
        public string HairColor { get; set; }
        public string Occupation { get; set; }
        public string HalfCast { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public string BirthCountry { get; set; }
        public string BirthCity { get; set; }
        public string PhotoPath { get; set; }
        public long CommunicationMethodId { get; set; }
        public string ApplicationID { get; set; }
        public string OrganizationID { get; set; }
        public long? UserId { get; set; }
        public AddressRequest Address { get; set; }
        public List<FamilyRequest> FamilyRequests { get; set; }

    }
    public class AddressRequest
    {
        public long PersonId { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Zone { get; set; }
        public string Wereda { get; set; }
        public string Street { get; set; }
        public string HouseNo { get; set; }
        public string POBox { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string RequestPlace { get; set; }
    }
    public class FamilyRequest
    {
        public long PersonId { get; set; }
        public long FamiltyTypeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
