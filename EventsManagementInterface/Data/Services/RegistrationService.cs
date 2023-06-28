using EventsManagementInterface.Data;
using Microsoft.EntityFrameworkCore;
using EventsManagementInterface.Data.Migrations;
using EventsManagementInterface.Data.Models;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace EventsManagementInterface.Data.Services
{
    public class RegistrationService
    {
        private LogService logService;
        private ApplicationDbContext database;
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public RegistrationService(ApplicationDbContext database, LogService logService)
        {
            this.database = database;
            this.logService = logService;
        }

        public async Task<bool> RegisterAttendee(Registration registration)
        {
            Attendee attendee = new Attendee
            {
                FirstName = registration.FirstName,
                MiddleName = registration.MiddleName,
                LastName = registration.LastName,
                DateOfBirth = registration.DateOfBirth,
                EmailAddress = registration.EmailAddress,
                NumberOfGuests = registration.NumberOfGuests,
                CreatedDateTime = DateTime.Now,
                DrinkTokenAllowance = registration.NumberOfGuests * 2,
                FoodTokenAllowance = registration.NumberOfGuests * 2,
                Archived = false,
            };

            //logService.CreateLog();

            return false;
        }
    }
} 