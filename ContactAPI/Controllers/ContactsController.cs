using DataAccessLibrary;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ContactAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly MongoDBDataAccess _db;
        private readonly string _tableName = "Contacts";
        private readonly IConfiguration _config;

        public ContactsController(IConfiguration config)
        {
            _config = config;
            _db = new MongoDBDataAccess("MongoContactsDB", _config.GetConnectionString("Default"));
        }

        [HttpGet]
        public List<ContactModel> GetAll()
        {
            return _db.LoadRecords<ContactModel>(_tableName);
        }

        [HttpPost]
        public void InsertRecord(ContactModel contact)
        {
            _db.UpsertRecord(_tableName, contact.Id, contact);
        }
    }
}
