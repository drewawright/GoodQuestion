using GoodQuestion.Services;
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
        //GET api/Song/GetFromSpotify
        public IHttpActionResult GetFromSpotify(string playlistId)
        {
            var svc = CreateSongServices();
            var songs = svc.GetSongsInPlaylist(playlistId);
            return Ok(songs);
        }

        //PUT api/Song/
        public IHttpActionResult PutRefreshPlaylistSongsArtwork(string playlistId)
        {
            var svc = CreateSongServices();
            var refreshed = svc.RefreshPlaylistSongsArtwork(playlistId);
            return Ok(refreshed);
        }

        private SongServices CreateSongServices()
        {
            var songService = new SongServices();
            return songService;
        } 

        // GET Song Index
        public IHttpActionResult GetIndexDb(string playlistId)
        {
            SongServices songService = CreateSongServices();
            var songs = songService.GetSongIndexDb(playlistId);

            return Ok(songs);
        }
    }
}
