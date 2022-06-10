using System;
using System.Collections.Generic;
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
                        "disk.api.write",
                        "disk.api.full",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    RequireConsent = false
                }
            };

        internal static IEnumerable<ApiScope> IdentityApiScopes
            => new List<ApiScope>
            {
                new ApiScope("disk.api.read", "Disk API read"),
                new ApiScope("disk.api.write", "Disk API write"),
                new ApiScope("disk.api.full", "Disk API full")
            };

        internal static IEnumerable<IdentityResource> IdentityResources
            => new List<IdentityResource>
            {
                new IdentityResources.Profile(),
                new IdentityResources.OpenId()
            };

        // audience (a.k.a ApiName)
        internal static IEnumerable<ApiResource> ApiResources
            => new List<ApiResource>
            {
                new ApiResource(){
                    Name = "DiskApi",
                    DisplayName = "Disk API",
                    Scopes = {
                        "disk.api.read",
                        "disk.api.write",
                        "disk.api.full"
                    }
                }
            };
    }
}