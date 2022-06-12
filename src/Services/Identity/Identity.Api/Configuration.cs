using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Identity.Api
{
    internal static class Configuration
    {
        internal static IEnumerable<Client> Clients
            => new List<Client>
            {
                new Client
                {
                    ClientId = "SPA",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireClientSecret = false,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "disk.api.read"
                    },
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AlwaysSendClientClaims = true,
                    AllowAccessTokensViaBrowser = true,
                    // AllowedCorsOrigins = { "https://oauth.pstmn.io" },
                    RedirectUris = { "https://oauth.pstmn.io/v1/callback", "http://localhost:5000/" },
                },
                new Client
                {
                    ClientId = "Passworded",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RequireClientSecret = false,
                    AllowedScopes =
                    {
                        "disk.api.read",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        JwtClaimTypes.Role
                    },
                    AllowOfflineAccess = false,
                },
                new Client()
                {
                    ClientId = "WebApiClientId",
                    ClientSecrets =
                    {
                        new Secret("WebApiClientSecret".ToSha256())
                    },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes =
                    {
                        "disk.api.read",
                        "disk.api.write"
                    },
                    RequireConsent = false
                }
            };

        internal static IEnumerable<ApiScope> IdentityApiScopes
            => new List<ApiScope>
            {
                new ApiScope("disk.api.read", "Disk API read"),
                new ApiScope("disk.api.write", "Disk API write"),
            };

        internal static IEnumerable<IdentityResource> IdentityResources
            => new List<IdentityResource>
            {
                new IdentityResources.Profile(),
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResource
                    {
                        Name = JwtClaimTypes.Role,
                        UserClaims = new List<string> {JwtClaimTypes.Role}
                    }
            };

        internal static IEnumerable<ApiResource> ApiResources
            => new List<ApiResource>
            {
                new ApiResource(){
                    Name = "DiskApi", // audience (a.k.a ApiName)
                    DisplayName = "Disk API",
                    Scopes = {
                        "disk.api.read",
                        "disk.api.write",
                    },
                    UserClaims = new List<string> {JwtClaimTypes.Role}
                }
            };
    }
}