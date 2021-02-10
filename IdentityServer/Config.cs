// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("products.read", "Read products"),
                new ApiScope("products.write", "Write products"),
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("products", "Represents the products domain")
                { 
                    Scopes = 
                    { 
                        "products.read",
                        "products.write" 
                    }
                }
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedScopes = { "products.read", "products.write" }
                },

                // SPA client using code flow + pkce
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "http://identityserver.io",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =
                    {
                        "http://localhost:5002/index.html",
                        "http://localhost:5002/callback.html",
                        "http://localhost:5002/silent.html",
                        "http://localhost:5002/popup.html",
                    },

                    BackChannelLogoutUri = "http://localhost:5002/logout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:5002/index.html" },
                    AllowedCorsOrigins = { "http://localhost:5002" },

                    AllowedScopes = { "openid", "profile", "products.read" }
                },
                new Client
                {
                    ClientId = "mvc1",
                    ClientName = "MVC Client",
                    ClientUri = "http://identityserver.io",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =
                    {
                        "https://localhost:5003/signin-oidc",
                    },

                    BackChannelLogoutUri = "http://localhost:5003/logout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:5003/index.html" },
                    AllowedCorsOrigins = { "http://localhost:5003" },

                    AllowedScopes = { "openid", "profile", "products.read" }
                },
                

                // Device Code client
                new Client
                {
                    ClientId = "cli",
                    ClientName = "Device Code Client",
                    RequireClientSecret = false,

                    AllowedGrantTypes = new[] { "urn:ietf:params:oauth:grant-type:device_code" },
                    AllowedScopes = { "openid", "profile", "products.read", "products.write" }
                }
            };
    }
}
