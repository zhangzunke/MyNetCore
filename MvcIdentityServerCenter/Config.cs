using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace MvcIdentityServerCenter
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "API Application")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client()
                {
                    ClientId = "mvc",
                    ClientName = "Mvc Client",
                    ClientUri = "http://localhost:5001",
                    LogoUri = "https://chocolatey.org/content/packageimages/dotnetcore-runtime.install.2.0.5.png",
                    AllowRememberConsent = true,
                    AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,
                    AllowAccessTokensViaBrowser = true,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RequireConsent = true,
                    RedirectUris = { "http://localhost:5001/signin-oidc"},
                    PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc" },
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email
                    }
                }
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "10000",
                    Username = "mike",
                    Password = "123qwe",
                    Claims = new List<Claim>
                    {
                        new Claim("name", "Michael Zhang"),
                        new Claim("website", "http://www.baidu.com")
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }
    }
}
