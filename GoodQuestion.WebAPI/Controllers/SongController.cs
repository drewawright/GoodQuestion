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
    [RoutePrefix("api/Song")]
    public class SongController : ApiController
    {
        // GET Song Index
        [Route("Index/{playlistId}")]
        public IHttpActionResult GetIndexDb(string playlistId)
        {
            SongServices songService = CreateSongServices();
            var songs = songService.GetSongIndexDb(playlistId);

            return Ok(songs);
        }

        // GET api/Song/GetSongDetail
        [Route("Detail/{songId}")]
        public IHttpActionResult GetSongDetail(string songId)
        {
            var svc = CreateSongServices();
            var song = svc.GetSongDetail(songId);
            return Ok(song);
        }


        //GET api/Song/GetFromSpotify
        [Route("{playlistId}")]
        public IHttpActionResult GetFromSpotify(string playlistId)
        {
            var svc = CreateSongServices();
            var songs = svc.GetSongsInPlaylist(playlistId);
            return Ok(songs);
        }

        //PUT api/Song/
        [Route("{playlistId}")]
        public IHttpActionResult PutRefreshPlaylistSongsArtwork(string playlistId)
        {
            var svc = CreateSongServices();
            var refreshed = svc.RefreshPlaylistSongsArtwork(playlistId);
            return Ok(refreshed);
        }

        //DELETE api/Song/DeleteSong
        [Route("Delete/{songId}")]
        public IHttpActionResult DeleteSong(string songId)
        {
            var svc = CreateSongServices();

            if (!svc.DeleteSongDb(songId))
                return InternalServerError();

            return Ok();
        }

        private SongServices CreateSongServices()
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
            var songService = new SongServices(userId);
            songService.SetToken();

            return songService;
        }

    }
}
