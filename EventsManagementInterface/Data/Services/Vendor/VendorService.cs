using EventsManagementInterface.Data.Models.Attendee;
using EventsManagementInterface.Data.Models.Vendor;
using EventsManagementInterface.Data.Models;
using System.Globalization;
using EventsManagementInterface.Data.Enums;
using System;
using EventsManagementInterface.Data.Models.Administration;

namespace EventsManagementInterface.Data.Services
{
    public class VendorService
    {
        private LogService logService;
        private AdministrationService administrationService;
        private ApplicationDbContext database;
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public VendorService(ApplicationDbContext database, LogService logService, AdministrationService administrationService)
        {
            this.database = database;
            this.logService = logService;
            this.administrationService = administrationService;
        }

        public async Task<BaseModal> SubmitVendorInput(VendorInput vendorInput, List<Order> orders)
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
                        Errors = new List<string> 
                        {
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
                    errors.Add($"There are no alcoholic drink tokens remaining.");
                }

                if (vendorInput.AlcoholicDrinkToken > 0 && attendee.AlcoholicDrinkTokenAllowance < vendorInput.AlcoholicDrinkToken)
                {
                    allowanceExceeded = true;
                    errors.Add($"There are not enough alcoholic drink tokens remaining.");
                }

                if (vendorInput.NonAlcoholicDrinkToken > 0 && attendee.NonAlcoholicDrinkTokenAllowance <= 0)
                {
                    allowanceExceeded = true;
                    errors.Add("There are no non-alcoholic drink tokens remaining.");
                }

                if (vendorInput.NonAlcoholicDrinkToken > 0 && attendee.NonAlcoholicDrinkTokenAllowance < vendorInput.NonAlcoholicDrinkToken)
                {
                    allowanceExceeded = true;
                    errors.Add("There are not enough non-alcoholic drink tokens remaining.");
                }

                if (vendorInput.FoodToken > 0 && attendee.FoodTokenAllowance <= 0)
                {
                    allowanceExceeded = true;
                    errors.Add("There are no food tokens remaining.");
                }

                if (vendorInput.FoodToken > 0 && attendee.FoodTokenAllowance < vendorInput.FoodToken)
                {
                    allowanceExceeded = true;
                    errors.Add("There are not enough food tokens remaining.");
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

                foreach (Order order in orders)
                {
                    switch (order.Type)
                    {
                        case OrderType.ALCOHOL:
                            switch (order.Name)
                            {
                                case (nameof(Drink.MADRI_LAGER_PINT)):
                                    order.Price = 5.00;
                                    break;
                                case (nameof(Drink.MADRI_LAGER_HALF_PINT)):
                                    order.Price = 2.50;
                                    break;
                                case (nameof(Drink.NECK_OIL_IPA_PINT)):
                                    order.Price = 5.00;
                                    break;
                                case (nameof(Drink.NECK_OIL_IPA_HALF_PINT)):
                                    order.Price = 2.50;
                                    break;
                                case (nameof(Drink.ASPALLS_CIDER)):
                                    order.Price = 5.00;
                                    break;
                                case (nameof(Drink.KOPPARBERG_MIXED_FRUITS)):
                                    order.Price = 5.00;
                                    break;
                                case (nameof(Drink.SMIRNOFF_RED_VODKA)):
                                    order.Price = 5.00;
                                    break;
                                case (nameof(Drink.GORDONS_GIN)):
                                    order.Price = 5.00;
                                    break;
                                case (nameof(Drink.BACARDI)):
                                    order.Price = 5.00;
                                    break;
                                case (nameof(Drink.JACK_DANIELS)):
                                    order.Price = 5.00;
                                    break;
                                case (nameof(Drink.MORGANS_SPICED_RUM)):
                                    order.Price = 5.00;
                                    break;
                                case (nameof(Drink.MALIBU)):
                                    order.Price = 5.00;
                                    break;
                                case (nameof(Drink.GORDONS_PINK_GIN)):
                                    order.Price = 5.50;
                                    break;
                                case (nameof(Drink.WHITLEY_NEILL_SELECTION)):
                                    order.Price = 5.50;
                                    break;
                                case (nameof(Drink.PIMMS_AND_LEMONADE)):
                                    order.Price = 4.50;
                                    break;
                                case (nameof(Drink.PROSECCO)):
                                    order.Price = 4.50;
                                    break;
                                case (nameof(Drink.RED_WINE)):
                                    order.Price = 4.50;
                                    break;
                                case (nameof(Drink.WHITE_WINE)):
                                    order.Price = 4.50;
                                    break;
                            }
                            break;

                        case OrderType.NON_ALCOHOL:
                            {
                                switch (order.Name)
                                {
                                    case (nameof(Drink.HEINEKEN_ZERO)):
                                        order.Price = 3.50;
                                        break;
                                    case (nameof(Drink.LEMONADE)):
                                        order.Price = 1.00;
                                        break;
                                    case (nameof(Drink.COKE)):
                                        order.Price = 1.00;
                                        break;
                                    case (nameof(Drink.FRUIT_SHOOT)):
                                        order.Price = 1.00;
                                        break;
                                    case (nameof(Drink.ORANGE)):
                                        order.Price = 1.00;
                                        break;
                                    case (nameof(Drink.WATER)):
                                        order.Price = 1.00;
                                        break;
                                }
                                break;
                            }
                    }


                    order.GuestIdentificationNumber = vendorInput.GuestIdentificationNumber;
                    database.Add(order);
                }

                database.SaveChanges();

                if (vendorInput.AlcoholicDrinkToken != null && vendorInput.AlcoholicDrinkToken != 0)
                {
                    logService.CreateLog(
                        LogType.AlcoholicDrinkTokenUsed,
                        "REDEEMED TOKENS",
                        vendorInput.AlcoholicDrinkToken,
                        vendorInput.GuestIdentificationNumber,
                        "ALCOHOL"
                    );
                }

                if (vendorInput.NonAlcoholicDrinkToken != null && vendorInput.NonAlcoholicDrinkToken != 0)
                {
                    logService.CreateLog(
                        LogType.NonAlcoholicDrinkTokenUsed,
                        "REDEEMED TOKENS",
                        vendorInput.NonAlcoholicDrinkToken,
                        vendorInput.GuestIdentificationNumber,
                        "NON-ALCOHOL"
                    );
                }

                if (vendorInput.FoodToken != null && vendorInput.FoodToken != 0)
                {
                    logService.CreateLog(
                        LogType.FoodTokenUsed,
                        "REDEEMED TOKENS",
                        vendorInput.FoodToken,
                        vendorInput.GuestIdentificationNumber,
                        "FOOD"
                    );
                }

                return vendorInputModal;
            }
            catch (Exception ex)
            {
                Utility.SendExceptionThrownEmail("SubmitVendorInput", ex);

                logService.CreateExceptionThrownLog("SubmitVendorInput", ex);

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

        public async Task<BaseModal> CheckTokenAllowance(VendorInput vendorInput)
        {
            return await administrationService.QuickCheckTokenAllowance(vendorInput.GuestIdentificationNumber);
        }
    }
}