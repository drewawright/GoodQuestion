using Owin.Security.Providers.Spotify.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace GoodQuestion.WebAPI.Providers
{
    public class SpotifyAuthProvider : SpotifyAuthenticationProvider
    {
        public override Task Authenticated(SpotifyAuthenticatedContext context)
        {
            context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            return Task.FromResult<object>(null);
        }
    }
}