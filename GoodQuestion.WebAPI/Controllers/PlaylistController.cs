﻿using GoodQuestion.Services;
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

        //PUT api/Playlist
        public IHttpActionResult RefreshUserPlaylistsArtwork()
        {
            var svc = CreatePlaylistServices();
            var refreshed = svc.RefreshUserPlaylistsArtwork();
            return Ok(refreshed);
        }

        // GET api/Playlist/GetAllUserPlaylists
        public IHttpActionResult GetAllUserPlaylistsSpotify(string spotifyId)
        {
            var svc = CreatePlaylistServices();
            var playlists = svc.GetAllUserPlaylistsSpotify(spotifyId);

            return Ok(playlists);
        }

        // PUT api/Playlist/UpdateDbPlaylist
        public IHttpActionResult UpdateDbPlaylist(string playlistId)
        {
            var svc = CreatePlaylistServices();
            var playlistUpdate = svc.UpdateDbPlaylist(playlistId);

            return Ok(playlistUpdate);
        }

        // GET api/Playlist/PlaylistDetails
        public IHttpActionResult GetPlaylistDetails(string playlistId)
        {
            var svc = CreatePlaylistServices();
            var details = svc.GetPlaylistDetail(playlistId);

            return Ok(details);
        }

        private PlaylistServices CreatePlaylistServices()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var playlistServices = new PlaylistServices(userId);
            return playlistServices;
        }
    }
}
