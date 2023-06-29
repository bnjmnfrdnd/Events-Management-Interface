using EventsManagementInterface.Data.Models;
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
    }
}