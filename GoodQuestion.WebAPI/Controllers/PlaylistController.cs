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
        public IHttpActionResult GetPlaylistIndex()
        {
            var svc = CreatePlaylistServices();
            var playlists = svc.GetPlaylistIndex();
            return Ok(playlists);
        }

        //DELETE api/Playlist/Delete 
        public IHttpActionResult DeletePlaylist(string playlistId)
        {
            var svc = CreatePlaylistServices();

            if (!svc.DeletePlaylistDb(playlistId))
                return InternalServerError();

            return Ok();
        }

        private PlaylistServices CreatePlaylistServices()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            var playlistServices = new PlaylistServices(userId);
            return playlistServices;
        }
    }
}
