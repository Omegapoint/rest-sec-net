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
                new IdentityResources.Email(),
            };


        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope(Scopes.ProductsRead, "Read products"),
                new ApiScope(Scopes.ProductsWrite, "Write products"),
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("products", "Represents the products domain")
                {
                    Scopes =
                    {
                        Scopes.ProductsRead,
                        Scopes.ProductsWrite
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

                    AllowedScopes = { Scopes.ProductsRead, Scopes.ProductsWrite }
                },

                // MVC client using code flow + pkce, with one time refresh-tokens
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    ClientUri = "https://localhost:5003",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    AllowOfflineAccess = true,
                    RequireConsent = true,
                    AllowRememberConsent = true,

                    RedirectUris =
                    {
                        "https://localhost:5003/signin-oidc",
                    },
                    
                    PostLogoutRedirectUris = { "https://localhost:5003/signout-callback-oidc" },

                    AllowedScopes = { Scopes.Openid, Scopes.Profile, Scopes.Email, Scopes.OfflineAccess, Scopes.ProductsRead, Scopes.ProductsWrite }
                },
                
                // Device Code client
                new Client
                {
                    ClientId = "cli",
                    ClientName = "Device Code Client",
                    RequireClientSecret = false,

                    AllowedGrantTypes = new[] { "urn:ietf:params:oauth:grant-type:device_code" },
                    AllowedScopes = { Scopes.Openid, Scopes.Profile, Scopes.ProductsRead }
                }
            };
    }

    public static class Scopes
    {
        //OIDC
        public const string Openid = "openid";
        public const string Profile = "profile";
        public const string Email = "email";
        public const string OfflineAccess = "offline_access";

        //Custom
        public const string ProductsRead = "products.read";
        public const string ProductsWrite = "products.write";
    }
}
