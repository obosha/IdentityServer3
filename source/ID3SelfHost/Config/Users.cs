using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.Core;
using IdentityServer3.Core.Services.InMemory;

namespace SelfHost.Config
{
    static class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser{Subject = "alice", Username = "alice", Password = "alice", 
					Enabled = true,
					Claims = new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "Alice"),
                        new Claim(ClaimTypes.GivenName, "Smith"),
                        new Claim(ClaimTypes.Email, "AliceSmith@email.com"),
						new Claim(ClaimTypes.NameIdentifier, "aliceTESTId"), 
                    }
                },
                new InMemoryUser{Subject = "bob", Username = "bob", Password = "bob", 
                    Claims = new Claim[]
                    {
						new Claim(ClaimTypes.Name, "Bob"),
						new Claim(ClaimTypes.GivenName, "Smith"),
						new Claim(ClaimTypes.Email, "bobSmith@email.com"),
						new Claim(ClaimTypes.NameIdentifier, "bobTESTId"),
					}
                },
            };
        }
    }
}