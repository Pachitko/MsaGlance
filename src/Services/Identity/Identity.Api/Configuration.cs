using System;
using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace Identity.Api
{
    internal static class Configuration
    {
        internal static IEnumerable<Client> GetClients()
            => new List<Client>
            {
                new Client()
                {
                    ClientId = "Client_id",
                    ClientSecrets =
                    {
                        new Secret("Secrets".ToSha256())
                    },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes =
                    {
                        "DiskAPI"
                    }
                }
            };

        internal static IEnumerable<IdentityResource> GetIdentityResources()
            => new List<IdentityResource>
            {
                new IdentityResources.OpenId()
            };

        internal static IEnumerable<ApiResource> GetApiResources()
            => new List<ApiResource>
            {
                new ApiResource("DiskAPI")
            };
    }
}