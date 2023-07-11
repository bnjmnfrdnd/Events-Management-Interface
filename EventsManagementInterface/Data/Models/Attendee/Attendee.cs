﻿namespace EventsManagementInterface.Data.Models.Attendee
{
    public class Attendee : Registration
    {
        public int FoodTokenAllowance { get; set; }
        public int AlcoholicDrinkTokenAllowance { get; set; }
        public int NonAlcoholicDrinkTokenAllowance { get; set; }

        public int FoodTokensUsed { get; set; }
        public int AlcoholicDrinkTokensUsed { get; set; }
        public int NonAlcoholicDrinkTokensUsed { get; set; }

        public int GuestIdentificationNumber { get; set; }
        public bool GuestIdentificationNumberEmailSent { get; set; }
    }
}
