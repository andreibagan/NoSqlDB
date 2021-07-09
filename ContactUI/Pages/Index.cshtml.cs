using ContactUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContactUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGet()
        {
            await CreateContact();
            await GetAllContacts();
        }

        private async Task CreateContact()
        {
            ContactModel contact = new ContactModel
            {
                FirstName = "Evgeniy",
                LastName = "Strelkov"
            };

            contact.EmailAddresses.Add(new EmailAddressModel { EmailAddress = "strelkov2321@mail.ru" });
            contact.EmailAddresses.Add(new EmailAddressModel { EmailAddress = "strelkov2ge1@mail.ru" });
            contact.EmailAddresses.Add(new EmailAddressModel { EmailAddress = "strelkov412421@mail.ru" });
            contact.PhoneNumbers.Add(new PhoneNumberModel { PhoneNumber = "+375297832432" });
            contact.PhoneNumbers.Add(new PhoneNumberModel { PhoneNumber = "+375298371829" });

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync("https://localhost:44391/api/Contacts", 
                new StringContent(JsonSerializer.Serialize(contact), 
                Encoding.UTF8, "application/json"));


        }

        private async Task GetAllContacts()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:44391/api/Contacts");

            List<ContactModel> contacts;

            if(response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                string responseText = await response.Content.ReadAsStringAsync();
                contacts = JsonSerializer.Deserialize<List<ContactModel>>(responseText, options);
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
