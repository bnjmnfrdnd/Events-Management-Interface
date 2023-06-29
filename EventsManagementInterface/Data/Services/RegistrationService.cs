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
            try
            {
                if(!registration.EmailAddress.ToLower().Contains("@coloplast.com"))
                {
                    return false;
                }

                Random random = new Random();
                int newGuestIdentificationNumber = 0;
                bool guestIdentificationNumberExists = true;

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
                
                while (guestIdentificationNumberExists)
                {
                    newGuestIdentificationNumber = random.Next(1000, 9999);
                    guestIdentificationNumberExists = DoesGuestIdentifdicationNumberExist(newGuestIdentificationNumber);
                }
                
                attendee.GuestIdentificationNumber = newGuestIdentificationNumber;

                database.Add(attendee);
                database.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DoesGuestIdentifdicationNumberExist(int newGuestIdentificationNumber)
        {
            List<Attendee> attendees = database.Attendee.ToList();
            List<int> guestIdentificationNumbers = new List<int>();

            foreach (Attendee a in attendees)
            {
                guestIdentificationNumbers.Add(a.GuestIdentificationNumber);
            }

            foreach (int guestIdentificationNumber in guestIdentificationNumbers)
            {
                if (guestIdentificationNumber == newGuestIdentificationNumber)
                {
                    return true;
                }
            }
            return false;
        }
    }
} 