using EventsManagementInterface.Data.Models;
using EventsManagementInterface.Data.Models.Administration;
using EventsManagementInterface.Data.Models.Attendee;
using Microsoft.EntityFrameworkCore;

namespace EventsManagementInterface.Data.Services
{
    public class AdministrationService
    {

        private ApplicationDbContext database;

        public AdministrationService(ApplicationDbContext database)
        {
            this.database = database;
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
                    Message = "Please see token allowance below",
                    TokensRemaining = new List<string>
                    {
                        $"Alcoholic drink tokens: {attendee.AlcoholicDrinkTokenAllowance}",
                        $"Non-Alcoholic drink tokens: {attendee.NonAlcoholicDrinkTokenAllowance}",
                        $"Food tokens: {attendee.FoodTokenAllowance}"
                    }
                };

                database.Update(attendee);
                database.SaveChanges();

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
                    Message = "Token allowance has been updated",
                };

                database.Update(attendee);
                database.SaveChanges();

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