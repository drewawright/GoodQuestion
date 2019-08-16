using GoodQuestion.Data;
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

        //GET api/Playlist/Index
        [Route("Index")]
        public IHttpActionResult GetPlaylistIndex()
        {
            var svc = CreatePlaylistServices();
            var playlists = svc.GetPlaylistIndex();
            return Ok(playlists);
        }

        //PUT api/Playlist
        [HttpPut]
        public IHttpActionResult RefreshUserPlaylistsArtwork()
        {
            var svc = CreatePlaylistServices();
            var refreshed = svc.RefreshUserPlaylistsArtwork();
            return Ok(refreshed);
        }

        //DELETE api/Playlist/Delete
        [Route("Delete/{playlistId}")]
        public IHttpActionResult DeletePlaylist(string playlistId)
        {
            var svc = CreatePlaylistServices();

            if (!svc.DeletePlaylistDb(playlistId))
                return InternalServerError();

            return Ok();
        }

        // GET api/Playlist/GetAllUserPlaylists
        [Route("Spotify")]
        public IHttpActionResult GetAllUserPlaylistsSpotify()
        {
            var svc = CreatePlaylistServices();
            var playlists = svc.GetAllUserPlaylistsSpotify();

            return Ok(playlists);
        }

        // PUT api/Playlist/UpdateDbPlaylist
        [Route("{playlistId}")]
        public IHttpActionResult UpdateDbPlaylist(string playlistId)
        {
            var svc = CreatePlaylistServices();
            var playlistUpdate = svc.UpdateDbPlaylist(playlistId);

            return Ok(playlistUpdate);
        }

        // GET api/Playlist/PlaylistDetails
        [Route("Detail/{playlistId}")]
        public IHttpActionResult GetPlaylistDetails(string playlistId)
        {
            var svc = CreatePlaylistServices();
            var details = svc.GetPlaylistDetail(playlistId);

            return Ok(details);
        }

        private PlaylistServices CreatePlaylistServices()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());

            using (var ctx = new ApplicationDbContext())
            {
                var entity =
                    ctx
                    .Users
                    .Where(u => u.Id == userId.ToString())
                    .Single();

                if (entity.TokenExpiration < DateTime.Now)
                {
                    var accountController = new AccountController();
                    entity.SpotifyAuthToken = accountController.RefreshToken(entity.SpotifyRefreshToken).ToString();
                    entity.TokenExpiration = DateTime.Now.AddHours(1);
                }
            }

            var playlistServices = new PlaylistServices(userId);
            playlistServices.SetToken();
            return playlistServices;
        }
    }
}
