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
        
    }

    //public async Task<VendorInputModal> SubmitVendorInput()
    //{
    //    return null;
    //}
} 