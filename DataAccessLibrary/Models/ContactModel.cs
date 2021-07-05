using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace DataAccessLibrary.Models
{
    public class ContactModel
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<EmailAddressModel> EmailAddresses { get; set; } = new List<EmailAddressModel>();
        public List<PhoneNumberModel> PhoneNumbers { get; set; } = new List<PhoneNumberModel>();
    }
}
