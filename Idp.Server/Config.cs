﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using Idp.Server.Entities;
using System;
using System.Collections.Generic;

namespace Idp.Server
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
                {
                };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[] {};

        public static IEnumerable<Client> Clients =>
            new Client[] 
            {
            };

        public static IEnumerable<User> Users =>
            new User[] 
            {};

    }
}