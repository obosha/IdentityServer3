using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.WsFederation.Models;
using IdentityModel.Constants;

namespace SelfHost.Config
{
    public class RelyingParties
    {
        public static IEnumerable<RelyingParty> Get()
        {
            return new List<RelyingParty>
            {   
                new RelyingParty()
				{
					Enabled = true,
					ClaimMappings = new Dictionary<string, string>
					{
						{ "sub", ClaimTypes.NameIdentifier },
						{ "name", ClaimTypes.Name },
						{ "external_provider_user_id", ClaimTypes.Upn }
					},
                    Name = "ADFS Test",
					Realm = "http://sso.altegrity.com/adfs/services/trust",
					ReplyUrl = "https://sso.altegrity.com/adfs/ls"
				}
            };
        }
    }
}
