using EventsManagementInterface.Data.Enums;
using EventsManagementInterface.Data.Models.Vendor;
using EventsManagementInterface.Data.Models.Administration;
using System.Globalization;
using CsvHelper;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace EventsManagementInterface.Data.Services
{
    public class ExportService
    {
        private ApplicationDbContext database;

        public ExportService(ApplicationDbContext database)
        {
            this.database = database;

        }
    }
}
