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

        public async Task<AdministrationModal> UpdateGINAllowance()
        {
            return null;
        }
    }
}