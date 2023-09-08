using EventsManagementInterface.Data.Enums;
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

        public void CreateLog(LogType type, string summary, int tokensUsed, int guestIdentificationNumber, string message = null)
        {
            Log log = new Log()
            {
                Type = type,
                Summary = summary,
                TokensUsed = tokensUsed,
                GuestIdentificationNumber = guestIdentificationNumber,
                CreatedDateTime = DateTime.Now,
                Archived = false,
                Message = message ?? ""
            };

            database.Add(log);
            database.SaveChanges();
        }
 
        public void CreateExceptionThrownLog(string function, Exception exception)
        {
            Log log = new Log()
            {
                Type = LogType.ExceptionThrown,
                Summary = $"EXCEPTION",
                CreatedDateTime = DateTime.Now,
                Archived = false,
                Message = $"{function}: {exception.Message}",
            };

            database.Add(log);
            database.SaveChanges();
        }
    }
}