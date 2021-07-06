using DataAccessLibrary;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBUI
{
    class Program
    {
        private static CosmosDBDataAccess db;

        static async Task Main(string[] args)
        {
            var c = GetCosmosInfo();

            db = new CosmosDBDataAccess(c.endpointUrl, c.primaryKey, c.databaseName, c.containerName);

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

            await CreateContact(user);

            await GetAllContacts();

            await GetContactById("");

            await UpdateContactsFirstName("", "");

            await RemovePhoneNumberFromUser("", "");

            await RemoveUser("", "");

            Console.WriteLine("Cosmos DB");
            Console.ReadLine();
        }

        private static async Task CreateContact(ContactModel contact)
        {
            await db.UpsertRecordAsync(contact);
        }

        private static async Task UpdateContactsFirstName(string firstName, string id)
        {
            var contact = await db.LoadRecordByIdAsync<ContactModel>(id);

            contact.FirstName = firstName;

            await db.UpsertRecordAsync(contact);
        }

        private static async Task RemovePhoneNumberFromUser(string phoneNumber, string id)
        {
            var contact = await db.LoadRecordByIdAsync<ContactModel>(id);

            contact.PhoneNumbers = contact.PhoneNumbers.Where(x => x.PhoneNumber != phoneNumber).ToList();

            await db.UpsertRecordAsync(contact);
        }

        private static async Task RemoveUser(string id, string partitionKey)
        {
            await db.DeleteRecordAsync<ContactModel>(id, partitionKey);
        }

        private static async Task GetAllContacts()
        {
            var contacts = await db.LoadRecordsAsync<ContactModel>();

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

        private static async Task GetContactById(string id)
        {
            var contact = await db.LoadRecordByIdAsync<ContactModel>(id);

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

        private static (string endpointUrl, string primaryKey, string databaseName, string containerName) GetCosmosInfo()
        {
            (string endpointUrl, string primaryKey, string databaseName, string containerName) output;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();

            output.endpointUrl = config.GetValue<string>("CosmosDB:EndpointUrl");
            output.primaryKey = config.GetValue<string>("CosmosDB:PrimaryKey");
            output.databaseName = config.GetValue<string>("CosmosDB:DatabaseName");
            output.containerName = config.GetValue<string>("CosmosDB:ContainerName");

            return output;
        }
    }
}
