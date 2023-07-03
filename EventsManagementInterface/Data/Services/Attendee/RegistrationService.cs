using EventsManagementInterface.Data.Models.Administration;
using EventsManagementInterface.Data.Models.Attendee;
using EventsManagementInterface.Data.Models;
using System.Globalization;

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

        public RegistrationModal RegisterAttendee(Registration registration)
        {
            try
            {
                RegistrationModal registrationModal = ValidateRegistration(registration);

                if (!registrationModal.Success) 
                {
                    return registrationModal;
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
                    AlcoholicDrinkTokenAllowance = registration.NumberOfGuests * 2,
                    NonAlcoholicDrinkTokenAllowance = registration.NumberOfGuests * 2,
                    FoodTokenAllowance = registration.NumberOfGuests * 2,
                    Archived = false,
                };
                
                while (guestIdentificationNumberExists)
                {
                    newGuestIdentificationNumber = random.Next(1000, 9999);
                    guestIdentificationNumberExists = IsGuestIdentifdicationNumberInUse(newGuestIdentificationNumber);
                }
                
                attendee.GuestIdentificationNumber = newGuestIdentificationNumber;
                registrationModal.GuestIdentificationNumber = newGuestIdentificationNumber;

                database.Add(attendee);
                database.SaveChanges();

                return registrationModal;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return null;
            }
        }

        public RegistrationModal ValidateRegistration(Registration registration)
        {
            RegistrationModal registrationModal = new RegistrationModal();
            registrationModal.Errors = new List<string>();
            registrationModal.Success = true;

            if (registration.EmailAddress == null || registration.EmailAddress == String.Empty)
            {
                registrationModal.Success = false;
                registrationModal.Errors.Add("A Coloplast email address must be provided.");
            }
            else
            {
                if (!registration.EmailAddress.ToLower().Contains("@coloplast.com"))
                {
                    registrationModal.Success = false;
                    registrationModal.Errors.Add("A Coloplast email address must be provided.");
                }
            }            

            if (registration.FirstName == null || registration.FirstName == String.Empty)
            {
                registrationModal.Success = false;
                registrationModal.Errors.Add("A first name must be provided.");
            }

            if (registration.LastName == null || registration.LastName == String.Empty)
            {
                registrationModal.Success = false;
                registrationModal.Errors.Add("A last name must be provided.");
            }    
            
            if (registration.NumberOfGuests == 0)
            {
                registrationModal.Success = false;
                registrationModal.Errors.Add("Number of guests must be provided.");
            }

            //if (registration.DateOfBirth == DateTime.Now.AddYears(-35))
            //{
            //    registrationModal.Success = false;
            //    registrationModal.Errors.Add("Please provide a date of birth.");
            //}

            if (IsEmailAddressInUse(registration.EmailAddress))
            {
                registrationModal.Success = false; 
                registrationModal.Errors.Add("This email address has already been used.");
            }

            if (registrationModal.Success)
            {
                registrationModal.Title = "Your registration has been successful.";
                registrationModal.Message = "An email has been sent to you, with your unique Guest Identification Number (GIN). Your GIN is also in bold below.";
            }
            else
            {
                registrationModal.Title = "Your registration has not been successful.";
                registrationModal.Message = "Please see the following errors:";
            }

            return registrationModal;
        }

        public bool IsGuestIdentifdicationNumberInUse(int newGuestIdentificationNumber)
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

        public bool IsEmailAddressInUse(string newEmailAddress)
        {
            List<Attendee> attendees = database.Attendee.ToList();
            List<string> emailAddresses = new List<string>();

            foreach (Attendee a in attendees)
            {
                emailAddresses.Add(a.EmailAddress);
            }

            foreach (string emailAddress in emailAddresses)
            {
                if (emailAddress == newEmailAddress)
                {
                    return true;
                }
            }
            return false;
        }
    }
} 