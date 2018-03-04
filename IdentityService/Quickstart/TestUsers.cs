// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer4.Quickstart.UI
{
    public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>
        {
            new TestUser{SubjectId = "818727", Username = "alice", Password = "alice", 
                Claims = 
                {
                    new Claim(JwtClaimTypes.Name, "Alice Svensson"),
                    new Claim(JwtClaimTypes.GivenName, "Alice"),
                    new Claim(JwtClaimTypes.FamilyName, "Svensson"),
                    new Claim(JwtClaimTypes.Email, "Alice.Svensson@example.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                }
            }
        };
    }
}