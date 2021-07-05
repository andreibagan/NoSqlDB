using System;
using System.IO;
using System.Linq;
using DataAccessLibrary;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;

namespace MongoDBUI
{
    class Program
    {
        private static MongoDBDataAccess db;
        private static readonly string tableName = "Contacts";

        static void Main(string[] args)
        {
            db = new MongoDBDataAccess("MongoContactsDB", GetConnectionString());

            ContactModel user = new ContactModel
            {
                FirstName = "Vadim",
                LastName = "Baranovskiy",
            };

            user.EmailAddresses.Add(new EmailAddressModel { EmailAddress = "Vadim.Baranovskiy1@mail.ru" });
            user.EmailAddresses.Add(new EmailAddressModel { EmailAddress = "Vadim.Baranovskiy2@mail.ru" });
            user.EmailAddresses.Add(new EmailAddressModel { EmailAddress = "Vadim.Baranovskiy3@mail.ru" });
            user.PhoneNumbers.Add(new PhoneNumberModel { PhoneNumber = "+375242312428" });
            user.PhoneNumbers.Add(new PhoneNumberModel { PhoneNumber = "+375341234796" });

            //CreateContact(user); d16d7301-b714-4778-9688-8434d2c379d4

            //GetAllContacts();

            //GetContactById("d16d7301-b714-4778-9688-8434d2c379d4");

            //UpdateContactsFirstName("Vadik", "d16d7301-b714-4778-9688-8434d2c379d4");

            //RemovePhoneNumberFromUser("+375341234796", "d16d7301-b714-4778-9688-8434d2c379d4");

            RemoveUser("976f5ae6-60b5-4cf8-92f1-2f1883bc6ede"); 

            Console.WriteLine("Mongo DB");
            Console.ReadLine();
        }

        

        private static void CreateContact(ContactModel contact)
        {
            db.UpsertRecord(tableName, contact.Id, contact);
        }

        private static void UpdateContactsFirstName(string firstName, string id)
        {
            Guid guid = new Guid(id);
            var contact = db.LoadRecordById<ContactModel>(tableName, guid);

            contact.FirstName = firstName;

            db.UpsertRecord(tableName, contact.Id, contact);
        }

        private static void RemovePhoneNumberFromUser(string phoneNumber, string id)
        {
            Guid guid = new Guid(id);
            var contact = db.LoadRecordById<ContactModel>(tableName, guid);

            contact.PhoneNumbers = contact.PhoneNumbers.Where(x => x.PhoneNumber != phoneNumber).ToList();

            db.UpsertRecord(tableName, contact.Id, contact);
        }
        
        private static void RemoveUser(string id)
        {
            Guid guid = new Guid(id);
            db.DeleteRecord<ContactModel>(tableName, guid);
        }

        private static void GetAllContacts()
        {
            var contacts = db.LoadRecords<ContactModel>(tableName);

            foreach (var contact in contacts)
            {
                Console.WriteLine($"{contact.Id}: {contact.FirstName} {contact.LastName}");

                foreach (var email in contact.EmailAddresses)
                {
                    Console.WriteLine($"{email.EmailAddress}");
                }

                foreach (var phoneNumber in contact.PhoneNumbers)
                {
                    Console.WriteLine($"{phoneNumber.PhoneNumber}");
                }

                Console.WriteLine();
            }
        }

        private static void GetContactById(string id)
        {
            Guid guid = new Guid(id);
            var contact = db.LoadRecordById<ContactModel>(tableName, guid);

            Console.WriteLine($"{contact.Id}: {contact.FirstName} {contact.LastName}");

            foreach (var email in contact.EmailAddresses)
            {
                Console.WriteLine($"{email.EmailAddress}");
            }

            foreach (var phoneNumber in contact.PhoneNumbers)
            {
                Console.WriteLine($"{phoneNumber.PhoneNumber}");
            }
        }

        private static string GetConnectionString(string nameConnection = "Default")
        {
            string output = string.Empty;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();

            output = config.GetConnectionString(nameConnection);

            return output;
        }
    }
}
