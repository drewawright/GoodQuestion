using GoodQuestion.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GoodQuestion.WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Playlist")]
    public class PlaylistController : ApiController
    {
        private PlaylistServices CreatePlaylistServices()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var playlistServices = new PlaylistServices(userId);
            return playlistServices;
        }
    }
}
