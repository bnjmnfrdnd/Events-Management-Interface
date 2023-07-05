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

        public BaseModal RegisterAttendee(Registration registration)
        {
            try
            {
                BaseModal registrationModal = ValidateRegistration(registration);

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
                
                bool emailSent = Utility.SendEmail(new Models.Email.Email
                {
                    Recipient = attendee.EmailAddress,
                    Subject = "Coloplast Fund Day Registration",
                    HTMLMessage =
                    $"Hi {attendee.FirstName}, " +
                    $"<br/><br/> " +
                    $"Thank you for registering your attendance for the Coloplast Fun Day 2023!" +
                    $"<br/><br/>" +
                    $"Your Guest Identification Number (GIN) is: <b>{attendee.GuestIdentificationNumber}</b>. " +
                    $"<br/><br/>" +
                    $"Please dont forget it; you will need to inform the vendor of your GIN when you want to redeem your drink and food tokens." +
                    $"<br/><br/>" +
                    $"Your token allowance is: " +
                    $"<ul>" +
                    $"<li>Alcoholic Drink Tokens: {attendee.AlcoholicDrinkTokenAllowance}</li>" +
                    $"<li>Non-Alcoholic Drink Tokens: {attendee.NonAlcoholicDrinkTokenAllowance}</li>" +
                    $"<li>Food Tokens: {attendee.FoodTokenAllowance}</li>" +
                    $"</ul>" +
                    $"<br/>" +
                    $"Please contact People & Culture for any queries." +
                    $"<br><br/>" +
                    $"Kind regards, <br>" +
                    $"Coloplast Fun Day Team"  
                });

                if (emailSent)
                {
                    attendee.GuestIdentificationNumberEmailSent = true;
                    database.Add(attendee);
                    database.SaveChanges();

                    return registrationModal;
                }
                else
                {
                    return registrationModal;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return null;
            }
        }

        public BaseModal ValidateRegistration(Registration registration)
        {
            BaseModal registrationModal = new();
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
                registrationModal.Message = "An email has been sent to you with your 4-digit Guest Identification Number (GIN).";
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