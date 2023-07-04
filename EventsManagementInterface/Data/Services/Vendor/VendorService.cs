using EventsManagementInterface.Data.Models.Attendee;
using EventsManagementInterface.Data.Models.Vendor;
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

        public async Task<BaseModal> SubmitVendorInput(VendorInput vendorInput)
        {
            try
            {
                bool allowanceExceeded = false;
                List<string> errors = new List<string>();
                BaseModal vendorInputModal = new();

                #region If no vendor input

                if (vendorInput.AlcoholicDrinkToken == 0 && vendorInput.NonAlcoholicDrinkToken == 0 && vendorInput.FoodToken == 0)
                {
                    vendorInputModal = new BaseModal
                    {
                        GuestIdentificationNumber = vendorInput.GuestIdentificationNumber,
                        GuestName = $"",
                        Success = false,
                        Errors = new List<string>(),
                        TokensRemaining = new List<string>(),
                        Message = $"Please provide a token value for either, food, alcoholic drinks or non-alcoholic drinks.",
                        Title = "Error"
                    };

                    return vendorInputModal;
                }

                #endregion

                Attendee attendee = database.Attendee.SingleOrDefault(x => x.GuestIdentificationNumber == vendorInput.GuestIdentificationNumber);

                if (attendee == null)
                {

                    vendorInputModal = new BaseModal
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

                #region If allowance has been exceeded

                if (vendorInput.AlcoholicDrinkToken > 0 && attendee.AlcoholicDrinkTokenAllowance <= 0)
                {
                    allowanceExceeded = true;
                    errors.Add($"There are no alcoholic drink tokens remaining");
                }

                if (vendorInput.NonAlcoholicDrinkToken > 0 && attendee.NonAlcoholicDrinkTokenAllowance <= 0)
                {
                    allowanceExceeded = true;
                    errors.Add("There are no non-alcoholic drink tokens remaining");
                }

                if (vendorInput.FoodToken > 0 && attendee.FoodTokenAllowance <= 0)
                {
                    allowanceExceeded = true;
                    errors.Add("There are no food drink tokens remaining");
                }

                if (allowanceExceeded)
                {
                    vendorInputModal = new BaseModal
                    {
                        GuestIdentificationNumber = vendorInput.GuestIdentificationNumber,
                        GuestName = $"{attendee.FirstName} {attendee.LastName}",
                        Success = false,
                        Errors = errors,
                        TokensRemaining = new List<string>
                {
                    $"Alcohol tokens: {attendee.AlcoholicDrinkTokenAllowance}",
                    $"Non-Alcohol tokens: {attendee.NonAlcoholicDrinkTokenAllowance}",
                    $"Food tokens: {attendee.FoodTokenAllowance}"
                },
                        Message = $"There has been a problem for {attendee.FirstName} {attendee.LastName}",
                        Title = "Error"
                    };

                    return vendorInputModal;
                }

                #endregion

                attendee.AlcoholicDrinkTokenAllowance -= vendorInput.AlcoholicDrinkToken;
                attendee.NonAlcoholicDrinkTokenAllowance -= vendorInput.NonAlcoholicDrinkToken;
                attendee.FoodTokenAllowance -= vendorInput.FoodToken;

                vendorInputModal = new BaseModal
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
            catch (Exception ex)
            {
                BaseModal vendorInputModal = new BaseModal
                {
                    GuestIdentificationNumber = vendorInput.GuestIdentificationNumber,
                    GuestName = String.Empty,
                    Success = true,
                    Errors = new List<string>
                    {
                        $"Message: {ex.Message}",
                        $"Inner exception: {(ex.InnerException != null ? ex.InnerException.ToString() : "")}",
                        $"Stacktrace: {(ex.StackTrace != null ? ex.StackTrace.ToString() : "")}"
                    },
                    TokensRemaining = new List<string>(),
                    Message = $"The system administrator has been informed. Please try again.",
                    Title = "Error"
                };

                return vendorInputModal;
            }
        }
    }
} 