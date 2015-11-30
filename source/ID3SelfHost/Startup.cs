using Owin;
using SelfHost.Config;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Host.Config;
using IdentityServer3.WsFederation.Configuration;
using IdentityServer3.WsFederation.Models;
using IdentityServer3.WsFederation.Services;
using Microsoft.Owin.Security.Facebook;
using SelfHost.Provider;
using SelfHost.Services;
using Serilog;

namespace SelfHost
{
    internal class Startup
    {
		public static FacebookAuthenticationOptions FacebookAuthOptions { get; private set; }

		public void Configuration(IAppBuilder appBuilder)
        {
			//AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
			//JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

			Log.Logger = new LoggerConfiguration()
			   .MinimumLevel.Debug()
			   .WriteTo.LiterateConsole(outputTemplate: "{Timestamp} [{Level}] ({Name}){NewLine} {Message}{NewLine}{Exception}")
			   .CreateLogger();

			var factory = new IdentityServerServiceFactory()
                            .UseInMemoryClients(Clients.Get())
                            .UseInMemoryScopes(Scopes.Get());

			// Register our custom user service
			factory.UserService = new Registration<IUserService>(new CustomInMemoryUserService(Users.Get()));
			
            var options = new IdentityServerOptions
            {
                SiteName = "Kroll Secure Token Service",
				IssuerUri = "https://kroll-sts-local",
				
                SigningCertificate = Certificate.Get(),
                Factory = factory,
                PluginConfiguration = ConfigurePlugins,
				AuthenticationOptions = new IdentityServer3.Core.Configuration.AuthenticationOptions
				{
					EnablePostSignOutAutoRedirect = true,
					IdentityProviders = ConfigureIdentityProviders,
					EnableLocalLogin = false
				}
			};

            appBuilder.UseIdentityServer(options);
        }

		private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
		{
			FacebookAuthOptions = new FacebookAuthenticationOptions()
			{
				AppId = "952496471482222",
				AppSecret = "f7f3ea9d9d714ba6f46edae7866dec89",
				Provider = new FacebookAuthProvider(),
				SignInAsAuthenticationType = signInAsType
			};

			app.UseFacebookAuthentication(FacebookAuthOptions);

		}

		private void ConfigurePlugins(IAppBuilder pluginApp, IdentityServerOptions options)
        {
            var wsFedOptions = new WsFederationPluginOptions(options);

            // data sources for in-memory services
            wsFedOptions.Factory.Register(new Registration<IEnumerable<RelyingParty>>(RelyingParties.Get()));
            wsFedOptions.Factory.RelyingPartyService = new Registration<IRelyingPartyService>(typeof(InMemoryRelyingPartyService));
			
	        wsFedOptions.EnableMetadataEndpoint = true;
	        
            pluginApp.UseWsFederationPlugin(wsFedOptions);
        }
    }
}