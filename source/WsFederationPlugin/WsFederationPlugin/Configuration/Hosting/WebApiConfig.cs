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

using IdentityServer3.WsFederation.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;

namespace IdentityServer3.WsFederation.Configuration
{
    internal static class WebApiConfig
    {
        public static HttpConfiguration Configure(WsFederationPluginOptions options)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            config.SuppressDefaultHostAuthentication();

            config.MessageHandlers.Insert(0, new KatanaDependencyResolver());
            config.Services.Add(typeof(IExceptionLogger), new LogProviderExceptionLogger());
            config.Services.Replace(typeof(IHttpControllerTypeResolver), new HttpControllerTypeResolver());

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

            if (options.IdentityServerOptions.LoggingOptions.EnableWebApiDiagnostics)
            {
                var liblog = new TraceSource("LibLog");
                liblog.Switch.Level = SourceLevels.All;
                liblog.Listeners.Add(new LibLogTraceListener());

                var diag = config.EnableSystemDiagnosticsTracing();
                diag.IsVerbose = options.IdentityServerOptions.LoggingOptions.WebApiDiagnosticsIsVerbose;
                diag.TraceSource = liblog;
            }

            if (options.IdentityServerOptions.LoggingOptions.EnableHttpLogging)
            {
                config.MessageHandlers.Add(new RequestResponseLogger());
            }

            return config;
        }


        private class HttpControllerTypeResolver : IHttpControllerTypeResolver
        {
            public ICollection<Type> GetControllerTypes(IAssembliesResolver _)
            {
                var httpControllerType = typeof(IHttpController);
                return typeof(WebApiConfig)
                    .Assembly
                    .GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && httpControllerType.IsAssignableFrom(t))
                    .ToList();
            }
        }
    }
}