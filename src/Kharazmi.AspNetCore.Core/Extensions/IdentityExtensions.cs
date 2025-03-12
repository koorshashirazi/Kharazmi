using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Kharazmi.AspNetCore.Core.Runtime;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    public static partial class Core
    {
        public static string FindFirstValue(this IIdentity identity, string claimType)
        {
            var identity1 = identity as ClaimsIdentity;
            return identity1?.FindFirst(claimType)?.Value;
        }
        
        public static IEnumerable<Claim> Get(this IIdentity identity, Func<Claim, bool> predication)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var claimValue = predication == null ? claimsIdentity?.Claims : claimsIdentity?.Claims.Where(predication);
            return claimValue;
        }
        
        public static T FindUserId<T>(this IIdentity identity) where T : IEquatable<T>
        {
            return identity.FindUserId().FromString<T>();
        }

        public static T FindTenantId<T>(this IIdentity identity) where T : IEquatable<T>
        {
            return identity.FindTenantId().FromString<T>();
        }

        public static T FindBranchId<T>(this IIdentity identity) where T : IEquatable<T>
        {
            return identity.FindBranchId().FromString<T>();
        }

        public static string FindUserId(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var value = claimsIdentity.FindFirstValue(ClaimsType.IdentityTypes.NameIdentifier);
            if (value.IsEmpty()) value = claimsIdentity.FindFirstValue(ClaimsType.JwtTypes.Subject);
            return value;
        }

        public static string FindBranchName(this IIdentity identity)
        {
            return identity.FindUserClaimValue(ClaimsType.IdentityTypes.BranchName);
        }

        public static string FindBranchId(this IIdentity identity)
        {
            return identity?.FindUserClaimValue(ClaimsType.IdentityTypes.BranchId);
        }

        public static string FindTenantId(this IIdentity identity)
        {
            return identity.FindUserClaimValue(ClaimsType.IdentityTypes.TenantId);
        }
        
        public static string FindTenantName(this IIdentity identity)
        {
            return identity.FindUserClaimValue(ClaimsType.IdentityTypes.TenantName);
        }

        public static string FindImpersonatorTenantId(this IIdentity identity)
        {
            return identity.FindUserClaimValue(ClaimsType.IdentityTypes.ImpersonatorTenantId);
        }

        public static string FindImpersonatorUserId(this IIdentity identity)
        {
            return identity.FindUserClaimValue(ClaimsType.IdentityTypes.ImpersonatorUserId);
        }

        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            return identity.FindFirst(claimType)?.Value;
        }

        public static string FindUserClaimValue(this IIdentity identity, string claimType)
        {
            return (identity as ClaimsIdentity)?.FindFirstValue(claimType);
        }

        public static string FindUserDisplayName(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var displayName = $"{claimsIdentity.FindFirstName()} {claimsIdentity.FindLastName()}";
            return displayName.IsEmpty() ? claimsIdentity.FindUserName() : displayName;
        }

        public static string FindUserName(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var userName= claimsIdentity.FindFirstValue(ClaimsType.IdentityTypes.Name);
            if (userName.IsEmpty()) userName = claimsIdentity.FindFirstValue(ClaimsType.JwtTypes.Name);
            if (userName.IsEmpty()) userName = claimsIdentity?.Name;
            return userName;
        }
        
        public static string FindUserEmail(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var email = claimsIdentity.FindFirstValue(ClaimsType.JwtTypes.Email);
            if (email.IsEmpty()) email = claimsIdentity.FindFirstValue(ClaimsType.IdentityTypes.Email);
            return email;
        }
        
        public static string FindFirstName(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var firstName =  claimsIdentity?.FindFirstValue(ClaimsType.IdentityTypes.GivenName);
            if(firstName.IsEmpty()) firstName = claimsIdentity?.FindFirstValue(ClaimsType.JwtTypes.GivenName);
            return firstName;
        }
        
        public static string FindLastName(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var firstName =  claimsIdentity?.FindFirstValue(ClaimsType.IdentityTypes.Surname);
            if(firstName.IsEmpty()) firstName = claimsIdentity?.FindFirstValue(ClaimsType.JwtTypes.FamilyName);
            return firstName;
        }
        
        public static IReadOnlyCollection<string> GetUserRoles(this IIdentity identity)
        {
            var claimValue = identity.Get(x => x.Type == ClaimsType.JwtTypes.Role)
                .Select(x => x.Value);
            return claimValue?.AsReadOnly();
        }
        
        public static IReadOnlyCollection<string> GetUserPermissions(this IIdentity identity)
        {
            var claimValue = identity.Get(x => x.Type == ClaimsType.JwtTypes.Permissions)
                .Select(x => x.Value);
            return claimValue?.AsReadOnly();
        }

        public static IReadOnlyCollection<string> GetUserClientNames(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var claimValue = claimsIdentity?.FindFirstValue(ClaimsType.JwtTypes.UserClientNames);
            var userClientNames = claimValue?.Split(',');
            return userClientNames.AsReadOnly();
        }

        public static IReadOnlyCollection<string> GetUserClientEmails(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var claimValue = claimsIdentity?.FindFirstValue("client_user_email");
            var userClientNames = claimValue?.Split(',');
            return userClientNames.AsReadOnly();
        }
    }
}