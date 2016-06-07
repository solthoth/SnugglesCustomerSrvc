namespace UserAuthService.Common.Lookups
{
    public enum UserAccountStatus
    {
        Inactive,
        Active,
        Unknown
    }

    public static class UserAccountStatusHelper
    {
        public static string ToString(this UserAccountStatus self)
        {
            switch (self)
            {
                case UserAccountStatus.Active: return "Active"; 
                case UserAccountStatus.Inactive: return "Inactive";
                case UserAccountStatus.Unknown: return "Unknown";
                default:
                    return "";
            }
        }

    }
}