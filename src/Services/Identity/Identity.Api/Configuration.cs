using System;
using System.Collections.Generic;
using IdentityModel;
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
                    ClientId = "client_id",
                    ClientSecrets =
                    {
                        new Secret("client_secret".ToSha256())
                    },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes =
                    {
                        "DiskApi"
                    }
                }
            };

        internal static IEnumerable<ApiScope> IdentityApiScopes
            => new List<ApiScope>
            {
                new ApiScope("DiskApi", "Disk API")
            };

        internal static IEnumerable<IdentityResource> IdentityResources
            => new List<IdentityResource>
            {
                new IdentityResources.OpenId()
            };

        internal static IEnumerable<ApiResource> ApiResources
            => new List<ApiResource>
            {
                new ApiResource("DiskApi")
            };
    }
}