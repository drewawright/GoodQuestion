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
        // GET : All user playlists from spotify
        public IHttpActionResult GetAllPlaylistsSpotify(string spotifyId)
        {
            var service = CreatePlaylistServices();
            var playlists = service.GetAllUserPlaylistsSpotify(spotifyId);

            return Ok(playlists);
        }

        // GET : playlist details
        public IHttpActionResult GetPlaylistDetails(string playlistId)
        {
            var service = CreatePlaylistServices();
            var details = service.GetPlaylistDetail(playlistId);

            return Ok(details);
        }

        // POST : Update Db Playlist
        public IHttpActionResult UpdateDbPlaylist(string playlistId)
        {
            var service = CreatePlaylistServices();
            var update = service.UpdateDbPlaylist(playlistId);

            return Ok(update);
        }

        private PlaylistServices CreatePlaylistServices()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var playlistServices = new PlaylistServices(userId);
            return playlistServices;
        }
    }
}
