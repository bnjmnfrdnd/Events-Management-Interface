using Azure.Core;
using EventsManagementInterface.Data.Enums;
using EventsManagementInterface.Data.Models;
using EventsManagementInterface.Data.Models.Administration;
using EventsManagementInterface.Data.Models.Attendee;
using EventsManagementInterface.Data.Models.Vendor;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata;

namespace EventsManagementInterface.Data.Services
{
    public class AdministrationService
    {
        private ApplicationDbContext database;
        private Utility utility;
        private LogService logService;        

        public AdministrationService(ApplicationDbContext database, Utility utility, LogService logService)
        {
            this.database = database;
            this.utility = utility;
            this.logService = logService;            
        }

        public async Task<List<Attendee>> GetAttendees()
        {
            return await database.Attendee.ToListAsync();
        }

        public async Task<BaseModal> QuickCheckTokenAllowance(int GIN)
        {
            Attendee attendee = database.Attendee.SingleOrDefault(x => x.GuestIdentificationNumber == GIN);
            BaseModal baseModal;

            if (attendee != null)
            {
                baseModal = new BaseModal
                {
                    Success = true,
                    Title = "Success",
                    Message = $"Please see token allowance for {attendee.FirstName} ({GIN}) below.",
                    TokensRemaining = new List<string>
                    {
                        $"Alcoholic drink tokens: {attendee.AlcoholicDrinkTokenAllowance}",
                        $"Non-Alcoholic drink tokens: {attendee.NonAlcoholicDrinkTokenAllowance}",
                        $"Food tokens: {attendee.FoodTokenAllowance}"
                    }
                };
            }
            else
            {
                baseModal = new BaseModal
                {
                    Success = false,
                    Title = "Error",
                    Message = $"Please see token allowance for ({GIN}) below.",
                    Errors = new List<string>()
                    {
                        "The GIN number does not exist."
                    },

                };
            }
            return baseModal;
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
                    Message = $"Please see token allowance below. An email has also been sent to {attendee.EmailAddress}.",
                    TokensRemaining = new List<string>
                    {
                        $"Alcoholic drink tokens: {attendee.AlcoholicDrinkTokenAllowance}",
                        $"Non-Alcoholic drink tokens: {attendee.NonAlcoholicDrinkTokenAllowance}",
                        $"Food tokens: {attendee.FoodTokenAllowance}"
                    }
                };

                Utility.SendEmail(new Models.Email.Email
                {
                    Recipient = attendee.EmailAddress,
                    Subject = "Coloplast Family Fun Day Allowance",
                    HTMLMessage =
                    $"Hi {attendee.FirstName}, " +
                    $"<br/><br/> " +
                    $"Your remaining token allowance is: <ul>" +
                    $"<li>Alcoholic Drink Tokens: {attendee.AlcoholicDrinkTokenAllowance}</li>" +
                    $"<li>Non-Alcoholic Drink Tokens: {attendee.NonAlcoholicDrinkTokenAllowance}</li>" +
                    $"<li>Food Tokens: {attendee.FoodTokenAllowance}</li>" +
                    $"</ul>" +
                    $"<br/>" +
                    $"Kind regards, <br>" +
                    $"Coloplast Family Fun Day Team"
                });

                return baseModal;
            }
            else
            {
                baseModal = new BaseModal
                {
                    Success = false,
                    Title = "Error",
                    Message = "There was a problem with this request, please see the errors below",
                    Errors = new List<string>()
                    {
                        $"Guest Identificiation Number ({guestManagement.GuestIdentificationNumber}) does not exist"
                    }
                };

                return baseModal;
            }
        }

        public async Task<BaseModal> SendInvitationEmails(bool includePreviouslyEmailedGuests)
        {
            try
            {
                List<Attendee> recipients = new List<Attendee>();
                BaseModal baseModal;
                bool sendSuccess;
                List<Attendee> failedInvitations = new List<Attendee>();

                if (includePreviouslyEmailedGuests)
                {
                    recipients = database.Attendee.ToList();
                }
                else
                {
                    recipients = database.Attendee.Where(x => x.GuestIdentificationNumberEmailSent == false).ToList();
                }

                foreach (Attendee recipient in recipients)
                {
                    sendSuccess = await Utility.SendEmailAsync(new Models.Email.Email
                    {
                        Recipient = recipient.EmailAddress,
                        Subject = "Your Coloplast Family Fun Day Invitation",
                        HTMLMessage =
                        $"Hi {recipient.FirstName}, " +
                        $"<br/><br/> " +
                        $"Thank you for registering your attendance for the Coloplast Family Fun Day {DateTime.Now.Year}." +
                        $"<br/><br/>" +
                        $"As per the recent email, please find below information you will need on the day, to obtain your food & drinks." +
                        $"<br/><br/>" +
                        $"Your Guest Identification Number (GIN) is: <b>{recipient.GuestIdentificationNumber}</b>" +
                        $"<br/><br/>" +
                        $"Based on each guest having an allowance of five drinks per person, your allowance is:" +
                        $"<ul>" +
                        $"<li>Alcoholic Drink Tokens: <b>{recipient.AlcoholicDrinkTokenAllowance}</b></li>" +
                        $"<li>Non-Alcoholic Drink Tokens: <b>{recipient.NonAlcoholicDrinkTokenAllowance}</b></li>" +
                        $"<li>Main Meal Food Tokens: <b>{recipient.FoodTokenAllowance}</b></li>" +
                        $"</ul>" +
                        $"Please note, you do not need a token for the dessert options." +
                        $"<br/><br/>" +
                        $"To use your token allowance, you will need to give your GIN number to a vendor when ordering (they will ask for it!), this will then reduce your allowance accordingly." +
                        $"<br/><br/>" +
                        $"If there are any queries, please email <a href='mailto:GB_people_engagement@coloplast.com?subject=Coloplast Family Fun Day 2023 Query'>GB_people_engagement@coloplast.com</a>. " +
                        $"<br/><br/>" +
                        $"Kind regards," +
                        $"<br>" +
                        $"Coloplast Family Fun Day Team"
                    });

                    if (!sendSuccess)
                    {
                        failedInvitations.Add(recipient);
                    }
                    else
                    {
                        recipient.GuestIdentificationNumberEmailSent = true;
                        database.Update(recipient);
                    }

                }

                database.SaveChanges();

                baseModal = new BaseModal
                {
                    Success = true,
                    Title = "Success",
                    Message = "Guests have been successfully emailed their invitations.",
                };

                return baseModal;
            }
            catch (Exception exception)
            {
                Utility.SendExceptionThrownEmail("SendInvitationEmails", exception);
                logService.CreateExceptionThrownLog("SendInvitationEmails", exception);

                BaseModal baseModal = new BaseModal
                {
                    Success = false,
                    Title = "Error",
                    Message = $"There has been an error sending out the email invitations. An email has been sent to the system admin.",
                    Errors = new List<string> { exception.Message }
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
                    Message = $"Token allowance has been updated. An email has been sent to {attendee.EmailAddress}.",
                };

                database.Update(attendee);
                database.SaveChanges();

                Utility.SendEmail(new Models.Email.Email
                {
                    Recipient = attendee.EmailAddress,
                    Subject = "Coloplast Family Fund Day Allowance",
                    HTMLMessage =
                    $"Hi {attendee.FirstName}, " +
                    $"<br/><br/> " +
                    $"Your new token allowance is: <ul>" +
                    $"<li>Alcoholic Drink Tokens: {attendee.AlcoholicDrinkTokenAllowance}</li>" +
                    $"<li>Non-Alcoholic Drink Tokens: {attendee.NonAlcoholicDrinkTokenAllowance}</li>" +
                    $"<li>Food Tokens: {attendee.FoodTokenAllowance}</li>" +
                    $"</ul>" +
                    $"<br/>" +
                    $"Kind regards, <br>" +
                    $"Coloplast Family Fun Day Team"
                });

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

        public async Task<double> GetTotalAvailableAlcoholDrinkTokens()
        {
            List<Attendee> attendees = database.Attendee.Where(x => x.Archived == false).ToList();
            double totalAvailableAlcoholDrinkTokens = 0;

            foreach (Attendee attendee in attendees)
            {
                totalAvailableAlcoholDrinkTokens += attendee.AlcoholicDrinkTokenAllowance;
            }

            return totalAvailableAlcoholDrinkTokens;
        }

        public async Task<double> GetTotalAvailableNonAlcoholDrinkTokens()
        {
            List<Attendee> attendees = database.Attendee.Where(x => x.Archived == false).ToList();
            double totalAvailableNonAlcoholDrinkTokens = 0;

            foreach (Attendee attendee in attendees)
            {
                totalAvailableNonAlcoholDrinkTokens += attendee.NonAlcoholicDrinkTokenAllowance;
            }

            return totalAvailableNonAlcoholDrinkTokens;
        }

        public async Task<double> GetTotalAvailableFoodTokens()
        {
            List<Attendee> attendees = database.Attendee.Where(x => x.Archived == false).ToList();
            double totalAvailableFoodTokens = 0;

            foreach (Attendee attendee in attendees)
            {
                totalAvailableFoodTokens += attendee.FoodTokenAllowance;
            }

            return totalAvailableFoodTokens;
        }

        public async Task<double> GetTotalGuestsWithUnusedTokens()
        {
            List<Attendee> attendees = database.Attendee.Where(x => x.Archived == false).ToList();
            List<Log> logs = database.Log.Where(x => x.Archived == false).ToList();
            double totalAttendeesWithUsedTokens = 0;

            foreach (Attendee attendee in attendees)
            {
                foreach (Log log in logs)
                {
                    if (log.GuestIdentificationNumber == attendee.GuestIdentificationNumber)
                    {
                        totalAttendeesWithUsedTokens++;
                    }
                }
            }

            return attendees.Count - totalAttendeesWithUsedTokens;
        }

        public async Task<BaseModal> UploadAttendeeData(InputFileChangeEventArgs document)
        {
            try
            {
                var attendee = new Attendee();
                List<Attendee> attendees = new List<Attendee>();
                List<Attendee> existingAttendees = database.Attendee.Where(x => x.Archived != true).ToList();
                MemoryStream ms = new MemoryStream();
                await document.File.OpenReadStream().CopyToAsync(ms);
                var bytes = ms.ToArray();
                string commaSeperatedValues = System.Text.Encoding.UTF8.GetString(bytes.ToArray()); // need to convert to string array
                bool attendeeExists = true;

                List<string> lines = commaSeperatedValues.Split("\n").ToList();

                foreach (string line in lines)
                {
                    string[] words = line.Split(',');

                    if (words != null)
                    {
                        attendee = new Attendee
                        {
                            EmailAddress = words.Length > 2 ? words[0].Trim().ToLower() : "",
                            FirstName = words[1].Trim(),
                            LastName = words.Length > 1 ? words[2].Trim() : "",
                            AlcoholicDrinkTokenAllowance = words.Length > 3 ? Int32.Parse(words[3]) : 2,
                            NonAlcoholicDrinkTokenAllowance = words.Length > 4 ? Int32.Parse(words[4]) : 2,
                            FoodTokenAllowance = words.Length > 5 ? Int32.Parse(words[5]) : 2
                        };

                        if (attendee.EmailAddress != null && attendee.EmailAddress != "")
                        {

                            foreach (Attendee att in existingAttendees)
                            {
                                if (att.EmailAddress.ToUpper().Trim() != attendee.EmailAddress.ToUpper().Trim())
                                {
                                    attendeeExists = false;
                                }
                                else
                                {
                                    attendeeExists = true;
                                    break;
                                }
                            }
                        }

                        if (attendeeExists == false || existingAttendees.Count == 0)
                        {
                            attendees.Add(attendee);
                        }
                    }
                }

                if (attendees.Count > 0)
                {
                    logService.CreateLog(
                        LogType.UploadAttendees,
                        $"{attendees.Count}",
                        0,
                        0,
                        "ATTENDEES UPLOADED"
                    );

                    foreach (Attendee att in attendees)
                    {
                        att.GuestIdentificationNumber = GenerateGuestIdentificationNumber();
                        database.Add(att);
                        database.SaveChanges();
                    }

                    BaseModal baseModal = new BaseModal
                    {
                        Success = true,
                        Title = "Success",
                        Message = $"{attendees.Count} guests have been successfully added.",
                    };

                    return baseModal;
                }
                else
                {
                    BaseModal baseModal = new BaseModal
                    {
                        Success = false,
                        Title = "Error",
                        Message = $"No new guests have been added. Please check the data and try again.",
                        Errors = new List<string>()
                        {
                             "The guest data already exist."
                        }
                    };

                    return baseModal;
                }


            }
            catch (Exception exception)
            {
                Utility.SendExceptionThrownEmail("UploadAttendeeData", exception);
                logService.CreateExceptionThrownLog("UploadAttendeeData", exception);

                BaseModal baseModal = new BaseModal
                {
                    Success = false,
                    Title = "Error",
                    Message = $"There has been an error uploading the data. An email has been sent to the system admin.",
                    Errors = new List<string> { exception.Message }
                };

                return baseModal;

            }
        }

        public async Task<BaseModal> SendInvitationEmail(Attendee attendee)
        {
            try
            {
                List<Attendee> recipients = new List<Attendee>();
                BaseModal baseModal;
                bool sendSuccess = false;

                sendSuccess = await Utility.SendEmailAsync(new Models.Email.Email
                {
                    Recipient = attendee.EmailAddress,
                    Subject = "Your Coloplast Family Fun Day Invitation",
                    HTMLMessage =
                        $"Hi {attendee.FirstName}, " +
                        $"<br/><br/> " +
                        $"Thank you for registering your attendance for the Coloplast Family Fun Day {DateTime.Now.Year}." +
                        $"<br/><br/>" +
                        $"As per the recent email, please find below information you will need on the day, to obtain your food & drinks." +
                        $"<br/><br/>" +
                        $"Your Guest Identification Number (GIN) is: <b>{attendee.GuestIdentificationNumber}</b>" +
                        $"<br/><br/>" +
                        $"Based on each guest having an allowance of five drinks per person, your token allowance is:" +
                        $"<ul>" +
                        $"<li>Alcoholic Drink Tokens: <b>{attendee.AlcoholicDrinkTokenAllowance}</b></li>" +
                        $"<li>Non-Alcoholic Drink Tokens: <b>{attendee.NonAlcoholicDrinkTokenAllowance}</b></li>" +
                        $"<li>Food Tokens: <b>{attendee.FoodTokenAllowance}</b></li>" +
                        $"</ul>" +
                        $"Please note, you do not need a token for the dessert options." +
                        $"<br/><br/>" +
                        $"To use your token allowance, you will need to give your GIN number to a vendor when ordering (they will ask for it!), this will then reduce your allowance accordingly." +
                        $"<br/><br/>" +
                        $"If there are any queries, please email <a href='mailto:GB_people_engagement@coloplast.com?subject=Coloplast Family Fun Day 2023 Query'>GB_people_engagement@coloplast.com</a>. " +
                        $"<br/><br/>" +
                        $"Kind regards," +
                        $"<br>" +
                        $"Coloplast Family Fun Day Team"
                });

                if (sendSuccess)
                {
                    attendee.GuestIdentificationNumberEmailSent = true;
                    database.Update(attendee);
                }

                database.SaveChanges();

                baseModal = new BaseModal
                {
                    Success = true,
                    Title = "Success",
                    Message = "Email invitation successfully sent.",
                };

                return baseModal;
            }
            catch (Exception exception)
            {
                Utility.SendExceptionThrownEmail("SendInvitationEmail", exception);
                logService.CreateExceptionThrownLog("SendInvitationEmail", exception);

                BaseModal baseModal = new BaseModal
                {
                    Success = false,
                    Title = "Error",
                    Message = $"There has been an error sending out the email invitation. An email has been sent to the system admin.",
                    Errors = new List<string> { exception.Message }
                };

                return baseModal;

            }
        }

        public int GenerateGuestIdentificationNumber()
        {
            Random random = new Random();
            int newGIN = random.Next(1000, 9999);
            List<Attendee> attendees = database.Attendee.ToList();
            List<int> existingGINS = new List<int>();
            bool GINExists = true;

            foreach (Attendee a in attendees)
            {
                existingGINS.Add(a.GuestIdentificationNumber);
            }

            if (existingGINS.Count != 0)
            {
                foreach (int existingGIN in existingGINS)
                {
                    if (existingGIN != newGIN)
                    {
                        GINExists = false;
                    }
                    else
                    {
                        GINExists = true;
                        break;
                    }

                    if (GINExists) GenerateGuestIdentificationNumber();
                }

                return newGIN;
            }
            else
            {
                return newGIN;
            }


        }

        public async Task<BaseModal> SaveAttendee(Attendee attendee, bool sendInvitationEmail)
        {
            try
            {

                BaseModal baseModal;
                string message = "";
                Attendee att = database.Attendee.SingleOrDefault(x => x.GuestIdentificationNumber == attendee.GuestIdentificationNumber);

                if (att == null)
                {
                    attendee.GuestIdentificationNumber = GenerateGuestIdentificationNumber();
                    database.Add(attendee);
                }
                else
                {
                    database.Update(att);
                }

                database.SaveChanges();

                if (sendInvitationEmail)
                {
                    message = $"Guest has been successfully saved. An email invitation has been sent to {attendee.EmailAddress}";
                    await SendInvitationEmail(attendee);
                }
                else
                {
                    message = "Guest has been successfully saved.";
                }

                logService.CreateLog(
                        LogType.SaveAttendee,
                        "ATTENDEE SAVED",
                        0,
                        attendee.GuestIdentificationNumber,
                        "Manual Save"
                    );

                baseModal = new BaseModal
                {
                    Success = true,
                    Title = "Success",
                    Message = message,
                };

                return baseModal;
            }
            catch (Exception exception)
            {
                Utility.SendExceptionThrownEmail("SaveAttendee", exception);
                logService.CreateExceptionThrownLog("SaveAttendee", exception);

                BaseModal baseModal = new BaseModal
                {
                    Success = false,
                    Title = "Error",
                    Message = $"There has been an error saving the attendee. An email has been sent to the system admin.",
                    Errors = new List<string> { exception.Message }
                };

                return baseModal;
            }
        }

    }
}