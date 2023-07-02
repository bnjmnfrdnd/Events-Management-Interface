using EventsManagementInterface.Data.Models;
using System.Globalization;

namespace EventsManagementInterface.Data.Services
{
    public class VendorService
    {
        private LogService logService;
        private ApplicationDbContext database;
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public VendorService(ApplicationDbContext database, LogService logService)
        {
            this.database = database;
            this.logService = logService;
        }


        public async Task<VendorInputModal> SubmitVendorInput(VendorInput vendorInput)
        {
            Attendee attendee = database.Attendee.SingleOrDefault(x => x.GuestIdentificationNumber == vendorInput.GuestIdentificationNumber);
            VendorInputModal vendorInputModal = new VendorInputModal();

            if (attendee == null) {

                vendorInputModal = new VendorInputModal
                {
                    GuestIdentificationNumber = vendorInput.GuestIdentificationNumber,
                    GuestName = "",
                    Success = false,
                    Errors = new List<string> {
                        $"Guest Identificiation Number ({vendorInput.GuestIdentificationNumber}) does not exist"
                    },
                    Message = "Please try again",
                    Title = "Error",
                    TokensRemaining = new List<string>()
                };

                return vendorInputModal;
            }

            attendee.AlcoholicDrinkTokenAllowance -= vendorInput.AlcoholicDrinkToken;
            attendee.NonAlcoholicDrinkTokenAllowance -= vendorInput.NonAlcoholicDrinkToken;
            attendee.FoodTokenAllowance -= vendorInput.FoodToken;

            vendorInputModal = new VendorInputModal
            {
                GuestIdentificationNumber = vendorInput.GuestIdentificationNumber,
                GuestName = $"{attendee.FirstName} {attendee.LastName}",
                Success = true,
                Errors = new List<string>(),
                TokensRemaining = new List<string>
                {
                    $"Alcohol tokens: {attendee.AlcoholicDrinkTokenAllowance}",
                    $"Non-Alcohol tokens: {attendee.NonAlcoholicDrinkTokenAllowance}",
                    $"Food tokens: {attendee.FoodTokenAllowance}"
                },
                Message = $"Tokens have been deducted for {attendee.FirstName} {attendee.LastName}",
                Title = "Success"
            };

            database.Update(attendee);
            database.SaveChanges();

            return vendorInputModal;
        }
    }
} 