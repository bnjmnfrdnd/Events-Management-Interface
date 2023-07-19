using EventsManagementInterface.Data.Models;
using EventsManagementInterface.Data.Models.Administration;
using EventsManagementInterface.Data.Models.Attendee;
using EventsManagementInterface.Data.Models.Vendor;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

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
                    Subject = "Coloplast Fun Day Allowance",
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

        public async Task<double> GetTotalAvailableAlcoholDrinkTokens()
        {
            List<Attendee> attendees = database.Attendee.Where(x => x.Archived == false).ToList();
            double totalAvailableAlcoholDrinkTokens = 0;

            foreach(Attendee attendee in attendees)
            {
                totalAvailableAlcoholDrinkTokens += attendee.AlcoholicDrinkTokenAllowance;
            }

            return totalAvailableAlcoholDrinkTokens;
        }

        public async Task<double> GetTotalAvailableNonAlcoholDrinkTokens()
        {
            List<Attendee> attendees = database.Attendee.Where(x => x.Archived == false).ToList();
            double totalAvailableNonAlcoholDrinkTokens = 0;

            foreach (Attendee attendee in attendees)
            {
                totalAvailableNonAlcoholDrinkTokens += attendee.NonAlcoholicDrinkTokenAllowance;
            }

            return totalAvailableNonAlcoholDrinkTokens;
        }

        public async Task<double> GetTotalAvailableFoodTokens()
        {
            List<Attendee> attendees = database.Attendee.Where(x => x.Archived == false).ToList();
            double totalAvailableFoodTokens = 0;

            foreach (Attendee attendee in attendees)
            {
                totalAvailableFoodTokens += attendee.FoodTokenAllowance;
            }

            return totalAvailableFoodTokens;
        }

        public async Task<BaseModal> UploadAttendeeData(InputFileChangeEventArgs document)
        {
            try
            {
                var attendee = new Attendee();
                List<Attendee> attendees = new List<Attendee>();
                MemoryStream ms = new MemoryStream();
                await document.File.OpenReadStream().CopyToAsync(ms);
                var bytes = ms.ToArray();
                string commaSeperatedValues = System.Text.Encoding.UTF8.GetString(bytes.ToArray()); // need to convert to string array

                List<string> lines = commaSeperatedValues.Split("\n").ToList();

                foreach (string line in lines)
                {
                    string[] words = line.Split(',');

                    if (words != null)
                    {
                        attendee = new Attendee
                        {
                            FirstName = words[0],
                            LastName = words.Length > 1 ? words[1] : "",
                            EmailAddress = words.Length > 2 ? words[2] : "",
                            AlcoholicDrinkTokenAllowance = words.Length > 3 ? Int32.Parse(words[3]) : 2,
                            NonAlcoholicDrinkTokenAllowance = words.Length > 4 ? Int32.Parse(words[4]) : 2,
                            FoodTokenAllowance = words.Length > 5 ? Int32.Parse(words[5]) : 2
                        };

                        if (attendee.EmailAddress != null || attendee.EmailAddress != "")
                        {
                            attendees.Add(attendee);
                        }

                    }
                }

                BaseModal baseModal = new BaseModal
                {
                    Success = true,
                    Title = "Success",
                    Message = $"An email containing designated GINs and token allowances has been sent to all guests. Total guests: {attendees.Count}",                    
                };

                return baseModal;
            }
            catch(Exception exception) 
            {
                Utility.SendExceptionThrownEmail("UploadAttendeeData", exception);

                BaseModal baseModal = new BaseModal
                {
                    Success = false,
                    Title = "Error",
                    Message = $"there has been an error uploading the data. An email has been sent to the system admin.",
                    Errors = new List<string> { exception.Message }    
                };

                return baseModal;
                
            }
        }
    }
}