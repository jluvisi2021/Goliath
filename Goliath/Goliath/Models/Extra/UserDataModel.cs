using Goliath.Models.Accounts;
using System.Collections.Generic;

namespace Goliath.Models.Extra
{
    /// <summary>
    /// All of the data that is sent to the user when they download their data.
    /// </summary>
    public class UserDataModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string Email { get; set; }
        public string EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberConfirmed { get; set; }
        public string BackgroundColor { get; set; }
        public string DarkTheme { get; set; }
        public string AccountCreationDate { get; set; }
        public string LastUserLogin { get; set; }
        public string LogoutThreshold { get; set; }
        public string UnverifiedNewEmail { get; set; }
        public string UnverifiedNewPhone { get; set; }
        public string LastPasswordUpdate { get; set; }
        public string TwoFactorUpdated { get; set; }
        public List<LoginTracebackEntryModel> AccountLoginHistory { get; set; }
        public string PendingNotifications { get; set; }
        public string UserData { get; set; }
        public string TwoFactorMethod { get; set; }
    }
}