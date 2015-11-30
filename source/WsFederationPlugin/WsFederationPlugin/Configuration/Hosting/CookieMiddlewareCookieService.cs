﻿/*
 * Copyright 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using IdentityServer3.WsFederation.Logging;
using Microsoft.Owin;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

#pragma warning disable 1591

namespace IdentityServer3.WsFederation.Hosting
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class CookieMiddlewareTrackingCookieService : ITrackingCookieService
    {
        private readonly static ILog Logger = LogProvider.GetCurrentClassLogger();
        private readonly IOwinContext _context;

        public CookieMiddlewareTrackingCookieService(IOwinContext context)
        {
            _context = context;
        }

        public async Task AddValueAsync(string name, string value)
        {
            var urls = await GetValuesAsync(name);

            var duplicateUrl = urls.FirstOrDefault(s => s == value);
            if (duplicateUrl != null)
            {
                Logger.DebugFormat("{0} already exists in {1} cookie", value, name);
                return;
            }

            Logger.DebugFormat("Adding {0} to {1} cookie", value, name);
            urls.Add(value);

            var claims = new List<Claim>(from u in urls select new Claim("url", u));
            var id = new ClaimsIdentity(claims, name);

            _context.Authentication.SignIn(id);
        }

        public async Task<IEnumerable<string>> GetValuesAndDeleteCookieAsync(string name)
        {
            var urls = await GetValuesAsync(name);

            Logger.DebugFormat("Removing cookie {0}", name);
            _context.Authentication.SignOut(name);

            return urls;
        }

        async Task<List<string>> GetValuesAsync(string name)
        {
            Logger.DebugFormat("Retrieving values of cookie {0}", name);
            var result = await _context.Authentication.AuthenticateAsync(name);
            
            if (result == null || result.Identity == null)
            {
                Logger.DebugFormat("Cookie {0} does not exist", name);
                return new List<string>();
            }

            var urls = from c in result.Identity.Claims
                       where c.Type == "url"
                       select c.Value;

            return urls.ToList();
        }
    }
}