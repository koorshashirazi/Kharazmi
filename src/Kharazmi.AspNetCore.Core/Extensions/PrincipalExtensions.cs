using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Kharazmi.AspNetCore.Core.Authorization;
using Kharazmi.AspNetCore.Core.Runtime;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    public static partial class Core
    {
        /// <summary>
        /// Returns the value for the first claim of the specified type otherwise null the claim is not present.
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance this method extends.</param>
        /// <param name="claimType">The claim type whose first value should be returned.</param>
        /// <returns>The value of the first instance of the specified claim type, or null if the claim is not present.</returns>        
        public static string FindFirstValue(this ClaimsPrincipal principal, string claimType)
        {
            if (claimType.IsEmpty()) return "";
            principal.CheckArgumentIsNull(nameof(principal));
            var claim = principal.FindFirst(claimType);
            return claim?.Value;
        }

        /// <summary>
        /// Returns claims for the specified types. 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="claimTypes"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<Claim> GetClaims(this ClaimsPrincipal principal, IReadOnlyCollection<string> claimTypes)
        {
            principal.CheckArgumentIsNull(nameof(principal));
            claimTypes.CheckArgumentIsNull(nameof(claimTypes));

            var claims = new HashSet<Claim>();

            foreach (var claimType in claimTypes)
            {
                var claimList = principal.Claims
                    .Where(c => c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase));

                claims.AddRange(claimList);
            }

            return claims.AsReadOnly();
        }

        /// <summary>
        /// Checks the user claims, whether it has {role} or {http://schemas.microsoft.com/ws/2008/06/identity/claims/role}
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IReadOnlyList<string> FindRoles(this ClaimsPrincipal principal)
        {
            principal.CheckArgumentIsNull(nameof(principal));
            var roles = principal.Claims
                .Where(c =>
                {
                    var value = c.Type.Equals(UserClaimTypes.Role, StringComparison.OrdinalIgnoreCase);
                    if (!value)
                        value = c.Type.Equals(UserClaimTypes.JwtTypes.Role, StringComparison.OrdinalIgnoreCase);
                    return value;
                })
                .Select(c => c.Value).ToList();

            return roles.AsReadOnly();
        }

        /// <summary>
        /// Checks the user claims whether it has a {Permission} claim 
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IReadOnlyList<string> FindPermissions(this ClaimsPrincipal principal)
        {
            principal.CheckArgumentIsNull(nameof(principal));
            var permissions = principal.Claims
                .Where(c => c.Type.Equals(UserClaimTypes.Permission, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value)
                .ToList();

            var packedPermissions = principal.Claims.Where(c =>
                    c.Type.Equals(UserClaimTypes.PackedPermission, StringComparison.OrdinalIgnoreCase))
                .SelectMany(c => c.Value.UnpackFromString(PermissionConstant.PackingSymbol));

            permissions.AddRange(packedPermissions);

            return permissions.AsReadOnly();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static bool HasPermission(this ClaimsPrincipal principal, string permission)
        {
            return principal.FindPermissions().Any(p => p.Equals(permission, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetUserId<T>(this ClaimsPrincipal principal) where T : IConvertible
        {
            var firstValue = principal?.FindUserId();
            return firstValue != null
                ? (T) Convert.ChangeType(firstValue, typeof(T), CultureInfo.InvariantCulture)
                : default;
        }

        /// <summary>
        ///  Returns the value for the first claim of {"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"}
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string FindUserId(this ClaimsPrincipal principal)
        {
            var value = principal.FindFirstValue(UserClaimTypes.UserId);
            if (value.IsEmpty()) value = principal.FindFirstValue(UserClaimTypes.JwtTypes.Subject);
            return value;
        }

        /// <summary>
        /// Returns the value for the first claim of {BranchName}
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string FindBranchName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.BranchName);
        }

        /// <summary>
        ///  Returns the value for the first claim of {BranchId}
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string FindBranchId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.BranchId);
        }

        /// <summary>
        ///  Returns the value for the first claim of {TenantId}
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string FindTenantId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.TenantId);
        }

        /// <summary>
        /// Returns the value for the first claim of {client_id}
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string FindClientId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.JwtTypes.ClientId);
        }

        /// <summary>
        /// Returns the value for the first claim of {TenantName}
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string FindTenantName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.TenantName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static bool IsHeadTenant(this ClaimsPrincipal principal)
        {
            return principal.Claims.Any(c =>
                c.Type == UserClaimTypes.IsHeadTenant && c.Value == "true");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static bool IsHeadOffice(this ClaimsPrincipal principal)
        {
            return principal.Claims.Any(c =>
                c.Type == UserClaimTypes.IsHeadOffice && c.Value == "true");
        }

        /// <summary>
        /// Returns the value for the first claim of {ImpersonatorTenantId}
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string FindImpersonatorTenantId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.ImpersonatorTenantId);
        }

        /// <summary>
        /// Returns the value for the first claim of {ImpersonatorUserId}
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string FindImpersonatorUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(UserClaimTypes.ImpersonatorUserId);
        }

        /// <summary>
        /// Returns the value for the first claim of {FirstName and LastName}
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string FindUserDisplayName(this ClaimsPrincipal principal)
        {
            var displayName = $"{principal.FindFirstName()} {principal.FindLastName()}";
            return displayName.IsEmpty() ? principal.FindUserName() : displayName;
        }

        /// <summary>
        /// Returns the value for the first claim of {http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name} or {name}
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string FindUserName(this ClaimsPrincipal principal)
        {
            var userName = principal.FindFirstValue(UserClaimTypes.UserName);
            if (userName.IsEmpty()) userName = principal.FindFirstValue(UserClaimTypes.JwtTypes.Name);
            if (userName.IsEmpty()) userName = principal.Identity.Name;
            return userName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string FindFirstName(this ClaimsPrincipal principal)
        {
            var firstName = principal?.FindFirstValue(UserClaimTypes.GivenName);
            if (firstName.IsEmpty()) firstName = principal?.FindFirstValue(UserClaimTypes.JwtTypes.GivenName);
            return firstName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static string FindLastName(this ClaimsPrincipal principal)
        {
            var firstName = principal?.FindFirstValue(UserClaimTypes.Surname);
            if (firstName.IsEmpty()) firstName = principal?.FindFirstValue(UserClaimTypes.JwtTypes.FamilyName);
            return firstName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromIdentity"></param>
        /// <param name="toIdentity"></param>
        /// <param name="claimType"></param>
        public static void CopyClaimValueTo(this ClaimsIdentity fromIdentity, ClaimsIdentity toIdentity,
            string claimType)
        {
            var value = fromIdentity?.FindFirstValue(claimType);
            if (value.IsNotEmpty()) toIdentity.AddClaim(new Claim(claimType, value));
        }
    }
}