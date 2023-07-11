﻿using EventsManagementInterface.Data.Enums;
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

        public void CreateLog(LogType type, string summary, int tokensUsed, int guestIdentificationNumber)
        {
            Log log = new Log()
            {
                Type = type,
                Summary = summary,
                TokensUsed = tokensUsed,
                GuestIdentificationNumber = guestIdentificationNumber,
                CreatedDateTime = DateTime.Now,
                Archived = false,
            };

            database.Add(log);
            database.SaveChanges();
        }
    }
}