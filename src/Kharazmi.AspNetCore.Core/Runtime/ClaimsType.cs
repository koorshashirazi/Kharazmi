namespace Kharazmi.AspNetCore.Core.Runtime
{
    public static class ClaimsType
    {
        //
        // Summary:
        //     Commonly used claim types
        public static class JwtTypes
        {
            //
            // Summary:
            //     UserType
            public const string UserType = "user_type";

            //
            // Summary:
            //     Unique Identifier for the End-User at the Issuer.
            public const string Subject = "sub";

            //
            // Summary:
            //     The iat (issued at) claim identifies the time at which the JWT was issued, ,
            //     specified as the number of seconds from 1970-01-01T0:0:0Z
            public const string IssuedAt = "iat";

            //
            // Summary:
            //     Authentication Methods References. JSON array of strings that are identifiers
            //     for authentication methods used in the authentication.
            public const string AuthenticationMethod = "amr";

            public const string SecurityStamp = "AspNet.Identity.SecurityStamp";

            //
            // Summary:
            //     Session identifier. This represents a Session of an OP at an RP to a User Agent
            //     or device for a logged-in End-User. Its contents are unique to the OP and opaque
            //     to the RP.
            public const string SessionId = "sid";

            //
            // Summary:
            //     Authentication Context Class Reference. String specifying an Authentication Context
            //     Class Reference value that identifies the Authentication Context Class that the
            //     authentication performed satisfied. The value "0" indicates the End-User authentication
            //     did not meet the requirements of ISO/IEC 29115 level 1. Authentication using
            //     a long-lived browser cookie, for instance, is one example where the use of "level
            //     0" is appropriate. Authentications with level 0 SHOULD NOT be used to authorize
            //     access to any resource of any monetary value. (This corresponds to the OpenID
            //     2.0 PAPE nist_auth_level 0.) An absolute URI or an RFC 6711 registered name SHOULD
            //     be used as the acr value; registered names MUST NOT be used with a different
            //     meaning than that which is registered. Parties using this claim will need to
            //     agree upon the meanings of the values used, which may be context-specific. The
            //     acr value is a case sensitive string.
            public const string AuthenticationContextClassReference = "acr";

            //
            // Summary:
            //     Time when the End-User authentication occurred. Its value is a JSON number representing
            //     the number of seconds from 1970-01-01T0:0:0Z as measured in UTC until the date/time.
            //     When a max_age request is made or when auth_time is requested as an Essential
            //     Claim, then this Claim is REQUIRED; otherwise, its inclusion is OPTIONAL.
            public const string AuthenticationTime = "auth_time";

            //
            // Summary:
            //     The party to which the ID Token was issued. If present, it MUST contain the OAuth
            //     2.0 Client ID of this party. This Claim is only needed when the ID Token has
            //     a single audience value and that audience is different than the authorized party.
            //     It MAY be included even when the authorized party is the same as the sole audience.
            //     The azp value is a case sensitive string containing a StringOrURI value.
            public const string AuthorizedParty = "azp";

            //
            // Summary:
            //     Access Token hash value. Its value is the base64url encoding of the left-most
            //     half of the hash of the octets of the ASCII representation of the access_token
            //     value, where the hash algorithm used is the hash algorithm used in the alg Header
            //     Parameter of the ID Token's JOSE Header. For instance, if the alg is RS256, hash
            //     the access_token value with SHA-256, then take the left-most 128 bits and base64url
            //     encode them. The at_hash value is a case sensitive string.
            public const string AccessTokenHash = "at_hash";

            //
            // Summary:
            //     NotificationId hash value. Its value is the base64url encoding of the left-most half of
            //     the hash of the octets of the ASCII representation of the code value, where the
            //     hash algorithm used is the hash algorithm used in the alg Header Parameter of
            //     the ID Token's JOSE Header. For instance, if the alg is HS512, hash the code
            //     value with SHA-512, then take the left-most 256 bits and base64url encode them.
            //     The c_hash value is a case sensitive string.
            public const string AuthorizationCodeHash = "c_hash";

            //
            // Summary:
            //     Time the End-User's information was last updated. Its value is a JSON number
            //     representing the number of seconds from 1970-01-01T0:0:0Z as measured in UTC
            //     until the date/time.
            public const string UpdatedAt = "updated_at";

            //
            // Summary:
            //     String value used to associate a Client session with an ID Token, and to mitigate
            //     replay attacks. The value is passed through unmodified from the Authentication
            //     Request to the ID Token. If present in the ID Token, Clients MUST verify that
            //     the nonce Claim Value is equal to the value of the nonce parameter sent in the
            //     Authentication Request. If present in the Authentication Request, Authorization
            //     Servers MUST include a nonce Claim in the ID Token with the Claim Value being
            //     the nonce value sent in the Authentication Request. Authorization Servers SHOULD
            //     perform no other processing on nonce values used. The nonce value is a case sensitive
            //     string.
            public const string Nonce = "nonce";

            //
            // Summary:
            //     Defines a set of event statements that each may add additional claims to fully
            //     describe a single logical event that has occurred.
            public const string Events = "events";

            //
            // Summary:
            //     OAuth 2.0 Client Identifier valid at the Authorization Server.
            public const string ClientId = "client_id";


            public const string ClientPrefix = "client_prefix";
            
            //
            // Summary:
            //     Return all usernames in the client {client_user_name}
            public const string UserClientNames = "client_user_name";

            //
            // Summary:
            //     OpenID Connect requests MUST contain the "openid" scope value. If the openid
            //     scope value is not present, the behavior is entirely unspecified. Other scope
            //     values MAY be present. Scope values used that are not understood by an implementation
            //     SHOULD be ignored.
            public const string Scope = "scope";

            //
            // Summary:
            //     The "act" (actor) claim provides a means within a JWT to express that delegation
            //     has occurred and identify the acting party to whom authority has been delegated.The
            //     "act" claim value is a JSON object and members in the JSON object are claims
            //     that identify the actor. The claims that make up the "act" claim identify and
            //     possibly provide additional information about the actor.
            public const string Actor = "act";

            //
            // Summary:
            //     The "may_act" claim makes a statement that one party is authorized to become
            //     the actor and act on behalf of another party. The claim value is a JSON object
            //     and members in the JSON object are claims that identify the party that is asserted
            //     as being eligible to act for the party identified by the JWT containing the claim.
            public const string MayAct = "may_act";

            //
            // Summary:
            //     an identifier
            public const string Id = "id";

            //
            // Summary:
            //     The identity provider
            public const string IdentityProvider = "idp";

            //
            // Summary:
            //     The role
            public const string Role = "role";

            //
            // Summary:
            //     The DynamicPolicy
            public const string DynamicPolicy = "DynamicPolicy";

            //
            // Summary:
            //     The permissions
            public const string Permissions = "permissions";

            //
            // Summary:
            //     The role id
            public const string RoleId = "role_id";

            //
            // Summary:
            //     The admin role id
            public const string AdminRoleId = "admin_role_id";

            //
            // Summary:
            //     The term "effective_role" is the amount of system user selection
            //     to determine the role of other users
            public const string EffectiveRole = "effective_role";

            //
            // Summary:
            //     JWT ID. A unique identifier for the token, which can be used to prevent reuse
            //     of the token. These tokens MUST only be used once, unless conditions for reuse
            //     were negotiated between the parties; any such negotiation is beyond the scope
            //     of this specification.
            public const string JwtId = "jti";

            //
            // Summary:
            //     The exp (expiration time) claim identifies the expiration time on or after which
            //     the token MUST NOT be accepted for processing, specified as the number of seconds
            //     from 1970-01-01T0:0:0Z
            public const string Expiration = "exp";

            //
            // Summary:
            //     The time before which the JWT MUST NOT be accepted for processing, specified
            //     as the number of seconds from 1970-01-01T0:0:0Z
            public const string NotBefore = "nbf";

            //
            // Summary:
            //     Issuer Identifier for the Issuer of the response. The iss value is a case sensitive
            //     URL using the https scheme that contains scheme, host, and optionally, port number
            //     and path components and no query or fragment components.
            public const string Issuer = "iss";

            //
            // Summary:
            //     End-User's full name in displayable form including all name parts, possibly including
            //     titles and suffixes, ordered according to the End-User's locale and preferences.
            public const string Name = "name";

            //
            // Summary:
            //     Given name(s) or first name(s) of the End-User. Note that in some cultures, people
            //     can have multiple given names; all can be present, with the names being separated
            //     by space characters.
            public const string GivenName = "given_name";

            //
            // Summary:
            //     Surname(s) or last name(s) of the End-User. Note that in some cultures, people
            //     can have multiple family names or no family name; all can be present, with the
            //     names being separated by space characters.
            public const string FamilyName = "family_name";

            //
            // Summary:
            //     Middle name(s) of the End-User. Note that in some cultures, people can have multiple
            //     middle names; all can be present, with the names being separated by space characters.
            //     Also note that in some cultures, middle names are not used.
            public const string MiddleName = "middle_name";

            //
            // Summary:
            //     Casual name of the End-User that may or may not be the same as the given_name.
            //     For instance, a nickname value of Mike might be returned alongside a given_name
            //     value of Michael.
            public const string NickName = "nickname";

            //
            // Summary:
            //     Shorthand name by which the End-User wishes to be referred to at the RP, such
            //     as janedoe or j.doe. This value MAY be any valid JSON string including special
            //     characters such as @, /, or whitespace. The relying party MUST NOT rely upon
            //     this value being unique
            //
            // Remarks:
            //     The RP MUST NOT rely upon this value being unique, as discussed in http://openid.net/specs/openid-connect-basic-1_0-32.html#ClaimStability
            public const string PreferredUserName = "preferred_username";

            //
            // Summary:
            //     URL of the End-User's profile page. The contents of this Web page SHOULD be about
            //     the End-User.
            public const string Profile = "profile";

            //
            // Summary:
            //     URL of the End-User's profile picture. This URL MUST refer to an image file (for
            //     example, a PNG, JPEG, or GIF image file), rather than to a Web page containing
            //     an image.
            //
            // Remarks:
            //     Note that this URL SHOULD specifically reference a profile photo of the End-User
            //     suitable for displaying when describing the End-User, rather than an arbitrary
            //     photo taken by the End-User.
            public const string Picture = "picture";

            public const string MimeType = "mime_type";

            //
            // Summary:
            //     URL of the End-User's Web page or blog. This Web page SHOULD contain information
            //     published by the End-User or an organization that the End-User is affiliated
            //     with.
            public const string WebSite = "website";

            //
            // Summary:
            //     End-User's preferred e-mail address. Its value MUST conform to the RFC 5322 [RFC5322]
            //     addr-spec syntax. The relying party MUST NOT rely upon this value being unique
            public const string Email = "email";

            //
            // Summary:
            //     "true" if the End-User's e-mail address has been verified; otherwise "false".
            //
            // Remarks:
            //     When this Claim Value is "true", this means that the OP took affirmative steps
            //     to ensure that this e-mail address was controlled by the End-User at the time
            //     the verification was performed. The means by which an e-mail address is verified
            //     is context-specific, and dependent upon the trust framework or contractual agreements
            //     within which the parties are operating.
            public const string EmailVerified = "email_verified";

            //
            // Summary:
            //     End-User's gender. Values defined by this specification are "female" and "male".
            //     Other values MAY be used when neither of the defined values are applicable.
            public const string Gender = "gender";

            //
            // Summary:
            //     End-User's birthday, represented as an ISO 8601:2004 [ISO8601‑2004] YYYY-MM-DD
            //     format. The year MAY be 0000, indicating that it is omitted. To represent only
            //     the year, YYYY format is allowed. Note that depending on the underlying platform's
            //     date related function, providing just year can result in varying month and day,
            //     so the implementers need to take this factor into account to correctly process
            //     the dates.
            public const string BirthDate = "birthdate";

            //
            // Summary:
            //     String from the time zone database (http://www.twinsun.com/tz/tz-link.htm) representing
            //     the End-User's time zone. For example, Europe/Paris or America/Los_Angeles.
            public const string ZoneInfo = "zoneinfo";

            //
            // Summary:
            //     End-User's locale, represented as a BCP47 [RFC5646] language tag. This is typically
            //     an ISO 639-1 Alpha-2 [ISO639‑1] language code in lowercase and an ISO 3166-1
            //     Alpha-2 [ISO3166‑1] country code in uppercase, separated by a dash. For example,
            //     en-US or fr-CA. As a compatibility note, some implementations have used an underscore
            //     as the separator rather than a dash, for example, en_US; Relying Parties MAY
            //     choose to accept this locale syntax as well.
            public const string Locale = "locale";

            //
            // Summary:
            //     End-User's preferred telephone number. E.164 (https://www.itu.int/rec/T-REC-E.164/e)
            //     is RECOMMENDED as the format of this Claim, for example, +1 (425) 555-1212 or
            //     +56 (2) 687 2400. If the phone number contains an extension, it is RECOMMENDED
            //     that the extension be represented using the RFC 3966 [RFC3966] extension syntax,
            //     for example, +1 (604) 555-1234;ext=5678.
            public const string PhoneNumber = "phone_number";

            //
            // Summary:
            //     True if the End-User's phone number has been verified; otherwise false. When
            //     this Claim Value is true, this means that the OP took affirmative steps to ensure
            //     that this phone number was controlled by the End-User at the time the verification
            //     was performed.
            //
            // Remarks:
            //     The means by which a phone number is verified is context-specific, and dependent
            //     upon the trust framework or contractual agreements within which the parties are
            //     operating. When true, the phone_number Claim MUST be in E.164 format and any
            //     extensions MUST be represented in RFC 3966 format.
            public const string PhoneNumberVerified = "phone_number_verified";

            //
            // Summary:
            //     End-User's preferred postal address. The value of the address member is a JSON
            //     structure containing some or all of the members defined in http://openid.net/specs/openid-connect-basic-1_0-32.html#AddressClaim
            public const string Address = "address";

            //
            // Summary:
            //     Audience(s) that this ID Token is intended for. It MUST contain the OAuth 2.0
            //     client_id of the Relying Party as an audience value. It MAY also contain identifiers
            //     for other audiences. In the general case, the aud value is an array of case sensitive
            //     strings. In the common special case when there is one audience, the aud value
            //     MAY be a single case sensitive string.
            public const string Audience = "aud";

            //
            // Summary:
            //     The reference token identifier
            public const string ReferenceTokenId = "reference_token_id";

            //
            // Summary:
            //     The confirmation
            public const string Confirmation = "cnf";
        }

        //
        // Summary:
        //     Defines constants for the well-known claim types that can be assigned to a subject.
        //     This class cannot be inherited.
        public static class IdentityTypes
        {
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
            //
            // Summary:
            //     http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor.
            public const string Actor = "http://schemas.xmlsoap.org/ws/2009/09/identity/claims/actor";

            //
            // Summary:
            //     The URI for a claim that specifies the postal code of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/postalcode.
            public const string PostalCode = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/postalcode";

            //
            // Summary:
            //     The URI for a claim that specifies the primary group SID of an entity, http://schemas.microsoft.com/ws/2008/06/identity/claims/primarygroupsid.
            public const string PrimaryGroupSid =
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/primarygroupsid";

            //
            // Summary:
            //     The URI for a claim that specifies the primary SID of an entity, http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid.
            public const string PrimarySid = "http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid";

            //
            // Summary:
            //     The URI for a claim that specifies the role of an entity, http://schemas.microsoft.com/ws/2008/06/identity/claims/role.
            public const string Role = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

            //
            // Summary:
            //     The URI for a claim that specifies an RSA key, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/rsa.
            public const string Rsa = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/rsa";

            //
            // Summary:
            //     The URI for a claim that specifies a serial number, http://schemas.microsoft.com/ws/2008/06/identity/claims/serialnumber.
            public const string SerialNumber = "http://schemas.microsoft.com/ws/2008/06/identity/claims/serialnumber";

            //
            // Summary:
            //     The URI for a claim that specifies a security identifier (SID), http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid.
            public const string Sid = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid";

            //
            // Summary:
            //     The URI for a claim that specifies a service principal name (SPN) claim, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/spn.
            public const string Spn = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/spn";

            //
            // Summary:
            //     The URI for a claim that specifies the state or province in which an entity resides,
            //     http://schemas.xmlsoap.org/ws/2005/05/identity/claims/stateorprovince.
            public const string StateOrProvince =
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/stateorprovince";

            //
            // Summary:
            //     The URI for a claim that specifies the street address of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/streetaddress.
            public const string StreetAddress = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/streetaddress";

            //
            // Summary:
            //     The URI for a claim that specifies the surname of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname.
            public const string Surname = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";

            //
            // Summary:
            //     The URI for a claim that identifies the system entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/system.
            public const string System = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/system";

            //
            // Summary:
            //     The URI for a claim that specifies a thumbprint, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/thumbprint.
            //     A thumbprint is a globally unique SHA-1 hash of an X.509 certificate.
            public const string Thumbprint = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/thumbprint";

            //
            // Summary:
            //     The URI for a claim that specifies a user principal name (UPN), http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn.
            public const string Upn = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";

            //
            // Summary:
            //     The URI for a claim that specifies a URI, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/uri.
            public const string Uri = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/uri";

            //
            // Summary:
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata.
            public const string UserData = "http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata";

            //
            // Summary:
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/version.
            public const string Version = "http://schemas.microsoft.com/ws/2008/06/identity/claims/version";

            //
            // Summary:
            //     The URI for a claim that specifies the webpage of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/webpage.
            public const string Webpage = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/webpage";

            //
            // Summary:
            //     The URI for a claim that specifies the Windows domain account name of an entity,
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname.
            public const string WindowsAccountName =
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname";

            //
            // Summary:
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsdeviceclaim.
            public const string WindowsDeviceClaim =
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsdeviceclaim";

            //
            // Summary:
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsdevicegroup.
            public const string WindowsDeviceGroup =
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsdevicegroup";

            //
            // Summary:
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsfqbnversion.
            public const string WindowsFqbnVersion =
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsfqbnversion";

            //
            // Summary:
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/windowssubauthority.
            public const string WindowsSubAuthority =
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/windowssubauthority";

            //
            // Summary:
            //     The URI for a claim that specifies the alternative phone number of an entity,
            //     http://schemas.xmlsoap.org/ws/2005/05/identity/claims/otherphone.
            public const string OtherPhone = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/otherphone";

            //
            // Summary:
            //     The URI for a claim that specifies the name of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier.
            public const string NameIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

            //
            // Summary:
            //     The URI for a claim that specifies the name of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name.
            public const string Name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

            //
            // Summary:
            //     The URI for a claim that specifies the mobile phone number of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone.
            public const string MobilePhone = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone";

            //
            // Summary:
            //     The URI for a claim that specifies the anonymous user; http://schemas.xmlsoap.org/ws/2005/05/identity/claims/anonymous.
            public const string Anonymous = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/anonymous";

            //
            // Summary:
            //     The URI for a claim that specifies details about whether an identity is authenticated,
            //     http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authenticated.
            public const string Authentication = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication";

            //
            // Summary:
            //     The URI for a claim that specifies the instant at which an entity was authenticated;
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant.
            public const string AuthenticationInstant =
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationinstant";

            //
            // Summary:
            //     The URI for a claim that specifies the method with which an entity was authenticated;
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod.
            public const string AuthenticationMethod =
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod";

            //
            // Summary:
            //     The URI for a claim that specifies an authorization decision on an entity; http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authorizationdecision.
            public const string AuthorizationDecision =
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authorizationdecision";

            //
            // Summary:
            //     The URI for a claim that specifies the cookie path; http://schemas.microsoft.com/ws/2008/06/identity/claims/cookiepath.
            public const string CookiePath = "http://schemas.microsoft.com/ws/2008/06/identity/claims/cookiepath";

            //
            // Summary:
            //     The URI for a claim that specifies the country/region in which an entity resides,
            //     http://schemas.xmlsoap.org/ws/2005/05/identity/claims/country.
            public const string Country = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/country";

            //
            // Summary:
            //     The URI for a claim that specifies the date of birth of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dateofbirth.
            public const string DateOfBirth = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dateofbirth";

            //
            // Summary:
            //     The URI for a claim that specifies the deny-only primary group SID on an entity;
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlyprimarygroupsid.
            //     A deny-only SID denies the specified entity to a securable object.
            public const string DenyOnlyPrimaryGroupSid =
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlyprimarygroupsid";

            //
            // Summary:
            //     The URI for a claim that specifies the deny-only primary SID on an entity; http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlyprimarysid.
            //     A deny-only SID denies the specified entity to a securable object.
            public const string DenyOnlyPrimarySid =
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlyprimarysid";

            //
            // Summary:
            //     The URI for a claim that specifies a deny-only security identifier (SID) for
            //     an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/denyonlysid.
            //     A deny-only SID denies the specified entity to a securable object.
            public const string DenyOnlySid = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/denyonlysid";

            //
            // Summary:
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsuserclaim.
            public const string WindowsUserClaim =
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsuserclaim";

            //
            // Summary:
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlywindowsdevicegroup.
            public const string DenyOnlyWindowsDeviceGroup =
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/denyonlywindowsdevicegroup";

            //
            // Summary:
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/dsa.
            public const string Dsa = "http://schemas.microsoft.com/ws/2008/06/identity/claims/dsa";

            //
            // Summary:
            //     The URI for a claim that specifies the email address of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/email.
            public const string Email = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

            //
            // Summary:
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/expiration.
            public const string Expiration = "http://schemas.microsoft.com/ws/2008/06/identity/claims/expiration";

            //
            // Summary:
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/expired.
            public const string Expired = "http://schemas.microsoft.com/ws/2008/06/identity/claims/expired";

            //
            // Summary:
            //     The URI for a claim that specifies the gender of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/gender.
            public const string Gender = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/gender";

            //
            // Summary:
            //     The URI for a claim that specifies the given name of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname.
            public const string GivenName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";

            //
            // Summary:
            //     The URI for a claim that specifies the SID for the group of an entity, http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid.
            public const string GroupSid = "http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid";

            //
            // Summary:
            //     The URI for a claim that specifies a hash value, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/hash.
            public const string Hash = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/hash";

            //
            // Summary:
            //     The URI for a claim that specifies the home phone number of an entity, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/homephone.
            public const string HomePhone = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/homephone";

            //
            // Summary:
            //     http://schemas.microsoft.com/ws/2008/06/identity/claims/ispersistent.
            public const string IsPersistent = "http://schemas.microsoft.com/ws/2008/06/identity/claims/ispersistent";

            //
            // Summary:
            //     The URI for a claim that specifies the locale in which an entity resides, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/locality.
            public const string Locality = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/locality";

            //
            // Summary:
            //     The URI for a claim that specifies the DNS name associated with the computer
            //     name or with the alternative name of either the subject or issuer of an X.509
            //     certificate, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dns.
            public const string Dns = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dns";

            //
            // Summary:
            //     The URI for a distinguished name claim of an X.509 certificate, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/x500distinguishedname.
            //     The X.500 standard defines the methodology for defining distinguished names that
            //     are used by X.509 certificates.
            public const string X500DistinguishedName =
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/x500distinguishedname";
        }
    }
}