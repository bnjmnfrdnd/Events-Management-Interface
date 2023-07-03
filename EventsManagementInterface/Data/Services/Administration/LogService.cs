using EventsManagementInterface.Data.Models.Administration;

namespace EventsManagementInterface.Data.Services
{
    public class LogService
    {

        private ApplicationDbContext database;

        public LogService(ApplicationDbContext database)
        {
            this.database = database;
        }

        public void CreateLog(string type, string summary)
        {
            Log log = new Log()
            {
                Type = type,
                Summary = summary,
                CreatedDateTime = DateTime.Now,
                Archived = false,
            };

            database.Add(log);
            database.SaveChanges();
        }
    }
}