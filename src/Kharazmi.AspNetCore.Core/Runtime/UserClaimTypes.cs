using System.Security.Claims;

namespace Kharazmi.AspNetCore.Core.Runtime
{
    public static class UserClaimTypes
    {
        public const string UserName = ClaimTypes.Name;
        public const string GivenName = ClaimTypes.GivenName;
        public const string Surname = ClaimTypes.Surname;
        public const string UserId = ClaimTypes.NameIdentifier;
        public const string SerialNumber = ClaimTypes.SerialNumber;
        public const string Role = ClaimTypes.Role;
        public const string DisplayName = nameof(DisplayName);
        public const string BranchId = nameof(BranchId);
        public const string BranchName = nameof(BranchName);
        public const string IsHeadOffice = nameof(IsHeadOffice);
        public const string TenantId = nameof(TenantId);
        public const string TenantName = nameof(TenantName);
        public const string IsHeadTenant = nameof(IsHeadTenant);
        public const string Permission = nameof(Permission);
        public const string PackedPermission = nameof(PackedPermission);
        public const string ImpersonatorUserId = nameof(ImpersonatorUserId);
        public const string ImpersonatorTenantId = nameof(ImpersonatorTenantId);

        public static class JwtTypes
        {
            public const string Subject = "sub";
            public const string Name = "name";
            public const string GivenName = "given_name";
            public const string FamilyName = "family_name";
            public const string Role = "role";
            public const string Picture = "picture";
            public const string Email = "email";
            public const string Gender = "gender";
            public const string PhoneNumber = "phone_number";
            public const string ClientId = "client_id";
        }
    }
}