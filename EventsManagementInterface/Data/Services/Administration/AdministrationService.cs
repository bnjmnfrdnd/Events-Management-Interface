using EventsManagementInterface.Data.Models;
using EventsManagementInterface.Data.Models.Administration;
using EventsManagementInterface.Data.Models.Attendee;
using EventsManagementInterface.Data.Models.Vendor;
using Microsoft.EntityFrameworkCore;

namespace EventsManagementInterface.Data.Services
{
    public class AdministrationService
    {

        private ApplicationDbContext database;
        private Utility utility;

        public AdministrationService(ApplicationDbContext database, Utility utility)
        {
            this.database = database;
            this.utility = utility;
        }

        public async Task<List<Attendee>> GetAttendees()
        {
            return await database.Attendee.ToListAsync();
        }

        public async Task<BaseModal> CheckTokenAllowance(GuestManagement guestManagement)
        {
            Attendee attendee = database.Attendee.SingleOrDefault(x => x.GuestIdentificationNumber == guestManagement.GuestIdentificationNumber);
            BaseModal baseModal;

            if (attendee != null)
            {
                baseModal = new BaseModal
                {
                    Success = true,
                    Title = "Success",
                    Message = $"Please see token allowance below. An email has also been sent to {attendee.EmailAddress}.",
                    TokensRemaining = new List<string>
                    {
                        $"Alcoholic drink tokens: {attendee.AlcoholicDrinkTokenAllowance}",
                        $"Non-Alcoholic drink tokens: {attendee.NonAlcoholicDrinkTokenAllowance}",
                        $"Food tokens: {attendee.FoodTokenAllowance}"
                    }
                };

                Utility.SendEmail(new Models.Email.Email
                {
                    Recipient = attendee.EmailAddress,
                    Subject = "Coloplast Fund Day Allowance",
                    HTMLMessage = 
                    $"Hi {attendee.FirstName}, " +
                    $"<br/><br/> " +
                    $"Your remaining token allowance is: <ul>" +
                    $"<li>Alcoholic Drink Tokens: {attendee.AlcoholicDrinkTokenAllowance}</li>" +
                    $"<li>Non-Alcoholic Drink Tokens: {attendee.NonAlcoholicDrinkTokenAllowance}</li>" +
                    $"<li>Food Tokens: {attendee.FoodTokenAllowance}</li>" +
                    $"</ul>" +
                    $"<br/>" +
                    $"Kind regards, <br>" +
                    $"Coloplast Fun Day Team"
                });

                return baseModal;
            }
            else
            {
                baseModal = new BaseModal
                {
                    Success = false,
                    Title = "Error",
                    Message = "There was a problem with this request, please see the errors below",
                    Errors = new List<string>()
                    {
                        $"Guest Identificiation Number ({guestManagement.GuestIdentificationNumber}) does not exist"
                    }
                };

                return baseModal;
            }
        }

        public async Task<BaseModal> UpdateTokenAllowance(GuestManagement guestManagement)
        {
            Attendee attendee = database.Attendee.SingleOrDefault(x => x.GuestIdentificationNumber == guestManagement.GuestIdentificationNumber);
            BaseModal baseModal;

            if (attendee != null)
            {
                attendee.AlcoholicDrinkTokenAllowance = guestManagement.AlcoholicDrinkTokenAllowance == null ? attendee.AlcoholicDrinkTokenAllowance : guestManagement.AlcoholicDrinkTokenAllowance.Value;
                attendee.NonAlcoholicDrinkTokenAllowance = guestManagement.NonAlcoholicDrinkTokenAllowance == null ? attendee.NonAlcoholicDrinkTokenAllowance : guestManagement.NonAlcoholicDrinkTokenAllowance.Value;
                attendee.FoodTokenAllowance = guestManagement.FoodTokenAllowance == null ? attendee.FoodTokenAllowance : guestManagement.FoodTokenAllowance.Value;

                baseModal = new BaseModal
                {
                    Success = true,
                    Title = "Success",
                    Message = $"Token allowance has been updated. An email has been sent to {attendee.EmailAddress}.",
                };

                database.Update(attendee);
                database.SaveChanges();

                Utility.SendEmail(new Models.Email.Email
                {
                    Recipient = attendee.EmailAddress,
                    Subject = "Coloplast Fund Day Allowance",
                    HTMLMessage =
                    $"Hi {attendee.FirstName}, " +
                    $"<br/><br/> " +
                    $"Your new token allowance is: <ul>" +
                    $"<li>Alcoholic Drink Tokens: {attendee.AlcoholicDrinkTokenAllowance}</li>" +
                    $"<li>Non-Alcoholic Drink Tokens: {attendee.NonAlcoholicDrinkTokenAllowance}</li>" +
                    $"<li>Food Tokens: {attendee.FoodTokenAllowance}</li>" +
                    $"</ul>" +
                    $"<br/>" +
                    $"Kind regards, <br>" +
                    $"Coloplast Fun Day Team"
                });

                return baseModal;
            }
            else
            {
                baseModal = new BaseModal
                {
                    Success = false,
                    Title = "Error",
                    Message = "The GIN number has not been found",
                };

                return baseModal;
            }
        }
    }
}